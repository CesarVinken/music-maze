using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorOverworldTileBackgroundPlacer : OverworldTileBackgroundPlacer<EditorOverworldTile>
{
    private EditorOverworldTile _tile;

    public override EditorOverworldTile Tile { get => _tile; set => _tile = value; }

    public EditorOverworldTileBackgroundPlacer(EditorOverworldTile tile)
    {
        Tile = tile;
    }

    // Called in the editor, when we need to update the connection score
    public void PlacePath(IPathType overworldTilePathType)
    {
        Logger.Warning("Start placing path.....");

        TileConnectionScoreInfo pathConnectionScore = NeighbourTileCalculator.MapNeighbourPathsOfTile(Tile, overworldTilePathType);
        Logger.Log($"Found a score of {pathConnectionScore.RawConnectionScore}");
        GameObject overworldTilePathGO = GameObject.Instantiate(OverworldManager.Instance.TilePathPrefab, Tile.BackgroundsContainer);
        OverworldTilePath overworldTilePath = overworldTilePathGO.GetComponent<OverworldTilePath>();
        overworldTilePath.WithPathType(overworldTilePathType);
        overworldTilePath.WithConnectionScoreInfo(pathConnectionScore);
        Tile.AddBackground(overworldTilePath as ITileBackground);

        // Update pathConnections for neighbouring tiles
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in Tile.Neighbours)
        {
            TilePath tilePathOnNeighbour = neighbour.Value.TryGetTilePath();

            if (tilePathOnNeighbour == null) continue;
            int oldConnectionScoreOnNeighbour = tilePathOnNeighbour.ConnectionScore;
            Logger.Warning($"We will look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {Tile.GridLocation.X},{Tile.GridLocation.Y}");

            TileConnectionScoreInfo overworldTilePathConnectionScoreOnNeighbourInfo = NeighbourTileCalculator.MapNeighbourPathsOfTile(neighbour.Value, overworldTilePathType);
            Logger.Log($"We calculated an tile connection type score of neighbour {overworldTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            tilePathOnNeighbour.WithConnectionScoreInfo(overworldTilePathConnectionScoreOnNeighbourInfo);

            if (oldConnectionScoreOnNeighbour == 16 && overworldTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore != 16)
            {
                PlaceBaseBackground(new OverworldDefaultBaseBackgroundType());
            }
        }
    }
}
