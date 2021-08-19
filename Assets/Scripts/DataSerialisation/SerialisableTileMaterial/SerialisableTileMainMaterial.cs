using System;
using UnityEngine;

namespace DataSerialisation
{
    [Serializable]
    public class SerialisableTileMainMaterial
    {
        public string MainMaterialType;
        public string SerialisedData;

        public SerialisableTileMainMaterial(string mainMaterialType, ISerialisableTileMainMaterial iSerialisableTileMainMaterial)
        {
            MainMaterialType = mainMaterialType;
            SerialisedData = JsonUtility.ToJson(iSerialisableTileMainMaterial);
        }
    }
}