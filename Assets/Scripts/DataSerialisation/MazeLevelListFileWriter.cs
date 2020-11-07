﻿using System.IO;
using UnityEngine;

public class JsonMazeLevelListFileWriter
{
    private LevelNamesData _levelNamesData;

    private string _path;

    public void SerialiseData(LevelNamesData levelNamesData)
    {
        _levelNamesData = levelNamesData;
        _path = Path.Combine(Application.dataPath, "StreamingAssets", "levels.json");

        string jsonLevelNamesData = JsonUtility.ToJson(_levelNamesData, true).ToString();

        File.WriteAllText(_path, jsonLevelNamesData);
    }
}
