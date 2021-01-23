using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorTileBackgroundPlacer : TileBackgroundPlacer<EditorTile>
{
    private EditorTile _tile;

    public override EditorTile Tile { get => _tile; set => _tile = value; }

    public EditorTileBackgroundPlacer(EditorTile tile)
    {
        Tile = tile;
    }

    // Called in the editor, when we need to update the connection score
    public void PlacePath(MazeTilePathType mazeTilePathType)
    {
        Logger.Warning("Start placing path.....");

        TileConnectionScoreInfo pathConnectionScore = NeighbourTileCalculator.MapNeighbourPathsOfTile(Tile, mazeTilePathType);
        Logger.Log($"Found a score of {pathConnectionScore.RawConnectionScore}");
        GameObject mazeTilePathGO = GameObject.Instantiate(MazeLevelManager.Instance.TilePathPrefab, Tile.BackgroundsContainer);
        MazeTilePath mazeTilePath = mazeTilePathGO.GetComponent<MazeTilePath>();
        mazeTilePath.WithPathType(MazeTilePathType.Default);
        mazeTilePath.WithConnectionScoreInfo(pathConnectionScore);
        Tile.MazeTileBackgrounds.Add(mazeTilePath as IMazeTileBackground);

        // Update pathConnections for neighbouring tiles
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in Tile.Neighbours)
        {
            MazeTilePath tilePathOnNeighbour = neighbour.Value.TryGetTilePath();

            if (tilePathOnNeighbour == null) continue;
            int oldConnectionScoreOnNeighbour = tilePathOnNeighbour.ConnectionScore;
            Logger.Warning($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {Tile.GridLocation.X},{Tile.GridLocation.Y}");

            TileConnectionScoreInfo mazeTilePathConnectionScoreOnNeighbourInfo = NeighbourTileCalculator.MapNeighbourPathsOfTile(neighbour.Value, mazeTilePathType);
            Logger.Log($"We calculated an maze connection type score of neighbour {mazeTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            tilePathOnNeighbour.WithConnectionScoreInfo(mazeTilePathConnectionScoreOnNeighbourInfo);

            if (oldConnectionScoreOnNeighbour == 16 && mazeTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore != 16)
            {
                PlaceBaseBackground(MazeTileBaseBackgroundType.DefaultGrass);
            }
        }
    }
}

public class InGameTileBackgroundPlacer : TileBackgroundPlacer<InGameTile>
{
    private InGameTile _tile;

    public override InGameTile Tile { get => _tile; set => _tile = value; }

    public InGameTileBackgroundPlacer(InGameTile tile)
    {
        Tile = tile;
    }
}

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
