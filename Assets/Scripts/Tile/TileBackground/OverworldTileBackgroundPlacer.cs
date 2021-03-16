using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OverworldTileBackgroundPlacer<T> : TileBackgroundPlacer<T> where T : OverworldTile
{
    public override T Tile { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public override void PlacePath(IPathType tilePathType, TileConnectionScoreInfo pathConnectionScoreInfo)
    {
        GameObject overworldTilePathGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<OverworldTilePath>(), Tile.BackgroundsContainer);
        OverworldTilePath overworldTilePath = overworldTilePathGO.GetComponent<OverworldTilePath>();
        overworldTilePath.WithPathType(tilePathType);
        overworldTilePath.WithConnectionScoreInfo(pathConnectionScoreInfo);
        overworldTilePath.SetTile(Tile);

        Tile.AddBackground(overworldTilePath);
    }

    public override void PlaceWater(IBaseBackgroundType waterType, TileConnectionScoreInfo pathConnectionScoreInfo)
    {
        Logger.Warning("TODO: PLACE WATER function");
        //GameObject waterGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<MazeTileBaseWater>(), Tile.BackgroundsContainer);
        //MazeTileBaseWater mazeTileBaseWater = waterGO.GetComponent<MazeTileBaseWater>();
        //mazeTileBaseWater.WithWaterType(waterType);
        //mazeTileBaseWater.WithConnectionScoreInfo(pathConnectionScoreInfo);
        //mazeTileBaseWater.SetTile(Tile);

        //Tile.AddBackground(mazeTileBaseWater);
        //Tile.TryMakeMarkable(true);
    }

    public override void PlaceBackground<U>()
    {
        switch (typeof(U))
        {
            case Type overworldTileBaseGround when overworldTileBaseGround == typeof(OverworldTileBaseGround):
            case Type overworldTilePath when overworldTilePath == typeof(OverworldTilePath):
                Tile.SetMainMaterial(new GroundMainMaterial());
                break;
            case Type overworldTileBaseWater when overworldTileBaseWater == typeof(OverworldTileBaseWater):
                Tile.SetMainMaterial(new WaterMainMaterial());
                break;
            default:
                Logger.Error($"Unknown type {typeof(U)}");
                break;
        }

        List<ITileBackground> backgrounds = Tile.GetBackgrounds();
        U oldBackground = (U)backgrounds.FirstOrDefault(background => background is U);

        if (oldBackground != null) return;

        GameObject baseBackgroundGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<U>(), Tile.BackgroundsContainer);
        U baseBackground = baseBackgroundGO.GetComponent<U>();

        baseBackground.SetTile(Tile);
        Tile.AddBackground(baseBackground);
    }
}
