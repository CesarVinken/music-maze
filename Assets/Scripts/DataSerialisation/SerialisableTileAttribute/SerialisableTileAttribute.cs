using System;
using UnityEngine;

public interface ISerialisableTileAttribute
{
}

[Serializable]
public class SerialisableTileAttribute
{
    public int TileAttributeId;
    public string AttributeType;
    public string SerialisedData;

    //public int ConnectionScore; // Should only be on TileObstacle class, but polymorphism is currently not possible on serialisation
    //public int SpriteNumber;

    //public const int ObstacleAttributeCode = 0;
    //public const int PlayerExitCode = 1;
    //public const int PlayerSpawnpointCode = 2;
    //public const int EnemySpawnpointCode = 3;
    //public const int PlayerOnlyAttributeCode = 4;
    //public const int MazeLevelEntryCode = 5;

    public SerialisableTileAttribute(string attributeType, ISerialisableTileAttribute iSerialisableTileAttribute)
    {
        AttributeType = attributeType;
        SerialisedData = JsonUtility.ToJson(iSerialisableTileAttribute);
    }
    //public void SetAttributeType()
    //{
    //    AttributeType = GetType().ToString();
    //}
}