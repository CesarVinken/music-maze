using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OverworldTileBackgroundPlacer<T> : TileBackgroundPlacer<T> where T : OverworldTile
{
    public override T Tile { get; set; }

    // Called in game when we already have the connection score
    public override void PlacePath(IPathType tilePathType, TileConnectionScoreInfo pathConnectionScoreInfo)
    {
        GameObject overworldTilePathGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<OverworldTilePath>(), Tile.BackgroundsContainer);
        OverworldTilePath overworldTilePath = overworldTilePathGO.GetComponent<OverworldTilePath>();
        overworldTilePath.WithType(tilePathType as IBackgroundType);
        overworldTilePath.WithConnectionScoreInfo(pathConnectionScoreInfo);
        overworldTilePath.SetTile(Tile);

        Tile.AddBackground(overworldTilePath);
    }

    public override void PlaceGround(IBaseBackgroundType groundType, TileConnectionScoreInfo connectionScoreInfo)
    {
        GameObject groundGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseGround>(), Tile.BackgroundsContainer);
        OverworldTileBaseGround mazeTileBaseGround = groundGO.GetComponent<OverworldTileBaseGround>();
        mazeTileBaseGround.WithType(groundType);
        mazeTileBaseGround.WithConnectionScoreInfo(connectionScoreInfo);
        mazeTileBaseGround.SetTile(Tile);

        //Tile.SetMainMaterial(new GroundMainMaterial());
        Tile.AddBackground(mazeTileBaseGround);
        //Tile.Walkable = true;
    }

    public override void PlaceWater(IBaseBackgroundType waterType, TileConnectionScoreInfo pathConnectionScoreInfo)
    {
        if (Tile.GridLocation.X == 0 && Tile.GridLocation.Y == 0)
        {
            Logger.Warning($"Set water HAMASDKQ");
        }
        GameObject waterGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseWater>(), Tile.BackgroundsContainer);
        OverworldTileBaseWater overworldTileBaseWater = waterGO.GetComponent<OverworldTileBaseWater>();
        overworldTileBaseWater.WithType(waterType);
        overworldTileBaseWater.SetTile(Tile);

        Tile.AddBackground(overworldTileBaseWater);
        Tile.SetWalkable(false);
    }

    public override U PlaceBackground<U>()
    {
        switch (typeof(U))
        {
            case Type overworldTileBaseGround when overworldTileBaseGround == typeof(OverworldTileBaseGround):
            case Type overworldTilePath when overworldTilePath == typeof(OverworldTilePath):
                Tile.SetMainMaterial(new GroundMainMaterial());
                break;
            case Type overworldTileBaseWater when overworldTileBaseWater == typeof(OverworldTileBaseWater):
                if (Tile.GridLocation.X == 0 && Tile.GridLocation.Y == 0)
                {
                    Logger.Warning($"Set water here");
                }
                Tile.SetMainMaterial(new WaterMainMaterial());
                Tile.SetWalkable(false);
                break;
            default:
                Logger.Error($"Unknown type {typeof(U)}");
                break;
        }

        List<ITileBackground> backgrounds = Tile.GetBackgrounds();
        U oldBackground = (U)backgrounds.FirstOrDefault(background => background is U);

        if (oldBackground != null) return oldBackground;

        GameObject baseBackgroundGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<U>(), Tile.BackgroundsContainer);
        U baseBackground = baseBackgroundGO.GetComponent<U>();

        baseBackground.SetTile(Tile);
        Tile.AddBackground(baseBackground);

        return baseBackground;
    }
}
