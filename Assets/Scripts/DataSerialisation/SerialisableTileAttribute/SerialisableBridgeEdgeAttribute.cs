using System;

namespace DataSerialisation
{
    [Serializable]
    public class SerialisableBridgeEdgeAttribute : ISerialisableTileAttribute
    {
        public string BridgeEdgeSide;
        public SerialisableBridgeEdgeAttribute(Direction bridgeEdgeSide)
        {
            BridgeEdgeSide = bridgeEdgeSide.ToString();
        }
    }
}