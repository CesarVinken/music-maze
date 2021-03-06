using System;
using System.Collections.Generic;

[Serializable]
public class SerialisableTile
{
    public string Id;
    public List<SerialisableTileAttribute> TileAttributes;
    public List<SerialisableTileBackground> TileBackgrounds;

    public SerialisableGridLocation GridLocation;

    public List<SerialisableGridLocation> TilesToTransform;

    public SerialisableTile(Tile tile)
    {
        Id = tile.TileId;
        TileAttributes = SerialiseTileAttributes(tile);
        TileBackgrounds = SerialiseTileBackgrounds(tile);

        GridLocation = new SerialisableGridLocation(tile.GridLocation.X, tile.GridLocation.Y);

        if(tile is IMazeLevel)
        {
            List<EditorMazeTile> tilesToTransform = MazeLevelManager.Instance.EditorLevel.FindTilesToTransform(tile as EditorMazeTile);
            TilesToTransform = SerialiseTilesToTransform(tilesToTransform);
        }
    }

    public SerialisableTile(string id, List<SerialisableTileAttribute> tileAttributes, List<SerialisableTileBackground> tileBackgrounds, int gridLocationX, int gridLocationY)
    {
        Id = id;
        TileAttributes = tileAttributes;
        TileBackgrounds = tileBackgrounds;
        GridLocation = new SerialisableGridLocation(gridLocationX, gridLocationY);
    }

    private List<SerialisableTileAttribute> SerialiseTileAttributes(Tile tile)
    {
        List<SerialisableTileAttribute> tileAttributes = new List<SerialisableTileAttribute>();

        foreach (ITileAttribute tileAttribute in tile.TileAttributes)
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
            else if (tileAttribute.GetType() == typeof(MazeLevelEntry))
            {
                SerialisableMazeLevelEntryAttribute serialisableMazeLevelEntryAttribute = new SerialisableMazeLevelEntryAttribute();
                tileAttributes.Add(serialisableMazeLevelEntryAttribute);
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

        foreach (ITileBackground tileBackground in tile.TileBackgrounds)
        {
            if (tileBackground.GetType() == typeof(MazeTilePath) || tileBackground.GetType() == typeof(OverworldTilePath))
            {
                TilePath tilePath = tileBackground as TilePath;
                SerialisableTilePathBackground serialisedTilePathBackground =
                    new SerialisableTilePathBackground(tilePath.ConnectionScore);
                tilebackgrounds.Add(serialisedTilePathBackground);
            }
            else if (tileBackground.GetType() == typeof(MazeTileBaseBackground) || (tileBackground.GetType() == typeof(OverworldTileBaseBackground)))
            {
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

    private List<SerialisableGridLocation> SerialiseTilesToTransform(List<EditorMazeTile> tilesToTransform)
    {
        List<SerialisableGridLocation> serialisableTilesToTransform = new List<SerialisableGridLocation>();

        for (int i = 0; i < tilesToTransform.Count; i++)
        {
            serialisableTilesToTransform.Add(new SerialisableGridLocation(tilesToTransform[i].GridLocation.X, tilesToTransform[i].GridLocation.Y));
        }

        return serialisableTilesToTransform;
    }
}
