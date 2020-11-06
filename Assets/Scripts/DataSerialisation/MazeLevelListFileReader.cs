using System.IO;
using UnityEngine;

public class MazeLevelListFileReader
{
    public string ReadMazeLevelList()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "levels.txt");
        //#if UNITY_EDITOR
        //filePath = "/StreamingAssets/" + arguments[1] + ".json";
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Dispose();
            Logger.Warning("File doesn't exist");

            return "";
        }

        string fileContent = File.ReadAllText(filePath);

        return fileContent;
    }
}
