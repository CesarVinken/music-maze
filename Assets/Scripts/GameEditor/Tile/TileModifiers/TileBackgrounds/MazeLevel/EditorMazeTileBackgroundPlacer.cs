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
        Logger.Log($"Found a score of {pathConnectionScore.RawConnectionScore}");
        GameObject mazeTilePathGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<MazeTilePath>(), Tile.BackgroundsContainer);
        MazeTilePath mazeTilePath = mazeTilePathGO.GetComponent<MazeTilePath>();
        mazeTilePath.WithPathType(mazeTilePathType);
        mazeTilePath.WithConnectionScoreInfo(pathConnectionScore);
        Tile.AddBackground(mazeTilePath as ITileBackground);
        Tile.TryMakeMarkable(true);

        // Update pathConnections for neighbouring tiles
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in Tile.Neighbours)
        {
            TilePath tilePathOnNeighbour = neighbour.Value.TryGetTilePath();

            if (tilePathOnNeighbour == null) continue;
            int oldConnectionScoreOnNeighbour = tilePathOnNeighbour.ConnectionScore;
            Logger.Warning($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {Tile.GridLocation.X},{Tile.GridLocation.Y}");

            TileConnectionScoreInfo mazeTilePathConnectionScoreOnNeighbourInfo = NeighbourTileCalculator.MapNeighbourPathsOfTile(neighbour.Value, mazeTilePathType);
            Logger.Log($"We calculated an maze connection type score of neighbour {mazeTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            tilePathOnNeighbour.WithConnectionScoreInfo(mazeTilePathConnectionScoreOnNeighbourInfo);

            if (neighbour.Value.TileMainMaterial?.GetType() == typeof(GroundMainMaterial) && oldConnectionScoreOnNeighbour == 16 && mazeTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore != 16)
            {
                PlaceBackground<MazeTileBaseGround>();
            }
        }

        Tile.RemoveBeautificationTriggerers();
    }

    public void PlaceWater(IBaseBackgroundType waterType)
    {
        TileConnectionScoreInfo waterConnectionScore = NeighbourTileCalculator.MapNeighbourWaterOfTile(Tile, waterType);
 
        GameObject waterGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<MazeTileBaseWater>(), Tile.BackgroundsContainer);
        MazeTileBaseWater mazeTileBaseWater = waterGO.GetComponent<MazeTileBaseWater>();
        mazeTileBaseWater.WithWaterType(waterType);
        mazeTileBaseWater.WithConnectionScoreInfo(waterConnectionScore);
        mazeTileBaseWater.SetTile(Tile);

        Tile.SetMainMaterial(new WaterMainMaterial());
        Tile.AddBackground(mazeTileBaseWater);
        Tile.TryMakeMarkable(false);

        // Update pathConnections for neighbouring tiles
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in Tile.Neighbours)
        {
            TilePath tilePathOnNeighbour = neighbour.Value.TryGetTilePath();

            if (tilePathOnNeighbour == null) continue;
            int oldConnectionScoreOnNeighbour = tilePathOnNeighbour.ConnectionScore;
            Logger.Warning($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {Tile.GridLocation.X},{Tile.GridLocation.Y}");

            TileConnectionScoreInfo mazeTilePathConnectionScoreOnNeighbourInfo = NeighbourTileCalculator.MapNeighbourPathsOfTile(neighbour.Value, tilePathOnNeighbour.TilePathType);
            Logger.Log($"We calculated an maze connection type score of neighbour {mazeTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            tilePathOnNeighbour.WithConnectionScoreInfo(mazeTilePathConnectionScoreOnNeighbourInfo);

            if (neighbour.Value.TileMainMaterial?.GetType() == typeof(GroundMainMaterial) && oldConnectionScoreOnNeighbour == 16 && mazeTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore != 16)
            {
                PlaceBackground<MazeTileBaseGround>();
            }
        }

        // Update water connections on neighbours
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in Tile.Neighbours)
        {
            TileWater waterOnNeighbour = neighbour.Value.TryGetTileWater();

            if (waterOnNeighbour == null) continue;
            int oldConnectionScoreOnNeighbour = waterOnNeighbour.ConnectionScore;
            Logger.Warning($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {Tile.GridLocation.X},{Tile.GridLocation.Y}");

            TileConnectionScoreInfo mazeTileWaterConnectionScoreOnNeighbourInfo = NeighbourTileCalculator.MapNeighbourWaterOfTile(neighbour.Value, waterType);
            Logger.Log($"We calculated an maze connection type score of neighbour {mazeTileWaterConnectionScoreOnNeighbourInfo.RawConnectionScore} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            waterOnNeighbour.WithConnectionScoreInfo(mazeTileWaterConnectionScoreOnNeighbourInfo);

            if (neighbour.Value.TileMainMaterial?.GetType() == typeof(WaterMainMaterial) && oldConnectionScoreOnNeighbour == 16 && mazeTileWaterConnectionScoreOnNeighbourInfo.RawConnectionScore != 16)
            {
                PlaceBackground<MazeTileBaseWater>();
            }
        }
        Logger.Log("todo, update neighbours for water and paths");



        Tile.RemoveBeautificationTriggerers();
    }

    public override void PlaceBackground<U>()
    {
        Logger.Log($"Place background of type {typeof(U)}");

        switch (typeof(U))
        {
            case Type mazeTileBaseGround when mazeTileBaseGround == typeof(MazeTileBaseGround):
                //case Type mazeTilePath when mazeTilePath == typeof(MazeTilePath):
                Logger.Warning("Set to ground main material");
                Tile.SetMainMaterial(new GroundMainMaterial());
                Logger.Warning($"it is now {Tile.TileMainMaterial}");
                break;
            case Type mazeTileBaseWater when mazeTileBaseWater == typeof(MazeTileBaseWater):
                PlaceWater(new MazeLevelDefaultWaterType());
                return;
            default:
                Logger.Error($"Unknown type {typeof(U)}");
                break;
        }

        U oldBackground = (U)Tile.GetBackgrounds().FirstOrDefault(background => background is U);
        if (oldBackground != null) return;

        GameObject backgroundGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<U>(), Tile.BackgroundsContainer);
        U baseBackground = backgroundGO.GetComponent<U>();

        baseBackground.SetTile(Tile);
        Tile.AddBackground(baseBackground);
    }
}
