using System;

namespace DataSerialisation
{
    [Serializable]
    public class SerialisableBridgePieceAttribute : ISerialisableTileAttribute
    {
        public string BridgePieceDirection;
        public SerialisableBridgePieceAttribute(BridgePieceDirection bridgePieceDirection)
        {
            BridgePieceDirection = bridgePieceDirection.ToString();
        }
    }
}