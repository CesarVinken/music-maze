using System;
using System.Collections.Generic;
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

        GameObject overworldTilePathGO = GameObject.Instantiate(OverworldGameplayManager.Instance.GetTileBackgroundPrefab<OverworldTilePath>(), Tile.BackgroundsContainer);
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
            GameObject backgroundGO = GameObject.Instantiate(OverworldGameplayManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseGround>(), Tile.BackgroundsContainer);
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

        GameObject waterGO = GameObject.Instantiate(OverworldGameplayManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseWater>(), Tile.BackgroundsContainer);
        OverworldTileBaseWater overworldTileBaseWater = waterGO.GetComponent<OverworldTileBaseWater>();
        overworldTileBaseWater.SetTile(Tile);

        Tile.SetMainMaterial(new WaterMainMaterial());
        Tile.AddBackground(overworldTileBaseWater);
        //Tile.SetWalkable(false);

        // Update pathConnections for neighbouring tiles
        UpdatePathConnectionsOnNeighbours();
        UpdateGroundConnectionsOnNeighbours(new OverworldDefaultGroundType());

        // Place corner fillers
        TileCornerFillerRegister.TryPlaceCornerFillers(Tile);
        TileCornerFillerRegister.TryPlaceCornerFillersForNeighbours(Tile);

        return overworldTileBaseWater;
    }

    public void PlaceCoveringBaseWater()
    {
        Tile.SetMainMaterial(new WaterMainMaterial());
        Logger.Log("Place a water tile without updating neighbours or removing land tiles.");

        GameObject waterGO = GameObject.Instantiate(OverworldGameplayManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseWater>(), Tile.BackgroundsContainer);
        OverworldTileBaseWater mazeTileBaseWater = waterGO.GetComponent<OverworldTileBaseWater>();
        mazeTileBaseWater.SetTile(Tile);

        Tile.AddBackground(mazeTileBaseWater);
    }

    public ITileBackground PlaceCoveringBaseGround()
    {
        Tile.SetMainMaterial(new GroundMainMaterial());

        Logger.Log("Place a base ground tile will connections on all sides.");
        GameObject backgroundGO = GameObject.Instantiate(OverworldGameplayManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseGround>(), Tile.BackgroundsContainer);
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
                
                GameObject backgroundGO = GameObject.Instantiate(OverworldGameplayManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseGround>(), neighbour.Value.BackgroundsContainer);
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

    public void UpdateGroundConnectionsOnNeighbours(IBaseBackgroundType groundType)
    {
        Logger.Log($"UpdateGroundConnectionsOnNeighbours with ground type {groundType}");
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
                GameObject backgroundGO = GameObject.Instantiate(OverworldGameplayManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseGround>(), neighbourTile.BackgroundsContainer);
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
            }
        }
    }

    
}
