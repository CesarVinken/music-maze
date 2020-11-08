using System.IO;
using UnityEngine;

public class JsonMazeLevelListFileReader
{
    public LevelNamesData ReadMazeLevelList()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "levels.json");
        //#if UNITY_EDITOR
        //filePath = "/StreamingAssets/" + arguments[1] + ".json";
        if (!File.Exists(filePath))
        {
            Logger.Warning("File doesn't exist. Creating a new levels.json file.");
            File.Create(filePath).Dispose();

            return null;
        }

        string fileContent = File.ReadAllText(filePath);
        LevelNamesData jsonFileContent = JsonUtility.FromJson<LevelNamesData>(fileContent);
        return jsonFileContent;
    }
}
