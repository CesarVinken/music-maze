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

    public override void PlaceBackground(IBaseBackgroundType baseBackgroundType)
    {
        Logger.Log($"Place background of type {baseBackgroundType.GetType()}");

        //TODO: Do not only check for BaseGround, but also Water
        MazeTileBaseGround oldBackground = (MazeTileBaseGround)Tile.GetBackgrounds().FirstOrDefault(background => background is MazeTileBaseGround);
        if (oldBackground != null) return;

        GameObject baseGroundGO = GameObject.Instantiate(MazeLevelManager.Instance.GetTileBackgroundPrefab<MazeTileBaseGround>(), Tile.BackgroundsContainer);
        MazeTileBaseGround baseBackground = baseGroundGO.GetComponent<MazeTileBaseGround>();

        int defaultConnectionScore = -1;
        baseBackground.WithPathConnectionScore(defaultConnectionScore);
        baseBackground.SetTile(Tile);
        Tile.AddBackground(baseBackground as ITileBackground);
    }
}
