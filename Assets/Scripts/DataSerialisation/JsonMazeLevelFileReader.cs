using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class JsonMazeLevelFileReader
{
    public MazeLevelData ReadLevelData(string levelName)
    {
        string jsonContent;

        if (Application.platform == RuntimePlatform.Android)
        {
            string sFilePath = Path.Combine(Application.streamingAssetsPath, levelName + ".json");

            UnityWebRequest loadingRequest = UnityWebRequest.Get(sFilePath);
            loadingRequest.SendWebRequest();
            while (!loadingRequest.isDone);
            jsonContent = loadingRequest.downloadHandler.text.Trim();
        }
        else
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, levelName + ".json");

            if (!File.Exists(filePath))
            {
                Logger.Warning("File doesn't exist");
                return null;
            }
            jsonContent = File.ReadAllText(filePath);
        }
        MazeLevelData levelData = JsonUtility.FromJson<MazeLevelData>(jsonContent);

        return levelData;
    }
}

