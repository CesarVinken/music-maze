using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class JsonOverworldListFileReader
{
    public OverworldNamesData ReadOverworldList()
    {
        string fileContent;
        string filePath = Path.Combine(Application.streamingAssetsPath, "overworld", "overworlds.json");

        if (Application.platform == RuntimePlatform.Android)
        {

            UnityWebRequest loadingRequest = UnityWebRequest.Get(filePath);
            loadingRequest.SendWebRequest();
            while (!loadingRequest.isDone) ;
            fileContent = loadingRequest.downloadHandler.text.Trim();
        }
        else
        {

            if (!File.Exists(filePath))
            {
                Logger.Warning("File doesn't exist. Creating a new overworlds.json file.");
                File.Create(filePath).Dispose();

                return null;
            }
            fileContent = File.ReadAllText(filePath);
        }

        OverworldNamesData jsonFileContent = JsonUtility.FromJson<OverworldNamesData>(fileContent);
        return jsonFileContent;
    }
}
