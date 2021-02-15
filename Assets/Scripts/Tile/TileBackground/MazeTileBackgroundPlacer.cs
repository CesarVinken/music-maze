using System.Collections.Generic;
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

        Tile.TileBackgrounds.Add(mazeTilePath);
        Tile.TryMakeMarkable(true);
    }

    //public void PlacePathVariation(MazeTilePath mazeTilePath)
    //{
    //    //return only connections that were updated
    //    List<MazeTilePath> updatedPathConnections = NeighbourTileCalculator.GetUpdatedTileModifiersForVariation<MazeTilePath>(Tile, mazeTilePath, mazeTilePath.MazeTilePathType.ToString());

    //    //update the sprites with the new variations
    //    for (int i = 0; i < updatedPathConnections.Count; i++)
    //    {
    //        updatedPathConnections[i].WithConnectionScoreInfo(new TileConnectionScoreInfo(updatedPathConnections[i].ConnectionScore, updatedPathConnections[i].SpriteNumber));
    //    }
    //}

    public override void PlaceBaseBackground(IBaseBackgroundType baseBackgroundType)
    {
        MazeTileBaseBackground oldBackground = (MazeTileBaseBackground)Tile.TileBackgrounds.FirstOrDefault(background => background is MazeTileBaseBackground);
        if (oldBackground != null) return;

        GameObject baseBackgroundGO = GameObject.Instantiate(MazeLevelManager.Instance.TileBaseBackgroundPrefab, Tile.BackgroundsContainer);
        MazeTileBaseBackground baseBackground = baseBackgroundGO.GetComponent<MazeTileBaseBackground>();

        int defaultConnectionScore = -1;
        baseBackground.WithPathConnectionScore(defaultConnectionScore);
        baseBackground.SetTile(Tile);
        Tile.TileBackgrounds.Add(baseBackground as ITileBackground);
    }
}
