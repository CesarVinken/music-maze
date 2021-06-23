using System;
using System.Collections.Generic;

[Serializable]
public class SerialisableTile
{
    public string Id;
    public SerialisableTileMainMaterial MainMaterial;

    public List<SerialisableTileAttribute> TileAttributes;
    public List<SerialisableTileBackground> TileBackgrounds;
    public List<SerialisableTileCornerFiller> TileCornerFillers;
    public List<string> TileAreaIds;

    public SerialisableGridLocation GridLocation;

    public List<SerialisableGridLocation> TilesToTransform;

    public SerialisableTile(Tile tile)
    {
        Id = tile.TileId;
        MainMaterial = SerialiseMainMaterial(tile);
        TileAttributes = SerialiseTileAttributes(tile);
        TileBackgrounds = SerialiseTileBackgrounds(tile);
        TileCornerFillers = SerialiseTileCornerFillers(tile);
        TileAreaIds = SerialiseTileAreaIds(tile);

        GridLocation = new SerialisableGridLocation(tile.GridLocation.X, tile.GridLocation.Y);

        if(tile is IMazeLevel)
        {
            List<EditorMazeTile> tilesToTransform = MazeLevelGameplayManager.Instance.EditorLevel.FindTilesToTransform(tile as EditorMazeTile);
            TilesToTransform = SerialiseTilesToTransform(tilesToTransform);
        }
    }

    public SerialisableTile(string id,
        SerialisableTileMainMaterial mainMaterial,
        List<SerialisableTileAttribute> tileAttributes,
        List<SerialisableTileBackground> tileBackgrounds,
        List<SerialisableTileCornerFiller> tileCornerFillers,
        int gridLocationX, int gridLocationY)
    {
        Id = id;
        MainMaterial = mainMaterial;
        TileAttributes = tileAttributes;
        TileBackgrounds = tileBackgrounds;
        TileCornerFillers = tileCornerFillers;
        GridLocation = new SerialisableGridLocation(gridLocationX, gridLocationY);
    }

    private SerialisableTileMainMaterial SerialiseMainMaterial(Tile tile)
    {  
        return new SerialisableTileMainMaterial(tile.TileMainMaterial.ToString(), CreateSerialisableTileMainMaterial(tile.TileMainMaterial));
    }

    private List<SerialisableTileAttribute> SerialiseTileAttributes(Tile tile)
    {
        List<SerialisableTileAttribute> tileAttributes = new List<SerialisableTileAttribute>();

        foreach (ITileAttribute tileAttribute in tile.GetAttributes())
        {
            ISerialisableTileAttribute iSerialisableTileAttribute = CreateSerialisableTileAttribute(tile, tileAttribute);
            string attributeType = iSerialisableTileAttribute.GetType().ToString();

            SerialisableTileAttribute serialisableTileAttribute = new SerialisableTileAttribute(
                attributeType,
                iSerialisableTileAttribute
            );

            tileAttributes.Add(serialisableTileAttribute);
        }

        return tileAttributes;
    }

    private List<SerialisableTileBackground> SerialiseTileBackgrounds(Tile tile)
    {
        List<SerialisableTileBackground> tilebackgrounds = new List<SerialisableTileBackground>();

        foreach (ITileBackground tileBackground in tile.GetBackgrounds())
        {
            ISerialisableTileBackground iSerialisableTileBackground = CreateSerialisableTileBackground(tileBackground);

            SerialisableTileBackground serialisableTileBackground = new SerialisableTileBackground(
                iSerialisableTileBackground.GetType().ToString(), iSerialisableTileBackground
                );
            tilebackgrounds.Add(serialisableTileBackground);
        }

        return tilebackgrounds;
    }

    private List<SerialisableTileCornerFiller> SerialiseTileCornerFillers(Tile tile)
    {
        List<SerialisableTileCornerFiller> tileCornerFillers = new List<SerialisableTileCornerFiller>();

        foreach (TileCornerFiller tileCornerFiller in tile.GetCornerFillers())
        {
            SerialisableTileCornerFiller serialisableTileCornerFiller = CreateSerialisableTileCornerFiller(tileCornerFiller);

            tileCornerFillers.Add(serialisableTileCornerFiller);
        }

        return tileCornerFillers;
    }

