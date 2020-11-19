using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class JsonMazeLevelListFileReader
{
    public MazeLevelNamesData ReadMazeLevelList()
    {
        string fileContent;
        string filePath = Path.Combine(Application.streamingAssetsPath, "levels.json");

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
                Logger.Warning("File doesn't exist. Creating a new levels.json file.");
                File.Create(filePath).Dispose();

                return null;
            }
            fileContent = File.ReadAllText(filePath);
        }

        MazeLevelNamesData jsonFileContent = JsonUtility.FromJson<MazeLevelNamesData>(fileContent);
        return jsonFileContent;
    }
}
