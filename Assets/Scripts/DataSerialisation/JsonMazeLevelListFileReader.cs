using System.IO;
using UnityEngine;

public class JsonMazeLevelListFileReader
{
    public MazeLevelNamesData ReadMazeLevelList()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "levels.json");
        //#if UNITY_EDITOR

        if (!File.Exists(filePath))
        {
            Logger.Warning("File doesn't exist. Creating a new levels.json file.");
            File.Create(filePath).Dispose();

            return null;
        }

        string fileContent = File.ReadAllText(filePath);
        MazeLevelNamesData jsonFileContent = JsonUtility.FromJson<MazeLevelNamesData>(fileContent);
        return jsonFileContent;
    }
}
