using System;

namespace DataSerialisation
{
    [Serializable]
    public class SerialisableTileArea
    {
        public string Id;
        public string Name;

        public SerialisableTileArea(TileArea tileArea)
        {
            Name = tileArea.Name;
            Id = tileArea.Id;
        }
    }
}