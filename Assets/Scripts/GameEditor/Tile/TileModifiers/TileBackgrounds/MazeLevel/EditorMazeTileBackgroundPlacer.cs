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
        mazeTilePath.WithPathType(mazeTilePathType);
        mazeTilePath.WithConnectionScoreInfo(pathConnectionScore);
        Tile.AddBackground(mazeTilePath as ITileBackground);
        Tile.TryMakeMarkable(true);

        // Update pathConnections for neighbouring tiles
        UpdatePathConnectionsOnNeighbours();

        Tile.RemoveBeautificationTriggerers();
    }

    public ITileBackground PlaceWater(IBaseBackgroundType waterType)
    {
        TileConnectionScoreInfo waterConnectionScore = NeighbourTileCalculator.MapNeighbourWaterOfTile(Tile, waterType);

        // if the tile will not completely be covered with water, make sure we have a land background as well.
        if(waterConnectionScore.RawConnectionScore != 16)
        {
            List<ITileBackground> backgrounds = Tile.GetBackgrounds();
            if (backgrounds.Count == 0)
            {
                GameObject backgroundGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<MazeTileBaseGround>(), Tile.BackgroundsContainer);
                MazeTileBaseGround baseBackground = backgroundGO.GetComponent<MazeTileBaseGround>();
                baseBackground.SetTile(Tile);
                Tile.AddBackground(baseBackground);
            }
        }
 
        GameObject waterGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<MazeTileBaseWater>(), Tile.BackgroundsContainer);
        MazeTileBaseWater mazeTileBaseWater = waterGO.GetComponent<MazeTileBaseWater>();
        mazeTileBaseWater.WithWaterType(waterType);
        mazeTileBaseWater.WithConnectionScoreInfo(waterConnectionScore);
        mazeTileBaseWater.SetTile(Tile);

        Tile.SetMainMaterial(new WaterMainMaterial());
        Tile.AddBackground(mazeTileBaseWater);
        Tile.TryMakeMarkable(false);

        // Update pathConnections for neighbouring tiles
        UpdatePathConnectionsOnNeighbours();

        // Update water connections on neighbours
        UpdateWaterConnectionsOnNeighbours(waterType);

        Tile.RemoveBeautificationTriggerers();

        return mazeTileBaseWater;
    }

    public U PlaceLand<U>() where U : ITileBackground
    {
        U oldBackground = (U)Tile.GetBackgrounds().FirstOrDefault(background => background is U);
        if (oldBackground != null)
        {
            UpdateWaterConnectionsOnNeighbours(new MazeLevelDefaultWaterType());
            return oldBackground;
        }

        GameObject backgroundGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<U>(), Tile.BackgroundsContainer);
        U baseBackground = backgroundGO.GetComponent<U>();

        // Update water connections on neighbours, because placing somewhere affects coastlines
        UpdateWaterConnectionsOnNeighbours(new MazeLevelDefaultWaterType());
        baseBackground.SetTile(Tile);
        Tile.AddBackground(baseBackground);
        return baseBackground;
    }

    public override U PlaceBackground<U>()
    {
        Logger.Log($"Place background of type {typeof(U)}");

        switch (typeof(U))
        {
            case Type mazeTileBaseGround when mazeTileBaseGround == typeof(MazeTileBaseGround):
                Logger.Warning("Set to ground main material");
                Tile.SetMainMaterial(new GroundMainMaterial());
                Logger.Warning($"it is now {Tile.TileMainMaterial}");
                break;
            case Type mazeTileBaseWater when mazeTileBaseWater == typeof(MazeTileBaseWater):
                return (U)PlaceWater(new MazeLevelDefaultWaterType());
            default:
                Logger.Error($"Unknown type {typeof(U)}");
                break;
        }

        U tileBackground = PlaceLand<U>();
        return tileBackground;
    }

    private void UpdatePathConnectionsOnNeighbours()
    {
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in Tile.Neighbours)
        {
            if (!neighbour.Value) continue;
            
            TilePath tilePathOnNeighbour = neighbour.Value.TryGetTilePath();

            if (tilePathOnNeighbour == null) continue;
            int oldConnectionScoreOnNeighbour = tilePathOnNeighbour.ConnectionScore;
            Logger.Warning($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {Tile.GridLocation.X},{Tile.GridLocation.Y}");

            TileConnectionScoreInfo mazeTilePathConnectionScoreOnNeighbourInfo = NeighbourTileCalculator.MapNeighbourPathsOfTile(neighbour.Value, tilePathOnNeighbour.TilePathType);
            Logger.Log($"We calculated an maze connection type score of neighbour {mazeTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}. The old score was {oldConnectionScoreOnNeighbour} with a main material of {neighbour.Value.TileMainMaterial?.GetType()}");

            //update connection score on neighbour
            tilePathOnNeighbour.WithConnectionScoreInfo(mazeTilePathConnectionScoreOnNeighbourInfo);

            if (neighbour.Value.TileMainMaterial == null || neighbour.Value.TileMainMaterial.GetType() == typeof(GroundMainMaterial)
                && oldConnectionScoreOnNeighbour == 16
                && mazeTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore != 16)
            {
                Logger.Warning($"oldConnectionScoreOnNeighbour {oldConnectionScoreOnNeighbour}");

                GameObject backgroundGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<MazeTileBaseGround>(), neighbour.Value.BackgroundsContainer);
                MazeTileBaseGround baseBackground = backgroundGO.GetComponent<MazeTileBaseGround>();
                baseBackground.SetTile(neighbour.Value);
                neighbour.Value.AddBackground(baseBackground);
            }
        }
    }

    public void UpdateWaterConnectionsOnNeighbours(IBaseBackgroundType waterType)
    {
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in Tile.Neighbours)
        {
            if (!neighbour.Value) continue;
            
            TileWater waterOnNeighbour = neighbour.Value.TryGetTileWater();

            if (waterOnNeighbour == null) continue;
            int oldConnectionScoreOnNeighbour = waterOnNeighbour.ConnectionScore;
            Logger.Warning($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {Tile.GridLocation.X},{Tile.GridLocation.Y}");

            TileConnectionScoreInfo mazeTileWaterConnectionScoreOnNeighbourInfo = NeighbourTileCalculator.MapNeighbourWaterOfTile(neighbour.Value, waterType);
            Logger.Log($"We calculated an maze connection type score of neighbour {mazeTileWaterConnectionScoreOnNeighbourInfo.RawConnectionScore} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            waterOnNeighbour.WithConnectionScoreInfo(mazeTileWaterConnectionScoreOnNeighbourInfo);

            if(waterOnNeighbour.ConnectionScore == 16) // The water score is no 16, we need to remove the existing ground sprit
            {
                MazeTileBackgroundRemover tileBackgroundRemover = new MazeTileBackgroundRemover(neighbour.Value as EditorMazeTile);

                List<ITileBackground> backgroundsOnNeighbour = neighbour.Value.GetBackgrounds();
                for (int i = 0; i < backgroundsOnNeighbour.Count; i++)
                {
                    switch (backgroundsOnNeighbour[i].GetType())
                    {
                        case Type mazeTileBaseGround when mazeTileBaseGround == typeof(MazeTileBaseGround):
                            tileBackgroundRemover.RemoveBackground<MazeTileBaseGround>();
                            break;
                        case Type mazeTileBaseWater when mazeTileBaseWater == typeof(MazeTileBaseWater):
                            break;
                        default:
                            Logger.Error($"Unknown type {backgroundsOnNeighbour[i].GetType()}");
                            break;
                    }
                }
            }

            if (neighbour.Value.TileMainMaterial?.GetType() == typeof(WaterMainMaterial)
                && oldConnectionScoreOnNeighbour == 16
                && mazeTileWaterConnectionScoreOnNeighbourInfo.RawConnectionScore != 16)
            {
                EditorMazeTileBackgroundPlacer tileBackgroundPlacerForNeighbour = new EditorMazeTileBackgroundPlacer(neighbour.Value as EditorMazeTile);
                tileBackgroundPlacerForNeighbour.PlaceLand<MazeTileBaseGround>();
            }
        }
    }
}
