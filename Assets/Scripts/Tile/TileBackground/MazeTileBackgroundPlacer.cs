using System;
using System.Linq;
using UnityEngine;

public class MazeTileBackgroundPlacer<T> : TileBackgroundPlacer<T> where T : MazeTile
{
    public override T Tile { get; set; }

    // Called in game when we already have the connection score
    public override void PlacePath(IPathType mazeTilePathType, TileConnectionScoreInfo pathConnectionScoreInfo)
    {
        GameObject mazeTilePathGO = GameObject.Instantiate(MazeLevelGameplayManager.Instance.GetTileBackgroundPrefab<MazeTilePath>(), Tile.BackgroundsContainer);
        MazeTilePath mazeTilePath = mazeTilePathGO.GetComponent<MazeTilePath>();
        mazeTilePath.WithType(mazeTilePathType as IBackgroundType);
        mazeTilePath.WithConnectionScoreInfo(pathConnectionScoreInfo);
        mazeTilePath.SetTile(Tile);

        Tile.AddBackground(mazeTilePath);
        Tile.TryMakeMarkable(true);
    }

    public override void PlaceGround(IBaseBackgroundType groundType, TileConnectionScoreInfo connectionScoreInfo)
    {
        GameObject groundGO = GameObject.Instantiate(MazeLevelGameplayManager.Instance.GetTileBackgroundPrefab<MazeTileBaseGround>(), Tile.BackgroundsContainer);
        MazeTileBaseGround mazeTileBaseGround = groundGO.GetComponent<MazeTileBaseGround>();
        mazeTileBaseGround.WithType(groundType);
        mazeTileBaseGround.WithConnectionScoreInfo(connectionScoreInfo);
        mazeTileBaseGround.SetTile(Tile);

        if(connectionScoreInfo.RawConnectionScore == 16)
        {
            Tile.SetMainMaterial(new GroundMainMaterial());
        }
        
        Tile.AddBackground(mazeTileBaseGround);
    }

    public override void PlaceWater(IBaseBackgroundType waterType, TileConnectionScoreInfo pathConnectionScoreInfo)
    {
        GameObject waterGO = GameObject.Instantiate(MazeLevelGameplayManager.Instance.GetTileBackgroundPrefab<MazeTileBaseWater>(), Tile.BackgroundsContainer);
        MazeTileBaseWater mazeTileBaseWater = waterGO.GetComponent<MazeTileBaseWater>();
        mazeTileBaseWater.WithType(waterType);
        mazeTileBaseWater.SetTile(Tile);

        Tile.SetMainMaterial(new WaterMainMaterial());
        Tile.AddBackground(mazeTileBaseWater);
        Tile.TryMakeMarkable(false);

        Tile.SetWalkable(false);
    }

    public override U PlaceBackground<U>()
    {
        switch (typeof(U))
        {
            case Type mazeTileBaseGround when mazeTileBaseGround == typeof(MazeTileBaseGround):
                Tile.SetMainMaterial(new GroundMainMaterial());
                break;
            case Type mazeTileBaseWater when mazeTileBaseWater == typeof(MazeTileBaseWater):
                Tile.SetMainMaterial(new WaterMainMaterial());
                Tile.SetWalkable(false);

                break;
            default:
                Logger.Error($"Unexpected type {typeof(U)}");
                break;
        }

        U oldBackground = (U)Tile.GetBackgrounds().FirstOrDefault(b => b is U);
        if (oldBackground != null) return oldBackground;

        GameObject backgroundGO = GameObject.Instantiate(MazeLevelGameplayManager.Instance.GetTileBackgroundPrefab<U>(), Tile.BackgroundsContainer);
        U background = backgroundGO.GetComponent<U>();
 
        background.SetTile(Tile);
        Tile.AddBackground(background);

        return background;
    }

    public override void PlaceCornerFiler(TileCorner tileCorner)
    {
        //create cornerfiller
        GameObject backgroundGO = GameObject.Instantiate(MazeLevelGameplayManager.Instance.GetTileBackgroundPrefab<TileCornerFiller>(), Tile.BackgroundsContainer);
        TileCornerFiller cornerFiller = backgroundGO.GetComponent<TileCornerFiller>();

        cornerFiller.SetTile(Tile);
        cornerFiller.WithType(new MazeLevelDefaultGroundType());
        cornerFiller.WithCorner(tileCorner); // pick sprite based on corner

        Tile.AddCornerFiller(cornerFiller);
    }
}
