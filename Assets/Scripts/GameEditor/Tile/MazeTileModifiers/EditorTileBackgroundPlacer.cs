using System.Collections.Generic;
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
        Tile.TryMakeMarkable(true);

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

        Tile.RemoveTransformationTriggerers();
    }
}
