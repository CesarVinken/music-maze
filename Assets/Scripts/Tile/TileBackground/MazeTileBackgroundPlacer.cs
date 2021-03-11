using System.Linq;
using UnityEngine;

public class MazeTileBackgroundPlacer<T> : TileBackgroundPlacer<T> where T : MazeTile
{
    public override T Tile { get; set; }

    // Called in game when we already have the connection score
    public override void PlacePath(IPathType mazeTilePathType, TileConnectionScoreInfo pathConnectionScoreInfo)
    {
        GameObject mazeTilePathGO = GameObject.Instantiate(MazeLevelManager.Instance.TilePathPrefab, Tile.BackgroundsContainer);
        MazeTilePath mazeTilePath = mazeTilePathGO.GetComponent<MazeTilePath>();
        mazeTilePath.WithPathType(mazeTilePathType);
        mazeTilePath.WithConnectionScoreInfo(pathConnectionScoreInfo);
        mazeTilePath.SetTile(Tile);

        Tile.AddBackground(mazeTilePath);
        Tile.TryMakeMarkable(true);
    }

    public override void PlaceBaseBackground(IBaseBackgroundType baseBackgroundType)
    {
        MazeTileBaseBackground oldBackground = (MazeTileBaseBackground)Tile.GetBackgrounds().FirstOrDefault(background => background is MazeTileBaseBackground);
        if (oldBackground != null) return;

        GameObject baseBackgroundGO = GameObject.Instantiate(MazeLevelManager.Instance.TileBaseBackgroundPrefab, Tile.BackgroundsContainer);
        MazeTileBaseBackground baseBackground = baseBackgroundGO.GetComponent<MazeTileBaseBackground>();

        int defaultConnectionScore = -1;
        baseBackground.WithPathConnectionScore(defaultConnectionScore);
        baseBackground.SetTile(Tile);
        Tile.AddBackground(baseBackground as ITileBackground);
    }
}
