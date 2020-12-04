using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MazeLevelLoader
{
    public static MazeLevelData LoadMazeLevelData(string mazeName)
    {
        JsonMazeLevelFileReader levelReader = new JsonMazeLevelFileReader();
        MazeLevelData mazeLevelData = levelReader.ReadLevelData(mazeName);

        return mazeLevelData;
    }

    public static void LoadMazeLevel(MazeLevelData mazeLevelData)
    {
        //If we are in the editor, first close the editor mode before loading a maze through the consule
        if (EditorManager.InEditor)
        {
            EditorManager.CloseEditor();
        }

        // Make checks such as if there are starting locations for the players
        MazeLevelManager.Instance.UnloadLevel();
        MazeLevelManager.Instance.SetupLevel(mazeLevelData); // sets new Level in MazeLevelManager
    }

    public static void LoadMazelLevelForEditor(MazeLevelData mazeLevelData)
    {
        MazeLevelManager.Instance.UnloadLevel();
        MazeLevelManager.Instance.SetupLevelForEditor(mazeLevelData); // sets up the level without instantiating characters etc.
    }

    public static bool MazeLevelExists(string mazeLevelName)
    {
        string sanatisedMazeName = mazeLevelName.ToLower().Replace(" ", " ");

        string filePath = Path.Combine(Application.streamingAssetsPath, sanatisedMazeName + ".json");

        if (!File.Exists(filePath))
        {
            Logger.Warning(Logger.Datawriting, $"Looked for the maze level '{sanatisedMazeName}' but could not find it");
            Logger.Log($"The available levels are: {GetAllLevelNamesForPrint()}");
            return false;
        }

        return true;
    }

    public static string GetAllLevelNamesForPrint(string printLine = "")
    {
        foreach (string mazeName in Directory.GetFiles(Application.streamingAssetsPath, "*.json"))
        {
            string[] fileNameParts = mazeName.Split('\\');
            string[] fileNameWithoutExtention = fileNameParts[fileNameParts.Length - 1].Split('.');
            printLine += "\n   -" + fileNameWithoutExtention[0];
        }

        return printLine;
    }

    public static MazeLevelNamesData GetAllLevelNamesData()
    {
        JsonMazeLevelListFileReader jsonMazeLevelListFileReader = new JsonMazeLevelListFileReader();
        MazeLevelNamesData levelNamesData = jsonMazeLevelListFileReader.ReadMazeLevelList();

        return levelNamesData;
    }

    public static List<string> GetAllPlayableLevelNames()
    {
        List<string> playableLevelNames = new List<string>();

        JsonMazeLevelListFileReader jsonMazeLevelListFileReader = new JsonMazeLevelListFileReader();
        MazeLevelNamesData levelNamesData = jsonMazeLevelListFileReader.ReadMazeLevelList();

        if (levelNamesData == null) return playableLevelNames;

        for (int i = 0; i < levelNamesData.LevelNames.Count; i++)
        {
            MazeLevelNameData levelNameData = levelNamesData.LevelNames[i];

            if (!levelNameData.IsPlayable) continue;

            if (levelNameData.LevelName == MazeLevelManager.Instance.Level.MazeName) continue;

            playableLevelNames.Add(levelNameData.LevelName);
        }

        return playableLevelNames;
    }

    public static void ReplaceMazeLevel(string sourceLevelName, string destinationLevelName)
    {
        string sourceLevelFilePath = Path.Combine(Application.streamingAssetsPath, sourceLevelName + ".json");
        string destinationLevelFilePath = Path.Combine(Application.streamingAssetsPath, destinationLevelName + ".json");
        File.Copy(sourceLevelFilePath, destinationLevelFilePath, true);
    }
}
