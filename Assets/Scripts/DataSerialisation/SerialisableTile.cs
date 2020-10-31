using System;
using System.Collections.Generic;

[Serializable]
public class SerialisableTile
{
    public string Id;
    public List<int> TileAttributes;
    //public bool Walkable;   // TODO: turn into enum/int value with different possible wallpieces. Eg. 0 = Walkable, 1 = CornerLeft, 2 = CorderRight etc.
    public int GridLocationX;
    public int GridLocationY;

    public const int ObstacleAttributeCode = 0;
    public const int PlayerExitCode = 1;
    public const int PlayerSpawnpointCode = 2;
    public const int EnemySpawnpointCode = 3;

    public SerialisableTile(Tile tile)
    {
        Id = tile.TileId;
        TileAttributes = SerialiseTileAttributes(tile);
        GridLocationX = tile.GridLocation.X;
        GridLocationY = tile.GridLocation.Y;
    }

    private List<int> SerialiseTileAttributes(Tile tile)
    {
        List<int> tileAttributes = new List<int>();

        foreach (var tileAttribute in tile.MazeTileAttributes)
        {
            if (tileAttribute.GetType() == typeof(TileObstacle))
            {
                tileAttributes.Add(ObstacleAttributeCode);
            }
            if (tileAttribute.GetType() == typeof(PlayerExit))
            {
                tileAttributes.Add(PlayerExitCode);
            }
            if (tileAttribute.GetType() == typeof(PlayerSpawnpoint))
            {
                tileAttributes.Add(PlayerSpawnpointCode);
            }
            if (tileAttribute.GetType() == typeof(EnemySpawnpoint))
            {
                tileAttributes.Add(EnemySpawnpointCode);
            }
        }
        return tileAttributes;
    }
}