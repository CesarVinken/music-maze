using System;

namespace DataSerialisation
{
    [Serializable]
    public class SerialisableTileCornerFiller
    {
        public string TileCorner;

        public SerialisableTileCornerFiller(string tileCorner)
        {
            TileCorner = tileCorner;
        }
    }
}