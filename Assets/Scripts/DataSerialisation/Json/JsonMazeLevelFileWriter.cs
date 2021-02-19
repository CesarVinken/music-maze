using System.IO;
using UnityEngine;

public class JsonMazeLevelFileWriter
{
    private MazeLevelData _levelData;
    private string _path;

    public void SerialiseData(MazeLevelData levelData)
    {
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "StreamingAssets", "maze"));
        
        _levelData = levelData;
        _path = Path.Combine(Application.dataPath, "StreamingAssets", "overworld/", _levelData.Name + ".json");

        string jsonDataString = JsonUtility.ToJson(_levelData, true).ToString();

        File.WriteAllText(_path, jsonDataString);
    }
}
