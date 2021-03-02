using System.IO;
using UnityEngine;

public class JsonMazeLevelFileWriter : IJsonFileWriter
{
    private MazeLevelData _levelData;
    private string _path;

    public void SerialiseData<T>(T levelData)
    {
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "StreamingAssets", "maze"));
        
        _levelData = levelData as MazeLevelData;
        _path = Path.Combine(Application.dataPath, "StreamingAssets", "maze", _levelData.Name + ".json");

        string jsonDataString = JsonUtility.ToJson(_levelData, true).ToString();

        File.WriteAllText(_path, jsonDataString);
    }
}
