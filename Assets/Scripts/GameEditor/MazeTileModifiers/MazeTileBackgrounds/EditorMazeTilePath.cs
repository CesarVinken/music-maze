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
            TileAttributeRemover tileAttributeRemover = new TileAttributeRemover(tile);
            tileAttributeRemover.RemoveTileObstacle();
            tileAttributeRemover.RemovePlayerExit();

            tileBackgroundPlacer.PlacePath(MazeTilePathType.Default);
            return;
        }

        // This path already exists on this tile, so remove it
        tileBackgroundRemover.RemovePath();
    }

    public override void PlaceBackgroundVariation(Tile tile)
    {
        IMazeTileBackground mazeTilePath = (MazeTilePath)tile.MazeTileBackgrounds.FirstOrDefault(background => background is MazeTilePath);
        
        if (mazeTilePath == null) return; // only place variation if there is already a path

        TileBackgroundPlacer tileBackgroundPlacer = new TileBackgroundPlacer(tile);
        tileBackgroundPlacer.PlacePathVariation((MazeTilePath)mazeTilePath);
    }
}

public class EditorMazeTileBaseBackground : EditorMazeTileBackgroundModifier
{
    public override string Name => "Grass";

    public override void PlaceBackground(Tile tile)
    {
        TileBackgroundPlacer tileBackgroundPlacer = new TileBackgroundPlacer(tile);
        TileBackgroundRemover tileBackgroundRemover = new TileBackgroundRemover(tile);

        IMazeTileBackground mazeTileBaseBackground = (MazeTileBaseBackground)tile.MazeTileBackgrounds.FirstOrDefault(background => background is MazeTileBaseBackground);
        if (mazeTileBaseBackground == null)
        {
            tileBackgroundPlacer.PlaceBaseBackground(MazeTileBaseBackgroundType.DefaultGrass);
        }

        tileBackgroundRemover.RemoveBaseBackground(MazeTileBaseBackgroundType.DefaultGrass);
    }

    public override void PlaceBackgroundVariation(Tile tile)
    {
        Logger.Log("Background variations be implemented");
    }
}