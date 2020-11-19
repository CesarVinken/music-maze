using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class JsonMazeLevelFileReader
{
    public MazeLevelData ReadLevelData(string levelName)
    {
        string jsonContent;
        string filePath = "";

        if (Application.platform == RuntimePlatform.Android)
        {
            filePath = Path.Combine(Application.streamingAssetsPath, levelName + ".json");

            UnityWebRequest loadingRequest = UnityWebRequest.Get(filePath);
            loadingRequest.SendWebRequest();
            while (!loadingRequest.isDone);
            jsonContent = loadingRequest.downloadHandler.text.Trim();
        }
        else
        {
            filePath = Path.Combine(Application.streamingAssetsPath, levelName + ".json");

            if (!File.Exists(filePath))
            {
                Logger.Warning($"File {levelName}.json doesn't exist");
                return null;
            }
            jsonContent = File.ReadAllText(filePath);
        }
        MazeLevelData levelData = JsonUtility.FromJson<MazeLevelData>(jsonContent);

        return levelData;
    }
}

