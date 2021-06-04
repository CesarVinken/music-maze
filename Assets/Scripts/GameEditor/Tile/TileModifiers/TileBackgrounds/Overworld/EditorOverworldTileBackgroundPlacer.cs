using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorOverworldTileBackgroundPlacer : OverworldTileBackgroundPlacer<EditorOverworldTile>
{
    private EditorOverworldTile _tile;

    public override EditorOverworldTile Tile { get => _tile; set => _tile = value; }

    public EditorOverworldTileBackgroundPlacer(EditorOverworldTile tile)
    {
        Tile = tile;
    }

    // Called in the editor, when we need to update the connection score
    public void PlacePath(IPathType overworldTilePathType)
    {
        Logger.Warning("Start placing path.....");

        TileConnectionScoreInfo pathConnectionScore = NeighbourTileCalculator.MapNeighbourPathsOfTile(Tile, overworldTilePathType);

        GameObject overworldTilePathGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<OverworldTilePath>(), Tile.BackgroundsContainer);
        OverworldTilePath overworldTilePath = overworldTilePathGO.GetComponent<OverworldTilePath>();
        overworldTilePath.WithType(overworldTilePathType as IBackgroundType);
        overworldTilePath.WithConnectionScoreInfo(pathConnectionScore);

        Tile.AddBackground(overworldTilePath as ITileBackground);

        // Update pathConnections for neighbouring tiles
        UpdatePathConnectionsOnNeighbours();
    }

    public ITileBackground PlaceWater<U>() where U : ITileBackground
    {
        TileBaseGround existingGround = Tile.TryGetTileGround();
        int oldLandConnectionScore = existingGround ? existingGround.ConnectionScore : -1;

        TileConnectionScoreInfo newLandConnectionScoreInfo = NeighbourTileCalculator.MapGroundConnectionsWithNeighbours(Tile, new OverworldDefaultGroundType());
        int newLandConnectionScore = newLandConnectionScoreInfo.RawConnectionScore;
        Logger.Log($"Old land connections score for tile {Tile.GridLocation.X},{Tile.GridLocation.Y} was {oldLandConnectionScore}. The New Score is {newLandConnectionScore}");

        // If there are no land connections left (value -1), remove the ground sprite
        if (newLandConnectionScore == -1)
        {
            RemoveGroundBackgroundFromCoveredTile(Tile, 16);
        }
        else if (existingGround == null)
        {
            //There are some connections and no ground sprite. Create ground sprite.
            GameObject backgroundGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseGround>(), Tile.BackgroundsContainer);
            OverworldTileBaseGround baseBackground = backgroundGO.GetComponent<OverworldTileBaseGround>();

            baseBackground.SetTile(Tile);
            baseBackground.WithType(new OverworldDefaultGroundType());
            baseBackground.WithConnectionScoreInfo(newLandConnectionScoreInfo);
            Tile.AddBackground(baseBackground);

        }
        else
        {
            existingGround.WithConnectionScoreInfo(newLandConnectionScoreInfo);
        }

        GameObject waterGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseWater>(), Tile.BackgroundsContainer);
        OverworldTileBaseWater overworldTileBaseWater = waterGO.GetComponent<OverworldTileBaseWater>();
        overworldTileBaseWater.SetTile(Tile);

        Tile.SetMainMaterial(new WaterMainMaterial());
        Tile.AddBackground(overworldTileBaseWater);
        Tile.SetWalkable(false);

        // Update pathConnections for neighbouring tiles
        UpdatePathConnectionsOnNeighbours();
        UpdateGroundConnectionsOnNeighbours(new OverworldDefaultGroundType());

        TryPlaceCornerFillers(Tile);
        TryPlaceCornerFillersForNeighbours();


        return overworldTileBaseWater;
    }

    public void PlaceCoveringBaseWater()
    {
        Tile.SetMainMaterial(new WaterMainMaterial());
        Logger.Log("Place a water tile without updating neighbours or removing land tiles.");

        GameObject waterGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseWater>(), Tile.BackgroundsContainer);
        OverworldTileBaseWater mazeTileBaseWater = waterGO.GetComponent<OverworldTileBaseWater>();
        mazeTileBaseWater.SetTile(Tile);

        Tile.AddBackground(mazeTileBaseWater);
    }

    public ITileBackground PlaceCoveringBaseGround()
    {
        Tile.SetMainMaterial(new GroundMainMaterial());

        Logger.Log("Place a base ground tile will connections on all sides.");
        GameObject backgroundGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseGround>(), Tile.BackgroundsContainer);
        OverworldTileBaseGround baseBackground = backgroundGO.GetComponent<OverworldTileBaseGround>();

        baseBackground.SetTile(Tile);
        baseBackground.WithType(new OverworldDefaultGroundType());
        baseBackground.WithConnectionScoreInfo(new TileConnectionScoreInfo(16));
        Tile.AddBackground(baseBackground);

        UpdateGroundConnectionsOnNeighbours(new OverworldDefaultGroundType());
        return baseBackground;
    }

    public override U PlaceBackground<U>()
    {
        Logger.Log($"Place background of type {typeof(U)}");

        switch (typeof(U))
        {
            case Type overworldTileBaseGround when overworldTileBaseGround == typeof(OverworldTileBaseGround):
                return (U)PlaceCoveringBaseGround();
            case Type overworldTileBaseWater when overworldTileBaseWater == typeof(OverworldTileBaseWater):
                Tile.SetMainMaterial(new WaterMainMaterial());
                break;
            default:
                Logger.Error($"Unknown type {typeof(U)}");
                break;
        }

        U tileBackground = (U)PlaceWater<U>();
        return tileBackground;
    }

    private void UpdatePathConnectionsOnNeighbours()
    {
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in Tile.Neighbours)
        {
            if (!neighbour.Value) continue;
            
            TilePath tilePathOnNeighbour = neighbour.Value.TryGetTilePath();

            if (tilePathOnNeighbour == null) continue;

            int oldPathConnectionScoreOnNeighbour = tilePathOnNeighbour.ConnectionScore;
            Logger.Warning($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {Tile.GridLocation.X},{Tile.GridLocation.Y}");

            TileConnectionScoreInfo newPathConnectionScoreOnNeighbourInfo = NeighbourTileCalculator.MapNeighbourPathsOfTile(neighbour.Value, tilePathOnNeighbour.TilePathType);
            Logger.Log($"We calculated an tile connection type score of neighbour {newPathConnectionScoreOnNeighbourInfo.RawConnectionScore} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            tilePathOnNeighbour.WithConnectionScoreInfo(newPathConnectionScoreOnNeighbourInfo);

            if (neighbour.Value.TileMainMaterial?.GetType() == typeof(GroundMainMaterial)
                && oldPathConnectionScoreOnNeighbour == 16
                && newPathConnectionScoreOnNeighbourInfo.RawConnectionScore != 16)
            {
                TileBaseGround existingGroundTile = neighbour.Value.TryGetTileGround();

                if (existingGroundTile) continue;
                
                GameObject backgroundGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseGround>(), neighbour.Value.BackgroundsContainer);
                OverworldTileBaseGround baseBackground = backgroundGO.GetComponent<OverworldTileBaseGround>();

                baseBackground.SetTile(neighbour.Value);
                baseBackground.WithType(new OverworldDefaultGroundType());
                baseBackground.WithConnectionScoreInfo(new TileConnectionScoreInfo(16));
                neighbour.Value.AddBackground(baseBackground);
            }

            // If the path now covers the whole tile, remove any existing ground backgrounds
            if (oldPathConnectionScoreOnNeighbour != 16)
            {
                RemoveGroundBackgroundFromCoveredTile(neighbour.Value as EditorOverworldTile, newPathConnectionScoreOnNeighbourInfo.RawConnectionScore);
            }
        }
    }

    private void RemoveGroundBackgroundFromCoveredTile(EditorOverworldTile tile, int newPathConnectionScore)
    {
        if (newPathConnectionScore == 16)
        {
            OverworldTileBackgroundRemover backgroundRemover = new OverworldTileBackgroundRemover(tile);
            List<ITileBackground> backgrounds = tile.GetBackgrounds();
            for (int i = 0; i < backgrounds.Count; i++)
            {
                backgroundRemover.RemoveBackground<OverworldTileBaseGround>();
            }
        }
    }

    public void TryPlaceCornerFillersForNeighbours()
    {
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in Tile.Neighbours)
        {
            TryPlaceCornerFillers(neighbour.Value);
        }
    }

    public void TryPlaceCornerFillers(Tile tile)
    {
        if (tile == null) return;
        Logger.Warning("CheckForCornerFillers");
        TileBaseGround existingGround = tile.TryGetTileGround();

        if (!existingGround) return;
        Logger.Log($"existingGround.ConnectionScore if {tile.GridLocation.X}, {tile.GridLocation.Y} is {existingGround.ConnectionScore}");

        if (existingGround.ConnectionScore == 16)
        {
            var rightConnectionScore = tile.Neighbours[ObjectDirection.Right]?.TryGetTileGround()?.ConnectionScore;
            if (rightConnectionScore == 25)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }
            if (rightConnectionScore == 26)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
            }
            else if (rightConnectionScore == 16 || rightConnectionScore == 34)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
            }

            var downConnectionScore = tile.Neighbours[ObjectDirection.Down]?.TryGetTileGround()?.ConnectionScore;
            if (downConnectionScore == 23)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if(downConnectionScore == 26)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
            else if (downConnectionScore == 16 || downConnectionScore == 33)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }

            var leftConnectionScore = tile.Neighbours[ObjectDirection.Left]?.TryGetTileGround()?.ConnectionScore;
            if (leftConnectionScore == 21)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
            }
            if (leftConnectionScore == 23)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
            }
            if (leftConnectionScore == 16 || leftConnectionScore == 32)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
            }


            var upConnectionScore = tile.Neighbours[ObjectDirection.Up]?.TryGetTileGround()?.ConnectionScore;
            if (upConnectionScore == 21)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.RightDown);
            }
            else if(upConnectionScore == 25)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
            }
            else if (upConnectionScore == 16 || upConnectionScore == 31)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.RightDown);
            }
        }
        else if (existingGround.ConnectionScore == 17)
        {
            var rightConnectionScore = tile.Neighbours[ObjectDirection.Right]?.TryGetTileGround()?.ConnectionScore;
            if (rightConnectionScore == 16)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }
            else if (rightConnectionScore == 25)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }
            else if (rightConnectionScore == 31)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftDown);
            }
        }
        else if (existingGround.ConnectionScore == 18)
        {
            var downConnectionScore = tile.Neighbours[ObjectDirection.Down]?.TryGetTileGround()?.ConnectionScore;

            if (downConnectionScore == 16)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 20 || downConnectionScore == 23 || downConnectionScore == 26 || downConnectionScore == 24 || downConnectionScore == 30)
            {
                TryRemoveCornerFiller(tile, TileCorner.RightDown);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile, TileCorner.LeftDown);
            }
            else if (downConnectionScore == 33)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
        }
        else if (existingGround.ConnectionScore == 19)
        {
            var leftConnectionScore = tile.Neighbours[ObjectDirection.Left]?.TryGetTileGround()?.ConnectionScore;
            if(leftConnectionScore == 16)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.LeftUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightUp);
            }
        }
        else if (existingGround.ConnectionScore == 20)
        {
            var upConnectionScore = tile.Neighbours[ObjectDirection.Up]?.TryGetTileGround()?.ConnectionScore;
            if (upConnectionScore == 16)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.RightDown);
            }
            else if(upConnectionScore == 18)
            {
                TryAddCornerFiller(tile, TileCorner.LeftUp);
                TryAddCornerFiller(tile, TileCorner.RightUp);
            }
            else if (upConnectionScore == 21)
            {
                TryAddCornerFiller(tile, TileCorner.LeftUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.RightUp);
            }
            else if (upConnectionScore == 25)
            {
                TryAddCornerFiller(tile, TileCorner.RightUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
            }
            else if (upConnectionScore == 24 || upConnectionScore == 28)
            {
                TryAddCornerFiller(tile, TileCorner.RightUp);
                TryAddCornerFiller(tile, TileCorner.LeftUp);
            }
            else if (upConnectionScore == 31 || upConnectionScore == 34)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftDown);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.RightDown);
            }
        }
        else if (existingGround.ConnectionScore == 21)
        {
            var rightConnectionScore = tile.Neighbours[ObjectDirection.Right]?.TryGetTileGround()?.ConnectionScore;
            if(rightConnectionScore == 16)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
            }
            else if (rightConnectionScore == 19 || rightConnectionScore == 22 || rightConnectionScore == 29)
            {
                TryAddCornerFiller(tile, TileCorner.RightDown);
            }

            var downConnectionScore = tile.Neighbours[ObjectDirection.Down]?.TryGetTileGround()?.ConnectionScore;
            if (downConnectionScore == 23)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
        }
        else if (existingGround.ConnectionScore == 22)
        {
            var leftConnectionScore = tile.Neighbours[ObjectDirection.Left]?.TryGetTileGround()?.ConnectionScore;
            if (leftConnectionScore == 21)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
                var leftDownConnectionScore = tile.Neighbours[ObjectDirection.Left]?.Neighbours[ObjectDirection.Down]?.TryGetTileGround()?.ConnectionScore;
                if(leftDownConnectionScore == 24 || leftDownConnectionScore == 26)
                {
                    TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Left].Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                }
            }
            else if (leftConnectionScore == 31)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Left], TileCorner.RightDown);
            }
        }
        else if (existingGround.ConnectionScore == 23)
        {
            var rightConnectionScore = tile.Neighbours[ObjectDirection.Right]?.TryGetTileGround()?.ConnectionScore;
            if (rightConnectionScore == 33)
            {
                TryRemoveCornerFiller(tile, TileCorner.LeftUp);
            }

            var upConnectionScore = tile.Neighbours[ObjectDirection.Up]?.TryGetTileGround()?.ConnectionScore;
            if (upConnectionScore == 18 || upConnectionScore == 24 || upConnectionScore == 28)
            {
                TryAddCornerFiller(tile, TileCorner.RightUp);
                TryRemoveCornerFiller(tile, TileCorner.RightDown);
                TryRemoveCornerFiller(tile, TileCorner.LeftDown);
                TryAddCornerFiller(tile, TileCorner.LeftUp);
            }
            else if (upConnectionScore == 21)
            {
                TryAddCornerFiller(tile, TileCorner.LeftUp);
            }
        }
        else if (existingGround.ConnectionScore == 24)
        {
            var downConnectionScore = tile.Neighbours[ObjectDirection.Down]?.TryGetTileGround()?.ConnectionScore;
            if (downConnectionScore == 16)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 23)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 26)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }

            var upConnectionScore = tile.Neighbours[ObjectDirection.Up]?.TryGetTileGround()?.ConnectionScore;
            if (upConnectionScore == 18)
            {
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.LeftUp);
                TryRemoveCornerFiller(tile.Neighbours[ObjectDirection.Up], TileCorner.RightUp);
            }
            else if (upConnectionScore == 21)
            {
                TryRemoveCornerFiller(tile, TileCorner.RightUp);
                TryAddCornerFiller(tile, TileCorner.LeftUp);
            }
            else if (upConnectionScore == 24)
            {
                TryAddCornerFiller(tile, TileCorner.LeftUp);
                TryAddCornerFiller(tile, TileCorner.RightUp);
            }
            else if (upConnectionScore == 25)
            {
                TryAddCornerFiller(tile, TileCorner.RightUp);
                TryRemoveCornerFiller(tile, TileCorner.LeftUp);
            }
        }
        else if (existingGround.ConnectionScore == 25)
        {
            var downConnectionScore = tile.Neighbours[ObjectDirection.Down]?.TryGetTileGround()?.ConnectionScore;
            if (downConnectionScore == 23)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
                TryAddCornerFiller(tile, TileCorner.LeftDown);
            }
            else if (downConnectionScore == 24)
            {
                TryAddCornerFiller(tile, TileCorner.LeftDown);
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
            else if (downConnectionScore == 26)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
        }
        else if (existingGround.ConnectionScore == 26)
        {
            var leftConnectionScore = tile.Neighbours[ObjectDirection.Left]?.TryGetTileGround()?.ConnectionScore;
            if (leftConnectionScore == 17 || leftConnectionScore == 21 || leftConnectionScore == 22 || leftConnectionScore == 27)
            {
                var upConnectionScore = tile.Neighbours[ObjectDirection.Up]?.TryGetTileGround()?.ConnectionScore;
                if (upConnectionScore == 18 || upConnectionScore == 24 || upConnectionScore == 28)
                {
                    TryAddCornerFiller(tile, TileCorner.RightUp);
                    TryRemoveCornerFiller(tile, TileCorner.RightDown);
                    TryRemoveCornerFiller(tile, TileCorner.LeftDown);
                    TryAddCornerFiller(tile, TileCorner.LeftUp);
                }
                else if (upConnectionScore == 21)
                {
                    TryAddCornerFiller(tile, TileCorner.LeftUp);
                    TryRemoveCornerFiller(tile, TileCorner.RightUp);
                }
                else if (upConnectionScore == 25)
                {
                    TryAddCornerFiller(tile, TileCorner.RightUp);
                    TryRemoveCornerFiller(tile, TileCorner.LeftUp);
                }
            }
        }
        else if (existingGround.ConnectionScore == 31)
        {
            var rightConnectionScore = tile.Neighbours[ObjectDirection.Right]?.TryGetTileGround()?.ConnectionScore;
            if (rightConnectionScore == 34)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Right], TileCorner.LeftUp);
            }
        }
        else if (existingGround.ConnectionScore == 32)
        {
            var downConnectionScore = tile.Neighbours[ObjectDirection.Down]?.TryGetTileGround()?.ConnectionScore;
            if (downConnectionScore == 20)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.LeftUp);
            }
        }
        else if (existingGround.ConnectionScore == 33)
        {
            var upConnectionScore = tile.Neighbours[ObjectDirection.Up]?.TryGetTileGround()?.ConnectionScore;
            if (upConnectionScore == 21)
            {
                TryAddCornerFiller(tile, TileCorner.LeftUp);
            }
        }
        else if (existingGround.ConnectionScore == 34)
        {
            var downConnectionScore = tile.Neighbours[ObjectDirection.Down]?.TryGetTileGround()?.ConnectionScore;
            if (downConnectionScore == 20)
            {
                TryAddCornerFiller(tile.Neighbours[ObjectDirection.Down], TileCorner.RightUp);
            }
        }
    }

    public void TryAddCornerFiller(Tile tile, TileCorner tileCorner)
    {
        Logger.Warning($"TryAddCornerFiller on {tile.GridLocation.X}, {tile.GridLocation.Y} in the corner {tileCorner}");
        //Check if there is already a tilecorner
        if (tile.TryGetCornerFiller(tileCorner))
        {
            return;
        }

        //create cornerfiller
        GameObject backgroundGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<TileCornerFiller>(), tile.BackgroundsContainer);
        TileCornerFiller cornerFiller = backgroundGO.GetComponent<TileCornerFiller>();

        cornerFiller.SetTile(tile);
        cornerFiller.WithType(new OverworldDefaultGroundType());
        cornerFiller.WithCorner(tileCorner); // pick sprite based on corner

        tile.AddCornerFiller(cornerFiller);
    }

    public void TryRemoveCornerFiller(Tile tile, TileCorner tileCorner)
    {
        tile.TryRemoveCornerFiller(tileCorner);
    }

    public void UpdateGroundConnectionsOnNeighbours(IBaseBackgroundType groundType)
    {
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in Tile.Neighbours)
        {
            EditorOverworldTile neighbourTile = neighbour.Value as EditorOverworldTile;

            if (!neighbourTile) continue;

            if (neighbourTile.TileMainMaterial?.GetType() == typeof(GroundMainMaterial)) continue;

            TileBaseGround existingGround = neighbourTile.TryGetTileGround();

            TileConnectionScoreInfo newGroundConnectionScoreOnNeighbourInfo = NeighbourTileCalculator.MapGroundConnectionsWithNeighbours(neighbourTile, groundType);

            if (newGroundConnectionScoreOnNeighbourInfo.RawConnectionScore == -1)
            {
                RemoveGroundBackgroundFromCoveredTile(neighbourTile, 16);
            }
            else if (existingGround == null)
            {
                //There are some connections and no ground sprite. Add ground sprite.
                GameObject backgroundGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseGround>(), neighbourTile.BackgroundsContainer);
                OverworldTileBaseGround baseBackground = backgroundGO.GetComponent<OverworldTileBaseGround>();

                baseBackground.SetTile(Tile);
                baseBackground.WithType(new OverworldDefaultGroundType());
                baseBackground.WithConnectionScoreInfo(newGroundConnectionScoreOnNeighbourInfo);
                neighbourTile.AddBackground(baseBackground);
                //Logger.Warning("This is where we need to update the neighbours of the neighbours watercornerends WHEN THERE IS NO GROUND YET");
            }
            else
            {
                //Logger.Warning($"This is where we need to update the neighbours of the neighbours watercornerends. newGroundConnectionScoreOnNeighbourInfo ${newGroundConnectionScoreOnNeighbourInfo.RawConnectionScore} at {neighbourTile.GridLocation.X}, {neighbourTile.GridLocation.Y}");
                existingGround.WithConnectionScoreInfo(newGroundConnectionScoreOnNeighbourInfo);

                //if(newGroundConnectionScoreOnNeighbourInfo.RawConnectionScore == 22)
                //{
                //    if(neighbour.Key == ObjectDirection.Right) 
                //    { 
                //        Tile.TryAddCornerFiller(TileCorner.LeftUp);
                //    }
                //    else if(neighbour.Key == ObjectDirection.Left)
                //    {
                //        Tile.TryAddCornerFiller(TileCorner.RightUp);
                //    }
                //}
            }
        }
    }

    
}
