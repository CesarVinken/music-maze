using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TileBackgroundPlacer<T> where T : Tile
{
    public abstract T Tile { get; set; }

    public abstract void PlacePath(IPathType tilePathType, TileConnectionScoreInfo pathConnectionScoreInfo);

    public void PlacePathVariation(MazeTilePath mazeTilePath)
    {
        //return only connections that were updated
        List<MazeTilePath> updatedPathConnections = NeighbourTileCalculator.GetUpdatedTileModifiersForVariation<MazeTilePath>(Tile, mazeTilePath, mazeTilePath.MazeTilePathType.ToString());

        //update the sprites with the new variations
        for (int i = 0; i < updatedPathConnections.Count; i++)
        {
            updatedPathConnections[i].WithConnectionScoreInfo(new TileConnectionScoreInfo(updatedPathConnections[i].ConnectionScore, updatedPathConnections[i].SpriteNumber));
        }
    }

    public abstract void PlaceBaseBackground(IBaseBackgroundType baseBackgroundType);
    //public void PlaceBaseBackground(IBaseBackgroundType baseBackgroundType)
    //{
    //    MazeTileBaseBackground oldBackground = (MazeTileBaseBackground)Tile.TileBackgrounds.FirstOrDefault(background => background is MazeTileBaseBackground);
    //    if (oldBackground != null) return;

    //    GameObject baseBackgroundGO = GameObject.Instantiate(MazeLevelManager.Instance.TileBaseBackgroundPrefab, Tile.BackgroundsContainer);
    //    MazeTileBaseBackground baseBackground = baseBackgroundGO.GetComponent<MazeTileBaseBackground>();

    //    int defaultConnectionScore = -1;
    //    baseBackground.WithPathConnectionScore(defaultConnectionScore);
    //    baseBackground.SetTile(Tile);
    //    Tile.TileBackgrounds.Add(baseBackground as ITileBackground);
    //}
}
