using System;

namespace DataSerialisation
{
    [Serializable]
    public class SerialisableTilePathBackground : ISerialisableTileBackground
    {
        public int TileConnectionScore;
        public SerialisableTilePathBackground(int pathConnectionScore)
        {
            TileConnectionScore = pathConnectionScore;
        }
    }
}