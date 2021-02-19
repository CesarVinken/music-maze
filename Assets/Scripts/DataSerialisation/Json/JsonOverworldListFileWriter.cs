using System.IO;
using UnityEngine;

public class JsonOverworldListFileWriter
{
    private OverworldNamesData _levelNamesData;

    private string _path;

    public void SerialiseData(OverworldNamesData levelNamesData)
    {
        _levelNamesData = levelNamesData;
        _path = Path.Combine(Application.dataPath, "StreamingAssets", "maze", "levels.json");

        string jsonLevelNamesData = JsonUtility.ToJson(_levelNamesData, true).ToString();

        File.WriteAllText(_path, jsonLevelNamesData);
    }
}
