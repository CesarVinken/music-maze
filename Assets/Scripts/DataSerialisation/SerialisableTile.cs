using System;
using System.Collections.Generic;
using UnityEngine;

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
            ISerialisableTileAttribute iSerialisableTileAttribute = CreateSerialisableTileAttribute(tileAttribute);

            SerialisableTileAttribute serialisableTileAttribute = new SerialisableTileAttribute(
                iSerialisableTileAttribute.GetType().ToString(), iSerialisableTileAttribute
                );
            //serialisableTileAttribute.SerialisedData = JsonUtility.ToJson(iSerialisableTileAttribute);
            //serialisableTileAttribute.AttributeType = iSerialisableTileAttribute.GetType().ToString();

            tileAttributes.Add(serialisableTileAttribute);
        }

        return tileAttributes;
    }

    private List<SerialisableTileBackground> SerialiseTileBackgrounds(Tile tile)
    {
        List<SerialisableTileBackground> tilebackgrounds = new List<SerialisableTileBackground>();

        foreach (ITileBackground tileBackground in tile.TileBackgrounds)
        {
            ISerialisableTileBackground iSerialisableTileBackground = CreateSerialisableTileBackground(tileBackground);

            SerialisableTileBackground serialisableTileBackground = new SerialisableTileBackground(
                iSerialisableTileBackground.GetType().ToString(), iSerialisableTileBackground
                );
            tilebackgrounds.Add(serialisableTileBackground);
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

    private ISerialisableTileBackground CreateSerialisableTileBackground(ITileBackground tileBackground)
    {

        if (tileBackground.GetType() == typeof(MazeTilePath) || tileBackground.GetType() == typeof(OverworldTilePath))
        {
            TilePath tilePath = tileBackground as TilePath;

            SerialisableTilePathBackground serialisableTilePathBackground = new SerialisableTilePathBackground(tilePath.ConnectionScore);
            return serialisableTilePathBackground;
        }
        else if (tileBackground.GetType() == typeof(MazeTileBaseBackground) || (tileBackground.GetType() == typeof(OverworldTileBaseBackground)))
        {
            SerialisableTileBaseBackground serialisableTileBaseBackground = new SerialisableTileBaseBackground();
            return serialisableTileBaseBackground;
        }
        else
        {
            Logger.Error($"Could not serialise the tile background {tileBackground.GetType()}");
            return null;
        }
    }

    private ISerialisableTileAttribute CreateSerialisableTileAttribute(ITileAttribute tileAttribute)
    {
        if (tileAttribute.GetType() == typeof(TileObstacle))
        {
            TileObstacle tileObstacle = tileAttribute as TileObstacle;
            SerialisableTileObstacleAttribute serialisableTileObstacleAttribute =
                new SerialisableTileObstacleAttribute(
                    new TileConnectionScoreInfo(tileObstacle.ConnectionScore, tileObstacle.SpriteNumber));
            return serialisableTileObstacleAttribute;
        }
        else if (tileAttribute.GetType() == typeof(PlayerExit))
        {
            PlayerExit playerExit = tileAttribute as PlayerExit;

            SerialisablePlayerExitAttribute serialisablePlayerExitAttribute = new SerialisablePlayerExitAttribute(
                new TileConnectionScoreInfo(playerExit.ConnectionScore, playerExit.SpriteNumber));
            return serialisablePlayerExitAttribute;
        }
        else if (tileAttribute.GetType() == typeof(PlayerOnly))
        {
            SerialisablePlayerOnlyAttribute serialisablePlayerOnlyAttribute = new SerialisablePlayerOnlyAttribute();
            return serialisablePlayerOnlyAttribute;
        }
        else if (tileAttribute.GetType() == typeof(PlayerSpawnpoint))
        {
            SerialisablePlayerSpawnpointAttribute serialisablePlayerSpawnpointAttribute = new SerialisablePlayerSpawnpointAttribute();
            return serialisablePlayerSpawnpointAttribute;
        }
        else if (tileAttribute.GetType() == typeof(EnemySpawnpoint))
        {
            SerialisableEnemySpawnpointAttribute serialisableEnemySpawnpointAttribute = new SerialisableEnemySpawnpointAttribute();
            return serialisableEnemySpawnpointAttribute;
        }
        if (tileAttribute.GetType() == typeof(MazeLevelEntry))
        {
            MazeLevelEntry mazeLevelEntry = tileAttribute as MazeLevelEntry;
            SerialisableMazeLevelEntryAttribute serialisableMazeLevelEntryAttribute = new SerialisableMazeLevelEntryAttribute(mazeLevelEntry.MazeLevelName);
            return serialisableMazeLevelEntryAttribute;
        }
        else
        {
            Logger.Error($"Could not serialise the tile attribute {tileAttribute.GetType()}");
            return null;
        }
    }
}
