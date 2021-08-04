using System.IO;
using UnityEngine;

public class JsonMazeLevelListFileWriter : IJsonFileWriter
{
    private MazeLevelNamesData _levelNamesData;

    private string _path;

    public void SerialiseData<T>(T levelNamesData)
    {
        _levelNamesData = levelNamesData as MazeLevelNamesData;
        _path = Path.Combine(Application.dataPath, "StreamingAssets", "maze", "levels.json");

        string jsonLevelNamesData = JsonUtility.ToJson(_levelNamesData, true).ToString();

        File.WriteAllText(_path, jsonLevelNamesData);
        Logger.Log(Logger.Datawriting, "Wrote maze level list to file");
    }
}
