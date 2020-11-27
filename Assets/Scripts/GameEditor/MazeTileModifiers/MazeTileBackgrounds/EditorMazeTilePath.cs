using System.Linq;

public class EditorMazeTilePath : EditorMazeTileBackgroundModifier
{
    public override string Name => "Path";

    public override void PlaceBackground(Tile tile)
    {
        TileBackgroundPlacer tileBackgroundPlacer = new TileBackgroundPlacer(tile);
        TileBackgroundRemover tileBackgroundRemover = new TileBackgroundRemover(tile);

        IMazeTileBackground mazeTilePath = (MazeTilePath)tile.MazeTileBackgrounds.FirstOrDefault(background => background is MazeTilePath);
        if (mazeTilePath == null)
        {
            Logger.Log("there is no path yet, place the path");

            TileAttributeRemover tileAttributeRemover = new TileAttributeRemover(tile);
            tileAttributeRemover.RemoveTileObstacle();
            tileAttributeRemover.RemovePlayerExit();

            tileBackgroundPlacer.PlacePath(MazeTilePathType.Default);
            return;
        }
        Logger.Log("This path already exists on this tile, so remove it");
        // This path already exists on this tile, so remove it
        tileBackgroundRemover.RemovePath();
    }
}