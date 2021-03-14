using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MazeLevelLoader
{
    public static MazeLevelData LoadMazeLevelData(string mazeName)
    {
        MazeLevelData mazeLevelData = new JsonMazeLevelFileReader().ReadData<MazeLevelData>(mazeName);

        return mazeLevelData;
    }

    public static void LoadMazeLevel(MazeLevelData mazeLevelData)
    {
        //If we are in the editor, first close the editor mode before loading a maze through the consule
        if (EditorManager.InEditor)
        {
            EditorManager.CloseEditor();
        }

        if(PersistentGameManager.CurrentSceneType == SceneType.Overworld)
        {
            Logger.Warning("We are currently in the overworld scene. Do not load maze but return.");
            return;
        }

        // Make checks such as if there are starting locations for the players
        MazeLevelManager.Instance.UnloadLevel();
        MazeLevelManager.Instance.SetupLevel(mazeLevelData); // sets new Level in MazeLevelManager
    }

    public static void LoadMazeLevelForEditor(MazeLevelData mazeLevelData)
    {
        MazeLevelManager.Instance.SetupLevelForEditor(mazeLevelData); // sets up the level without instantiating characters etc.
    }

    public static bool MazeLevelExists(string mazeLevelName)
    {
        string sanatisedMazeName = mazeLevelName.ToLower().Replace(" ", " ");

        string filePath = Path.Combine(Path.Combine(Application.streamingAssetsPath, "maze"), sanatisedMazeName + ".json");

        if (!File.Exists(filePath))
        {
            Logger.Warning(Logger.Datawriting, $"Looked for the maze level '{sanatisedMazeName}' but could not find it");
            Logger.Log($"The available levels are: {GetAllMazeLevelNamesForPrint()}");
            return false;
        }

        return true;
    }

    public static string GetAllMazeLevelNamesForPrint(string printLine = "")
    {
        foreach (string mazeName in Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, "maze"), "*.json"))
        {
            string[] fileNameParts = mazeName.Split('\\');
            string[] fileNameWithoutExtention = fileNameParts[fileNameParts.Length - 1].Split('.');
            printLine += "\n   -" + fileNameWithoutExtention[0];
        }

        return printLine;
    }

    public static MazeLevelNamesData GetAllMazeLevelNamesData()
    {
        MazeLevelNamesData levelNamesData = new JsonMazeLevelListFileReader().ReadData<MazeLevelNamesData>();

        return levelNamesData;
    }

    public static List<string> GetAllPlayableLevelNames()
    {
        List<string> playableLevelNames = new List<string>();
        
        MazeLevelNamesData levelNamesData = new JsonMazeLevelListFileReader().ReadData<MazeLevelNamesData>();

        if (levelNamesData == null) return playableLevelNames;
        for (int i = 0; i < levelNamesData.LevelNames.Count; i++)
        {
            MazeLevelNameData levelNameData = levelNamesData.LevelNames[i];

            if (!levelNameData.IsPlayable) continue;

            if (levelNameData.LevelName == MazeLevelManager.Instance?.Level?.Name) continue;

            playableLevelNames.Add(levelNameData.LevelName);
        }

        if(playableLevelNames.Count == 0)
        {
            Logger.Warning("Found 0 playable levels!");
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
