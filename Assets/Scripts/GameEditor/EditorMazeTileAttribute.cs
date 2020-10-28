using UnityEngine;

public class EditorMazeTileAttribute
{
    public EditorMazeTileAttributeType AttributeType;
    public string Name = "";
    public Sprite Sprite = null;

    public EditorMazeTileAttribute(EditorMazeTileAttributeType attributeType)
    {
        AttributeType = attributeType;
        switch (attributeType)
        {
            case EditorMazeTileAttributeType.EnemySpawnpoint:
                Name = "Enemy Spawnpoint";
                break;
            case EditorMazeTileAttributeType.Obstacle:
                Name = "Obstacle";
                break;
            case EditorMazeTileAttributeType.PlayerExit:
                Name = "Player Exit";
                break;
            case EditorMazeTileAttributeType.PlayerSpawnpoint:
                Name = "Player Spawnpoint";
                break;
            default:
                Logger.Warning($"The maze tile attribute {attributeType} was not yet implemented");
                break;
        }
    }
}