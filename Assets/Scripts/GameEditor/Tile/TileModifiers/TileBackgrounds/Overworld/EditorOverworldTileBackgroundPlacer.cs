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
        overworldTilePath.WithPathType(overworldTilePathType);
        overworldTilePath.WithConnectionScoreInfo(pathConnectionScore);
        Tile.AddBackground(overworldTilePath as ITileBackground);

        // Update pathConnections for neighbouring tiles
        UpdatePathConnectionsOnNeighbours();
    }

    public ITileBackground PlaceWater(IBaseBackgroundType waterType)
    {
        TileConnectionScoreInfo waterConnectionScore = NeighbourTileCalculator.MapNeighbourWaterOfTile(Tile, waterType);

        // if the tile will not completely be covered with water, make sure we have a land background as well.
        if (waterConnectionScore.RawConnectionScore != 16)
        {
            List<ITileBackground> backgrounds = Tile.GetBackgrounds();
            if(backgrounds.Count == 0)
            {
                GameObject backgroundGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseGround>(), Tile.BackgroundsContainer);
                OverworldTileBaseGround baseBackground = backgroundGO.GetComponent<OverworldTileBaseGround>();
                baseBackground.SetTile(Tile);
                Tile.AddBackground(baseBackground);
            }
        }

        GameObject waterGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseWater>(), Tile.BackgroundsContainer);
        OverworldTileBaseWater overworldTileBaseWater = waterGO.GetComponent<OverworldTileBaseWater>();
        overworldTileBaseWater.WithWaterType(waterType);
        overworldTileBaseWater.WithConnectionScoreInfo(waterConnectionScore);
        overworldTileBaseWater.SetTile(Tile);

        Tile.SetMainMaterial(new WaterMainMaterial());
        Tile.AddBackground(overworldTileBaseWater);
        Tile.Walkable = false;

        // Update pathConnections for neighbouring tiles
        UpdatePathConnectionsOnNeighbours();

        // Update water connections on neighbours
        UpdateWaterConnectionsOnNeighbours(waterType);

        return overworldTileBaseWater;
    }

    public U PlaceLand<U>() where U : ITileBackground
    {
        U oldBackground = (U)Tile.GetBackgrounds().FirstOrDefault(background => background is U);
        if (oldBackground != null)
        {
            UpdateWaterConnectionsOnNeighbours(new OverworldDefaultWaterType());
            return oldBackground;
        }

        GameObject backgroundGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<U>(), Tile.BackgroundsContainer);
        U baseBackground = backgroundGO.GetComponent<U>();

        // Update water connections on neighbours, because placing somewhere affects coastlines
        UpdateWaterConnectionsOnNeighbours(new OverworldDefaultWaterType());

        baseBackground.SetTile(Tile);
        Tile.AddBackground(baseBackground);
        return baseBackground;
    }

    public override U PlaceBackground<U>()
    {
        Logger.Log($"Place background of type {typeof(U)}");

        switch (typeof(U))
        {
            case Type overworldTileBaseGround when overworldTileBaseGround == typeof(OverworldTileBaseGround):
                Logger.Warning("Set to ground main material");
                Tile.SetMainMaterial(new GroundMainMaterial());
                Logger.Warning($"it is now {Tile.TileMainMaterial}");
                break;
            case Type overworldTileBaseWater when overworldTileBaseWater == typeof(OverworldTileBaseWater):
                return (U)PlaceWater(new OverworldDefaultWaterType());
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
            TilePath tilePathOnNeighbour = neighbour.Value.TryGetTilePath();

            if (tilePathOnNeighbour == null) continue;
            int oldConnectionScoreOnNeighbour = tilePathOnNeighbour.ConnectionScore;
            Logger.Warning($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {Tile.GridLocation.X},{Tile.GridLocation.Y}");

            TileConnectionScoreInfo overworldTilePathConnectionScoreOnNeighbourInfo = NeighbourTileCalculator.MapNeighbourPathsOfTile(neighbour.Value, tilePathOnNeighbour.TilePathType);
            Logger.Log($"We calculated an tile connection type score of neighbour {overworldTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            tilePathOnNeighbour.WithConnectionScoreInfo(overworldTilePathConnectionScoreOnNeighbourInfo);

            if (neighbour.Value.TileMainMaterial == null || neighbour.Value.TileMainMaterial.GetType() == typeof(GroundMainMaterial)
                && oldConnectionScoreOnNeighbour == 16
                && overworldTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore != 16)
            {
                Logger.Warning($"oldConnectionScoreOnNeighbour {oldConnectionScoreOnNeighbour}");

                GameObject backgroundGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseGround>(), neighbour.Value.BackgroundsContainer);
                OverworldTileBaseGround baseBackground = backgroundGO.GetComponent<OverworldTileBaseGround>();
                baseBackground.SetTile(neighbour.Value);
                neighbour.Value.AddBackground(baseBackground);
            }
        }
    }

    private void UpdateWaterConnectionsOnNeighbours(IBaseBackgroundType waterType)
    {
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in Tile.Neighbours)
        {
            TileWater waterOnNeighbour = neighbour.Value.TryGetTileWater();

            if (waterOnNeighbour == null) continue;
            int oldConnectionScoreOnNeighbour = waterOnNeighbour.ConnectionScore;
            Logger.Warning($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {Tile.GridLocation.X},{Tile.GridLocation.Y}");

            TileConnectionScoreInfo overworldTileWaterConnectionScoreOnNeighbourInfo = NeighbourTileCalculator.MapNeighbourWaterOfTile(neighbour.Value, waterType);
            Logger.Log($"We calculated an maze connection type score of neighbour {overworldTileWaterConnectionScoreOnNeighbourInfo.RawConnectionScore} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            waterOnNeighbour.WithConnectionScoreInfo(overworldTileWaterConnectionScoreOnNeighbourInfo);

            if (neighbour.Value.TileMainMaterial?.GetType() == typeof(WaterMainMaterial) && oldConnectionScoreOnNeighbour == 16 && overworldTileWaterConnectionScoreOnNeighbourInfo.RawConnectionScore != 16)
            {
                PlaceBackground<OverworldTileBaseWater>();
            }
        }
    }
}
