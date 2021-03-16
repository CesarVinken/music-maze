﻿using System;
using System.Linq;
using UnityEngine;

public class MazeTileBackgroundPlacer<T> : TileBackgroundPlacer<T> where T : MazeTile
{
    public override T Tile { get; set; }

    // Called in game when we already have the connection score
    public override void PlacePath(IPathType mazeTilePathType, TileConnectionScoreInfo pathConnectionScoreInfo)
    {
        GameObject mazeTilePathGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<MazeTilePath>(), Tile.BackgroundsContainer);
        MazeTilePath mazeTilePath = mazeTilePathGO.GetComponent<MazeTilePath>();
        mazeTilePath.WithPathType(mazeTilePathType);
        mazeTilePath.WithConnectionScoreInfo(pathConnectionScoreInfo);
        mazeTilePath.SetTile(Tile);

        Tile.AddBackground(mazeTilePath);
        Tile.TryMakeMarkable(true);
    }

    public override void PlaceWater(IBaseBackgroundType waterType, TileConnectionScoreInfo pathConnectionScoreInfo)
    {
        GameObject waterGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<MazeTileBaseWater>(), Tile.BackgroundsContainer);
        MazeTileBaseWater mazeTileBaseWater = waterGO.GetComponent<MazeTileBaseWater>();
        mazeTileBaseWater.WithWaterType(waterType);
        mazeTileBaseWater.WithConnectionScoreInfo(pathConnectionScoreInfo);
        mazeTileBaseWater.SetTile(Tile);

        Tile.SetMainMaterial(new WaterMainMaterial());
        Tile.AddBackground(mazeTileBaseWater);
        Tile.TryMakeMarkable(false);
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
            //case Type mazeTileBaseWater when mazeTileBaseWater == typeof(MazeTileBaseWater):
            //    //Tile.SetMainMaterial(new WaterMainMaterial());
            //    break;
            default:
                Logger.Error($"Unexpected type {typeof(U)}");
                break;
        }

        U oldBackground = (U)Tile.GetBackgrounds().FirstOrDefault(background => background is U);
        if (oldBackground != null) return;

        GameObject backgroundGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<U>(), Tile.BackgroundsContainer);
        U baseBackground = backgroundGO.GetComponent<U>();

        //int defaultConnectionScore = -1;
        //baseBackground.WithPathConnectionScore(defaultConnectionScore);
        baseBackground.SetTile(Tile);
        Tile.AddBackground(baseBackground);
    }
}
