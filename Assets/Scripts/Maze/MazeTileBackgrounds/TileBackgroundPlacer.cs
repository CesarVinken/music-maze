using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TileBackgroundPlacer<T> where T : Tile
{
    public abstract T Tile { get; set; }

    // Called in game when we already have the connection score
    public void PlacePath(MazeTilePathType mazeTilePathType, TileConnectionScoreInfo pathConnectionScoreInfo)
    {
        GameObject mazeTilePathGO = GameObject.Instantiate(MazeLevelManager.Instance.TilePathPrefab, Tile.BackgroundsContainer);
        MazeTilePath mazeTilePath = mazeTilePathGO.GetComponent<MazeTilePath>();
        mazeTilePath.WithPathType(mazeTilePathType);
        mazeTilePath.WithConnectionScoreInfo(pathConnectionScoreInfo);
        mazeTilePath.SetTile(Tile);

        Tile.MazeTileBackgrounds.Add(mazeTilePath);
        Tile.TryMakeMarkable(true);
    }

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

    public void PlaceBaseBackground(MazeTileBaseBackgroundType mazeTileBaseBackgroundType)
    {
        MazeTileBaseBackground oldBackground = (MazeTileBaseBackground)Tile.MazeTileBackgrounds.FirstOrDefault(background => background is MazeTileBaseBackground);
        if (oldBackground != null) return;

        GameObject mazeTileBaseBackgroundGO = GameObject.Instantiate(MazeLevelManager.Instance.TileBaseBackgroundPrefab, Tile.BackgroundsContainer);
        MazeTileBaseBackground mazeTileBaseBackground = mazeTileBaseBackgroundGO.GetComponent<MazeTileBaseBackground>();

        int defaultConnectionScore = -1;
        mazeTileBaseBackground.WithPathConnectionScore(defaultConnectionScore);
        mazeTileBaseBackground.SetTile(Tile);
        Tile.MazeTileBackgrounds.Add(mazeTileBaseBackground as IMazeTileBackground);
    }
}
