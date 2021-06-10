using System;

[Serializable]
public class SerialisableBridgePieceAttribute : ISerialisableTileAttribute
{
    public string BridgePieceDirection;
    public SerialisableBridgePieceAttribute(BridgePieceDirection bridgePieceDirection)
    {
        BridgePieceDirection = bridgePieceDirection.ToString();
    }
}
