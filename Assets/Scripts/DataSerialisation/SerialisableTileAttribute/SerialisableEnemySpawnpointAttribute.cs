using System;
using System.Collections.Generic;

namespace DataSerialisation
{
    [Serializable]
    public class SerialisableEnemySpawnpointAttribute : ISerialisableTileAttribute
    {
        public List<string> TileAreaIds = new List<string>();

        // To do: Enemy type

        public SerialisableEnemySpawnpointAttribute(List<TileArea> tileAreas)
        {
            for (int i = 0; i < tileAreas.Count; i++)
            {
                TileAreaIds.Add(tileAreas[i].Id);
            }
        }
    }
}