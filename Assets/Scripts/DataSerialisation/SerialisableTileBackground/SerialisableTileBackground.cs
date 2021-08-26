using System;
using UnityEngine;

namespace DataSerialisation
{
    [Serializable]
    public class SerialisableTileBackground
    {
        public int TileBackgroundId;
        public string BackgroundType;
        public string SerialisedData;

        public SerialisableTileBackground(string backgroundType, ISerialisableTileBackground iSerialisableTileBackground)
        {
            BackgroundType = backgroundType;
            SerialisedData = JsonUtility.ToJson(iSerialisableTileBackground);
        }

        public static Type GetType(string typeString)
        {
            return Type.GetType("DataSerialisation." + typeString);
        }
    }
}