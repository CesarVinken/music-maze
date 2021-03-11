using System.Collections.Generic;
using System.Linq;

public class MazeTileBackgroundRemover : TileBackgroundRemover
{
    private EditorMazeTile _tile;

    public MazeTileBackgroundRemover(EditorMazeTile tile)
    {
        _tile = tile;
    }

    public override void RemovePath()
    {
        MazeTilePath mazeTilePath = (MazeTilePath)_tile.GetBackgrounds().FirstOrDefault(background => background is MazeTilePath);
        if (mazeTilePath == null) return;

        Logger.Log(mazeTilePath.TilePathType);
        IPathType mazeTilePathType = mazeTilePath.TilePathType;
        int oldConnectionScore = mazeTilePath.ConnectionScore;

        // If needed, place a background in the gap that the removed path left
        if (oldConnectionScore == NeighbourTileCalculator.ConnectionOnAllSidesScore)
        {
            EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(_tile);
            tileBackgroundPlacer.PlaceBaseBackground(new MazeLevelDefaultBaseBackgroundType());
        }

        _tile.RemoveBackground(mazeTilePath);
        mazeTilePath.Remove();

        TrySetTileNotMarkable();


        //After removing tile, check with neighbour tiles if wall connections should be updated
        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in _tile.Neighbours)
        {
            TilePath mazeTilePathOnNeighbour = neighbour.Value.TryGetTilePath();

            if (mazeTilePathOnNeighbour == null) continue;

            int oldConnectionScoreOnNeighbour = mazeTilePathOnNeighbour.ConnectionScore;

            Logger.Warning($"We will now look for connections for neighbour {neighbour.Value.GridLocation.X},{neighbour.Value.GridLocation.Y}, which is {neighbour.Key} of {_tile.GridLocation.X},{_tile.GridLocation.Y}");
            TileConnectionScoreInfo mazeTilePathConnectionScoreOnNeighbourInfo = NeighbourTileCalculator.MapNeighbourPathsOfTile(neighbour.Value, mazeTilePathType);
            Logger.Log($"We calculated an path connection type score of {mazeTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore} for location {neighbour.Value.GridLocation.X}, {neighbour.Value.GridLocation.Y}");

            //update connection score on neighbour
            mazeTilePathOnNeighbour.WithConnectionScoreInfo(mazeTilePathConnectionScoreOnNeighbourInfo);

            //Add background where needed
            if (oldConnectionScoreOnNeighbour == NeighbourTileCalculator.ConnectionOnAllSidesScore && mazeTilePathConnectionScoreOnNeighbourInfo.RawConnectionScore != NeighbourTileCalculator.ConnectionOnAllSidesScore)
            {
                EditorMazeTileBackgroundPlacer tileBackgroundPlacer = new EditorMazeTileBackgroundPlacer(neighbour.Value as EditorMazeTile);
                tileBackgroundPlacer.PlaceBaseBackground(new MazeLevelDefaultBaseBackgroundType());
            }
        }

        _tile.RemoveTileAsBeautificationTrigger();
    }

    public override void RemoveBaseBackground(IBaseBackgroundType mazeTileBaseBackgroundType)
    {
        MazeTileBaseBackground mazeTileBaseBackground = (MazeTileBaseBackground)_tile.GetBackgrounds().FirstOrDefault(background => background is MazeTileBaseBackground);

        if (mazeTileBaseBackground == null) return;

        _tile.RemoveBackground(mazeTileBaseBackground);
        mazeTileBaseBackground.Remove();
    }

    private void TrySetTileNotMarkable()
    {
        MazeTilePath mazeTilePath = (MazeTilePath)_tile.GetBackgrounds().FirstOrDefault(background => background is MazeTilePath);

        if (mazeTilePath == null)
        {
            _tile.TryMakeMarkable(false);
        }
    }
}
