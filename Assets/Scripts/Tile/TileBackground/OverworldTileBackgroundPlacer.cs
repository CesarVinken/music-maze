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

    public override void PlaceBackground(IBaseBackgroundType baseBackgroundType)
    {
        List<ITileBackground> backgrounds = Tile.GetBackgrounds();
        OverworldTileBaseGround oldBackground = (OverworldTileBaseGround)backgrounds.FirstOrDefault(background => background is OverworldTileBaseGround);

        if (oldBackground != null) return;

        GameObject baseBackgroundGO = GameObject.Instantiate(OverworldManager.Instance.GetTileBackgroundPrefab<OverworldTileBaseGround>(), Tile.BackgroundsContainer);
        OverworldTileBaseGround baseBackground = baseBackgroundGO.GetComponent<OverworldTileBaseGround>();

        int defaultConnectionScore = -1;
        baseBackground.WithPathConnectionScore(defaultConnectionScore);
        baseBackground.SetTile(Tile);
        Tile.AddBackground(baseBackground as ITileBackground);
    }
}
