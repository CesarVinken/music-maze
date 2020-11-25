using System;

[Serializable]
public class SerialisableTileBackground
{
    public int TileBackgroundId;

    public int PathConnectionScore; // Should only be on TileObstacle class, but polymorphism is currently not possible on serialisation

    public const int PathBackgroundCode = 0;
}