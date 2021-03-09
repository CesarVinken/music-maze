﻿using System;
using UnityEngine;

[Serializable]
public class SerialisableTileAttribute
{
    public int TileAttributeId;
    public string AttributeType;
    public string SerialisedData;

    public SerialisableTileAttribute(string attributeType, ISerialisableTileAttribute iSerialisableTileAttribute)
    {
        AttributeType = attributeType;
        SerialisedData = JsonUtility.ToJson(iSerialisableTileAttribute);
    }
}