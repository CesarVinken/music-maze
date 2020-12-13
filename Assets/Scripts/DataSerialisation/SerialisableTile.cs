using System;
using System.Collections.Generic;

[Serializable]
public class SerialisableTile
{
    public string Id;
    public List<SerialisableTileAttribute> TileAttributes;
    public List<SerialisableTileBackground> TileBackgrounds;

    public int GridLocationX;
    public int GridLocationY;

    public SerialisableTile(Tile tile)
    {
        Id = tile.TileId;
        TileAttributes = SerialiseTileAttributes(tile);
        TileBackgrounds = SerialiseTileBackgrounds(tile);
        GridLocationX = tile.GridLocation.X;
        GridLocationY = tile.GridLocation.Y;
    }

    public SerialisableTile(string id, List<SerialisableTileAttribute> tileAttributes, List<SerialisableTileBackground> tileBackgrounds, int gridLocationX, int gridLocationY)
    {
        Id = id;
        TileAttributes = tileAttributes;
        TileBackgrounds = tileBackgrounds;
        GridLocationX = gridLocationX;
        GridLocationY = gridLocationY;
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
                    new SerialisableTileObstacleAttribute(
                        new TileConnectionScoreInfo(tileObstacle.ConnectionScore, tileObstacle.SpriteNumber));
                tileAttributes.Add(serialisableTileObstacleAttribute);
            }
            else if (tileAttribute.GetType() == typeof(PlayerExit))
            {
                PlayerExit playerExit = tileAttribute as PlayerExit;

                SerialisablePlayerExitAttribute serialisablePlayerExitAttribute = new SerialisablePlayerExitAttribute(
                    new TileConnectionScoreInfo(playerExit.ConnectionScore, playerExit.SpriteNumber));
                tileAttributes.Add(serialisablePlayerExitAttribute);
            }
            else if (tileAttribute.GetType() == typeof(PlayerOnly))
            {
                SerialisablePlayerOnlyAttribute serialisablePlayerOnlyAttribute = new SerialisablePlayerOnlyAttribute();
                tileAttributes.Add(serialisablePlayerOnlyAttribute);
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
            else
            {
                Logger.Error($"Could not serialise the tile attribute {tileAttribute.GetType()}");
            }
        }
        return tileAttributes;
    }

    private List<SerialisableTileBackground> SerialiseTileBackgrounds(Tile tile)
    {
        List<SerialisableTileBackground> tilebackgrounds = new List<SerialisableTileBackground>();

        foreach (IMazeTileBackground tileBackground in tile.MazeTileBackgrounds)
        {
            if (tileBackground.GetType() == typeof(MazeTilePath))
            {
                MazeTilePath mazeTilePath = tileBackground as MazeTilePath;
                SerialisableTilePathBackground serialisedTilePathBackground =
                    new SerialisableTilePathBackground(mazeTilePath.ConnectionScore);
                tilebackgrounds.Add(serialisedTilePathBackground);
            }
            else if (tileBackground.GetType() == typeof(MazeTileBaseBackground))
            {
                MazeTileBaseBackground baseBackground = tileBackground as MazeTileBaseBackground;
                SerialisableTileBaseBackground serialisedTileBaseBackground =
                    new SerialisableTileBaseBackground(-1); // TODO: make work/remove connection scores for BaseBackground
                tilebackgrounds.Add(serialisedTileBaseBackground);
            }
            else
            {
                Logger.Error($"Could not serialise the tile background {tileBackground.GetType()}");
            }
        }

        return tilebackgrounds;
    }
}
