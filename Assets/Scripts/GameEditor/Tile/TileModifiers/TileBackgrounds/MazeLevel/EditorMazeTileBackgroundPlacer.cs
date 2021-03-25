using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorMazeTileBackgroundPlacer : MazeTileBackgroundPlacer<EditorMazeTile>
{
    private EditorMazeTile _tile;

    public override EditorMazeTile Tile { get => _tile; set => _tile = value; }

    public EditorMazeTileBackgroundPlacer(EditorMazeTile tile)
    {
        Tile = tile;
    }

    // Called in the editor, when we need to update the connection score
    public void PlacePath(IPathType mazeTilePathType)
    {
        Logger.Warning("Start placing path.....");

        TileConnectionScoreInfo pathConnectionScore = NeighbourTileCalculator.MapNeighbourPathsOfTile(Tile, mazeTilePathType);

        GameObject mazeTilePathGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<MazeTilePath>(), Tile.BackgroundsContainer);
        MazeTilePath mazeTilePath = mazeTilePathGO.GetComponent<MazeTilePath>();
        mazeTilePath.WithType(mazeTilePathType as IBackgroundType);
        mazeTilePath.WithConnectionScoreInfo(pathConnectionScore);

        Tile.AddBackground(mazeTilePath as ITileBackground);
        Tile.TryMakeMarkable(true);

        // Update pathConnections for neighbouring tiles
        UpdatePathConnectionsOnNeighbours();

        RemoveGroundBackgroundFromCoveredTile(_tile as EditorMazeTile, pathConnectionScore.RawConnectionScore);

        Tile.RemoveBeautificationTriggerers();
    }

    public ITileBackground PlaceWater<U>() where U : ITileBackground
    {
        TileBaseGround existingGround = Tile.TryGetTileGround();
        int oldLandConnectionScore = existingGround? existingGround.ConnectionScore : -1;

        TileConnectionScoreInfo newLandConnectionScoreInfo = NeighbourTileCalculator.MapGroundConnectionsWithNeighbours(Tile, new MazeLevelDefaultGroundType());
        int newLandConnectionScore = newLandConnectionScoreInfo.RawConnectionScore;
        Logger.Log($"Old land connections score for tile {Tile.GridLocation.X},{Tile.GridLocation.Y} was {oldLandConnectionScore}. The New Score is {newLandConnectionScore}");

        // If there are no land connections left (value -1), remove the ground sprite
        if (newLandConnectionScore == -1)
        {
            Tile.RemoveBeautificationTriggerers();
            RemoveGroundBackgroundFromCoveredTile(Tile, 16);
        }
        else if(existingGround == null)
        {
            //There are some connections and no ground sprite. Add ground sprite.
            GameObject backgroundGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<MazeTileBaseGround>(), Tile.BackgroundsContainer);
            MazeTileBaseGround baseBackground = backgroundGO.GetComponent<MazeTileBaseGround>();

            baseBackground.SetTile(Tile);
            baseBackground.WithType(new MazeLevelDefaultGroundType());
            baseBackground.WithConnectionScoreInfo(newLandConnectionScoreInfo);
            Tile.AddBackground(baseBackground);
        } 
        else
        {
            existingGround.WithConnectionScoreInfo(newLandConnectionScoreInfo);
        }

        GameObject waterGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<MazeTileBaseWater>(), Tile.BackgroundsContainer);
        MazeTileBaseWater mazeTileBaseWater = waterGO.GetComponent<MazeTileBaseWater>();
        mazeTileBaseWater.SetTile(Tile);

        Tile.AddBackground(mazeTileBaseWater);
        Tile.TryMakeMarkable(false);
        Tile.Walkable = false;

        // Update Connections for neighbouring tiles
        UpdatePathConnectionsOnNeighbours();
        UpdateGroundConnectionsOnNeighbours(new MazeLevelDefaultGroundType());
        //Tile.RemoveBeautificationTriggerers();

        return mazeTileBaseWater;
    }

    public ITileBackground PlaceGround<U>(IBaseBackgroundType groundType) where U : ITileBackground, ITileConnectable
    {
        TileConnectionScoreInfo groundConnectionScore = NeighbourTileCalculator.MapGroundConnectionsWithNeighbours(Tile, new MazeLevelDefaultGroundType());
        
        Logger.Log($"Place ground. Connection Score: {groundConnectionScore.RawConnectionScore}");
        U oldBackground = (U)Tile.GetBackgrounds().FirstOrDefault(background => background is U);
        if (oldBackground != null)
        {
            //UpdateWaterConnectionsOnNeighbours(new MazeLevelDefaultWaterType());
            return oldBackground;
        }

        GameObject backgroundGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<U>(), Tile.BackgroundsContainer);
        U baseBackground = backgroundGO.GetComponent<U>();

        // Update land connections on neighbours, because placing somewhere affects coastlines
        UpdateGroundConnectionsOnNeighbours(groundType);

        baseBackground.SetTile(Tile);
        baseBackground.WithType(groundType);
        baseBackground.WithConnectionScoreInfo(groundConnectionScore);

        Tile.AddBackground(baseBackground);
        //Tile.RemoveBeautificationTriggerers();

        return baseBackground;
    }

    public void PlaceCoveringBaseWater()
    {
        Tile.SetMainMaterial(new WaterMainMaterial());
        Logger.Log("Place a water tile without updating neighbours or removing land tiles.");

        GameObject waterGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<MazeTileBaseWater>(), Tile.BackgroundsContainer);
        MazeTileBaseWater mazeTileBaseWater = waterGO.GetComponent<MazeTileBaseWater>();
        mazeTileBaseWater.SetTile(Tile);

        Tile.AddBackground(mazeTileBaseWater);
        Tile.TryMakeMarkable(false);
    }

    public ITileBackground PlaceCoveringBaseGround()
    {
        Tile.SetMainMaterial(new GroundMainMaterial());

        Logger.Log("Place a base ground tile will connections on all sides.");
        GameObject backgroundGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<MazeTileBaseGround>(), Tile.BackgroundsContainer);
        MazeTileBaseGround baseBackground = backgroundGO.GetComponent<MazeTileBaseGround>();

        baseBackground.SetTile(Tile);
        baseBackground.WithType(new MazeLevelDefaultGroundType());
        baseBackground.WithConnectionScoreInfo(new TileConnectionScoreInfo(16));
        Tile.AddBackground(baseBackground);
        //Tile.RemoveBeautificationTriggerers();

        UpdateGroundConnectionsOnNeighbours(new MazeLevelDefaultGroundType());

        return baseBackground;
    }

    public override U PlaceBackground<U>()
    {
        Logger.Log($"Place background of type {typeof(U)}");

        switch (typeof(U))
        {
            case Type mazeTileBaseGround when mazeTileBaseGround == typeof(MazeTileBaseGround):
                Logger.Warning($"Set {Tile.GridLocation.X},{Tile.GridLocation.Y} to ground main material");
                return (U)PlaceCoveringBaseGround();
            case Type mazeTileBaseWater when mazeTileBaseWater == typeof(MazeTileBaseWater):
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
            Logger.Log($"We calculated an maze connection type score of neighbour {newPathConnectionScoreOnNeighbourInfo.RawConnectionScore} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}. The old score was {oldPathConnectionScoreOnNeighbour} with a main material of '{neighbour.Value.TileMainMaterial?.GetType()}'");

            //update connection score on neighbour
            tilePathOnNeighbour.WithConnectionScoreInfo(newPathConnectionScoreOnNeighbourInfo);

            // It is possible that the new path sprite reveals that there is no background underneath it. In that case, add background
            if (neighbour.Value.TileMainMaterial?.GetType() == typeof(GroundMainMaterial)
                && oldPathConnectionScoreOnNeighbour == 16
                && newPathConnectionScoreOnNeighbourInfo.RawConnectionScore != 16)
            {
                TileBaseGround existingGroundTile = neighbour.Value.TryGetTileGround();

                if (existingGroundTile) continue;

                GameObject backgroundGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<MazeTileBaseGround>(), neighbour.Value.BackgroundsContainer);
                MazeTileBaseGround baseGround = backgroundGO.GetComponent<MazeTileBaseGround>();
               
                baseGround.SetTile(neighbour.Value);
                baseGround.WithType(new MazeLevelDefaultGroundType());
                baseGround.WithConnectionScoreInfo(new TileConnectionScoreInfo(16));
                neighbour.Value.AddBackground(baseGround);
            }

            // If the path now covers the whole tile, remove any existing ground backgrounds
            if(oldPathConnectionScoreOnNeighbour != 16)
            {
                RemoveGroundBackgroundFromCoveredTile(neighbour.Value as EditorMazeTile, newPathConnectionScoreOnNeighbourInfo.RawConnectionScore);
            }
        }
    }

    private void RemoveGroundBackgroundFromCoveredTile(EditorMazeTile tile, int newPathConnectionScore)
    {
        if (newPathConnectionScore == 16)
        {
            MazeTileBackgroundRemover backgroundRemover = new MazeTileBackgroundRemover(tile);
            List<ITileBackground> backgrounds = tile.GetBackgrounds();
            for (int i = 0; i < backgrounds.Count; i++)
            {
                backgroundRemover.RemoveBackground<MazeTileBaseGround>();
            }
        }
    }

    public void UpdateGroundConnectionsOnNeighbours(IBaseBackgroundType groundType)
    {
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in Tile.Neighbours)
        {
            EditorMazeTile neighbourTile = neighbour.Value as EditorMazeTile;

            if (!neighbourTile) continue;

            if (neighbourTile.TileMainMaterial?.GetType() == typeof(GroundMainMaterial)) continue;

            TileBaseGround existingGround = neighbourTile.TryGetTileGround();

            TileConnectionScoreInfo newGroundConnectionScoreOnNeighbourInfo = NeighbourTileCalculator.MapGroundConnectionsWithNeighbours(neighbourTile, groundType);

            if (newGroundConnectionScoreOnNeighbourInfo.RawConnectionScore == -1)
            {
                neighbourTile.RemoveBeautificationTriggerers();
                RemoveGroundBackgroundFromCoveredTile(neighbourTile, 16);
            }
            else if (existingGround == null)
            {
                //There are some connections and no ground sprite. Add ground sprite.
                GameObject backgroundGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<MazeTileBaseGround>(), neighbourTile.BackgroundsContainer);
                MazeTileBaseGround baseBackground = backgroundGO.GetComponent<MazeTileBaseGround>();

                baseBackground.SetTile(Tile);
                baseBackground.WithType(new MazeLevelDefaultGroundType());
                baseBackground.WithConnectionScoreInfo(newGroundConnectionScoreOnNeighbourInfo);
                neighbourTile.AddBackground(baseBackground);
            }
            else
            {
                existingGround.WithConnectionScoreInfo(newGroundConnectionScoreOnNeighbourInfo);
            }
        }
    }
}
