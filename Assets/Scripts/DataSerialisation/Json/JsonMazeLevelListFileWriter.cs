﻿using System.IO;
using UnityEngine;

public class JsonMazeLevelListFileWriter
{
    private MazeLevelNamesData _levelNamesData;

    private string _path;

    public void SerialiseData(MazeLevelNamesData levelNamesData)
    {
        _levelNamesData = levelNamesData;
        _path = Path.Combine(Application.dataPath, "StreamingAssets", "maze", "levels.json");

        string jsonLevelNamesData = JsonUtility.ToJson(_levelNamesData, true).ToString();

        File.WriteAllText(_path, jsonLevelNamesData);
        Logger.Log(Logger.Datawriting, "Wrote level list to file");
    }
}
