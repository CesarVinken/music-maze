using System;
using System.Collections.Generic;

[Serializable]
public class SerialisableTile
{
    public string Id;
    public List<SerialisableTileAttribute> TileAttributes;

    public int GridLocationX;
    public int GridLocationY;

    public SerialisableTile(Tile tile)
    {
        Id = tile.TileId;
        TileAttributes = SerialiseTileAttributes(tile);
        GridLocationX = tile.GridLocation.X;
        GridLocationY = tile.GridLocation.Y;
    }

    private List<SerialisableTileAttribute> SerialiseTileAttributes(Tile tile)
    {
        List<SerialisableTileAttribute> tileAttributes = new List<SerialisableTileAttribute>();

        foreach (IMazeTileAttribute tileAttribute in tile.MazeTileAttributes)
        {
            if (tileAttribute.GetType() == typeof(TileObstacle))
            {
                TileObstacle tileObstacle = tileAttribute as TileObstacle;
                SerialisableTileObstacleAttribute serialisableTileObstacleAttribute = 
                    new SerialisableTileObstacleAttribute(tileObstacle.ObstacleConnectionScore);
                tileAttributes.Add(serialisableTileObstacleAttribute);
            }
            else if (tileAttribute.GetType() == typeof(PlayerExit))
            {
                PlayerExit playerExit = tileAttribute as PlayerExit;

                SerialisablePlayerExitAttribute serialisablePlayerExitAttribute = new SerialisablePlayerExitAttribute(playerExit.ObstacleConnectionScore);
                tileAttributes.Add(serialisablePlayerExitAttribute);
            }
            else if (tileAttribute.GetType() == typeof(PlayerSpawnpoint))
            {
                SerialisablePlayerSpawnpointAttribute serialisablePlayerSpawnpointAttribute = new SerialisablePlayerSpawnpointAttribute();
                tileAttributes.Add(serialisablePlayerSpawnpointAttribute);
            }
            else if (tileAttribute.GetType() == typeof(EnemySpawnpoint))
            {
                SerialisableEnemySpawnpointAttribute serialisableEnemySpawnpointAttribute = new SerialisableEnemySpawnpointAttribute();
                tileAttributes.Add(serialisableEnemySpawnpointAttribute);
            }
        }
        return tileAttributes;
    }
}
