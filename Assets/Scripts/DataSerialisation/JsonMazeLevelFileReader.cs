using System.IO;
using UnityEngine;

public class JsonMazeLevelFileReader
{
    public MazeLevelData ReadLevelData(string levelName)
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
