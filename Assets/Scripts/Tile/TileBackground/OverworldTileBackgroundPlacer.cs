using System.Linq;
using UnityEngine;

public class OverworldTileBackgroundPlacer<T> : TileBackgroundPlacer<T> where T : OverworldTile
{
    public override T Tile { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public override void PlacePath(IPathType tilePathType, TileConnectionScoreInfo pathConnectionScoreInfo)
    {
        GameObject overworldTilePathGO = GameObject.Instantiate(OverworldManager.Instance.TilePathPrefab, Tile.BackgroundsContainer);
        OverworldTilePath overworldTilePath = overworldTilePathGO.GetComponent<OverworldTilePath>();
        overworldTilePath.WithPathType(tilePathType);
        overworldTilePath.WithConnectionScoreInfo(pathConnectionScoreInfo);
        overworldTilePath.SetTile(Tile);

        Tile.TileBackgrounds.Add(overworldTilePath);
    }

    public override void PlaceBaseBackground(IBaseBackgroundType baseBackgroundType)
    {
        OverworldTileBaseBackground oldBackground = (OverworldTileBaseBackground)Tile.TileBackgrounds.FirstOrDefault(background => background is OverworldTileBaseBackground);
        if (oldBackground != null) return;

        GameObject baseBackgroundGO = GameObject.Instantiate(OverworldManager.Instance.TileBaseBackgroundPrefab, Tile.BackgroundsContainer);
        OverworldTileBaseBackground baseBackground = baseBackgroundGO.GetComponent<OverworldTileBaseBackground>();

        int defaultConnectionScore = -1;
        baseBackground.WithPathConnectionScore(defaultConnectionScore);
        baseBackground.SetTile(Tile);
        Tile.TileBackgrounds.Add(baseBackground as ITileBackground);
    }
}
