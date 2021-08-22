using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace DataSerialisation
{
    public class JsonMazeLevelFileReader : IJsonFileReader
    {
        public MazeLevelData ReadData<MazeLevelData>(string levelName)
        {
            string jsonContent;
            string filePath = "";
            string dashedMazeName = levelName.ToLower().Replace(" ", "-");

            if (Application.platform == RuntimePlatform.Android)
            {
                filePath = Path.Combine(Application.streamingAssetsPath, "maze", dashedMazeName + ".json");

                UnityWebRequest loadingRequest = UnityWebRequest.Get(filePath);
                loadingRequest.SendWebRequest();
                while (!loadingRequest.isDone) ;
                jsonContent = loadingRequest.downloadHandler.text.Trim();
            }
            else
            {
                filePath = Path.Combine(Application.streamingAssetsPath, "maze", dashedMazeName + ".json");

                if (!File.Exists(filePath))
                {
                    Logger.Warning($"File {dashedMazeName}.json doesn't exist");
                    return default;
                }
                jsonContent = File.ReadAllText(filePath);
            }
            MazeLevelData levelData = JsonUtility.FromJson<MazeLevelData>(jsonContent);

            return levelData;
        }
    }
}