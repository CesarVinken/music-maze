using System;
using UnityEngine;

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
}