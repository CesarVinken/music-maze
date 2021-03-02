using System.IO;
using UnityEngine;

public class JsonOverworldListFileWriter : IJsonFileWriter
{
    private OverworldNamesData _levelNamesData;

    private string _path;

    public void SerialiseData<T>(T levelNamesData)
    {
        _levelNamesData = levelNamesData as OverworldNamesData;
        _path = Path.Combine(Application.dataPath, "StreamingAssets", "maze", "levels.json");

        string jsonLevelNamesData = JsonUtility.ToJson(_levelNamesData, true).ToString();

        File.WriteAllText(_path, jsonLevelNamesData);
    }
}