    private List<string> SerialiseTileAreaIds(Tile tile)
    {
        List<string> tileAreaIds = new List<string>();
        List<TileArea> tileAreas = tile.GetTileAreas();

        for (int i = 0; i < tileAreas.Count; i++)
        {
            tileAreaIds.Add(tileAreas[i].Id);
        }

        return tileAreaIds;
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

    private ISerialisableTileMainMaterial CreateSerialisableTileMainMaterial(ITileMainMaterial mainMaterial)
    {
        if (mainMaterial.GetType() == typeof(GroundMainMaterial))
        {
            return new SerialisableLandMaterial();
        }
        else if (mainMaterial.GetType() == typeof(WaterMainMaterial))
        {
            return new SerialisableWaterMaterial();
        }
        else
        {
            Logger.Error($"Could not serialise the main material {mainMaterial.GetType()}");
            return null;
        }
    }

    private ISerialisableTileBackground CreateSerialisableTileBackground(ITileBackground tileBackground)
    {
        if (tileBackground.GetType() == typeof(MazeTilePath) || tileBackground.GetType() == typeof(OverworldTilePath))
        {
            TilePath tilePath = tileBackground as TilePath;

            SerialisableTilePathBackground serialisableTilePathBackground = new SerialisableTilePathBackground(tilePath.ConnectionScore);
            return serialisableTilePathBackground;
        }
        else if (tileBackground.GetType() == typeof(MazeTileBaseWater) || tileBackground.GetType() == typeof(OverworldTileBaseWater))
        {
            SerialisableTileBaseWater serialisableTileBaseWater = new SerialisableTileBaseWater();
            return serialisableTileBaseWater;
        }
        else if (tileBackground.GetType() == typeof(MazeTileBaseGround) || (tileBackground.GetType() == typeof(OverworldTileBaseGround)))
        {
            TileBaseGround tileGround = tileBackground as TileBaseGround;

            SerialisableTileBaseGround serialisableTileBaseGround = new SerialisableTileBaseGround(tileGround.ConnectionScore);

            return serialisableTileBaseGround;
        }
        else
        {
            Logger.Error($"Could not serialise the tile background {tileBackground.GetType()}");
            return null;
        }
    }

    private ISerialisableTileAttribute CreateSerialisableTileAttribute(Tile tile, ITileAttribute tileAttribute)
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
            EnemySpawnpoint enemySpawnpoint = tileAttribute as EnemySpawnpoint;
            SerialisableEnemySpawnpointAttribute serialisableEnemySpawnpointAttribute = new SerialisableEnemySpawnpointAttribute(enemySpawnpoint.TileAreas);
            return serialisableEnemySpawnpointAttribute;
        }
        else if (tileAttribute.GetType() == typeof(MazeLevelEntry))
        {
            MazeLevelEntry mazeLevelEntry = tileAttribute as MazeLevelEntry;
            SerialisableMazeLevelEntryAttribute serialisableMazeLevelEntryAttribute = new SerialisableMazeLevelEntryAttribute(mazeLevelEntry.MazeLevelName);
            return serialisableMazeLevelEntryAttribute;
        }
        else if (tileAttribute.GetType() == typeof(BridgePiece))
        {
            BridgePiece bridgePiece = tileAttribute as BridgePiece;
            BridgePieceDirection bridgePieceDirection = bridgePiece.BridgePieceDirection;

            SerialisableBridgePieceAttribute serialisableBridgePieceAttribute = new SerialisableBridgePieceAttribute(bridgePieceDirection);
            return serialisableBridgePieceAttribute;
        }
        else if (tileAttribute.GetType() == typeof(BridgeEdge))
        {
            BridgeEdge bridgeEdge = tileAttribute as BridgeEdge;
            
            SerialisableBridgeEdgeAttribute serialisableBridgeEdgeAttribute = new SerialisableBridgeEdgeAttribute(bridgeEdge.EdgeSide);
            return serialisableBridgeEdgeAttribute;
        }
        else
        {
            Logger.Error($"Could not serialise the tile attribute {tileAttribute.GetType()}");
            return null;
        }
    }

    private SerialisableTileCornerFiller CreateSerialisableTileCornerFiller(TileCornerFiller tileCornerFiller)
    {
        SerialisableTileCornerFiller serialisableTileCornerFiller =
            new SerialisableTileCornerFiller(tileCornerFiller.TileCorner.ToString());
        return serialisableTileCornerFiller;
    }
}
