using System.Collections.Generic;

public class MazeLevel
{
    public List<Tile> Tiles = new List<Tile>();

    public MazeLevel()
    {
        Tiles = TilesContainer.Instance.Tiles;
    }

    public static MazeLevel Create()
    {
        Logger.Log(Logger.Initialisation, "Set up new Maze Level");
        return new MazeLevel();
    }
}
