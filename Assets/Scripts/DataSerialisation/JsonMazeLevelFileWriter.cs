using System.IO;
using UnityEngine;

public class JsonMazeLevelFileWriter
{
    private MazeLevelData _levelData;
    private string _path;

    public void SerialiseData(MazeLevelData levelData)
    {
        _levelData = levelData;
        //_path = Path.Combine(Application.persistentDataPath, "mazes", levelData.Name + ".json");
        _path = Path.Combine(Application.dataPath, "StreamingAssets", levelData.Name + ".json");

        string jsonDataString = JsonUtility.ToJson(_levelData, true).ToString();

        File.WriteAllText(_path, jsonDataString);
    }
}

public class JsonMazeLevelFileReader
{
    public MazeLevelData LoadLevel(string levelName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, levelName + ".json");
        //#if UNITY_EDITOR
        //filePath = "/StreamingAssets/" + arguments[1] + ".json";
        if (!File.Exists(filePath))
        {
            Logger.Warning("File doesn't exist");
            return null;

        }
        //#endif
        //#if UNITY_ANDROID
        //    filePath = "jar:file:" + Application.dataPath + "!/assets/" + arguments[1] + ".json";
        //#endif
        string jsonContent = File.ReadAllText(filePath);
        MazeLevelData levelData = JsonUtility.FromJson<MazeLevelData>(jsonContent);

        return levelData;
    }
}