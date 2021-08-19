using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace DataSerialisation
{
    public class JsonOverworldFileReader : IJsonFileReader
    {
        public OverworldData ReadData<OverworldData>(string overworldName = "overworld")
        {
            string jsonContent;
            string filePath = "";

            if (Application.platform == RuntimePlatform.Android)
            {
                filePath = Path.Combine(Application.streamingAssetsPath, "overworld", overworldName + ".json");

                UnityWebRequest loadingRequest = UnityWebRequest.Get(filePath);
                loadingRequest.SendWebRequest();
                while (!loadingRequest.isDone) ;
                jsonContent = loadingRequest.downloadHandler.text.Trim();
            }
            else
            {
                filePath = Path.Combine(Application.streamingAssetsPath, "overworld", overworldName + ".json");

                if (!File.Exists(filePath))
                {
                    Logger.Warning($"File {overworldName}.json doesn't exist");
                    return default;
                }
                jsonContent = File.ReadAllText(filePath);
            }
            OverworldData overworldData = JsonUtility.FromJson<OverworldData>(jsonContent);

            return overworldData;
        }
    }
}