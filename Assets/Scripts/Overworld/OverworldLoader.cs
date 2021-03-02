using System.IO;
using UnityEngine;

public class OverworldLoader : MonoBehaviour
{
    public static OverworldData LoadOverworldData(string overworldName)
    {
        OverworldData overworldData = new JsonOverworldFileReader().ReadData<OverworldData>(overworldName);

        return overworldData;
    }

    public static void LoadOverworld(OverworldData overworldData)
    {
        //If we are in the editor, first close the editor mode before loading an overworld through the consule
        if (EditorManager.InEditor)
        {
            EditorManager.CloseEditor();
        }

        if (GameManager.CurrentSceneType == SceneType.Maze)
        {
            Logger.Warning("We are currently in the maze scene. Do not load overworld but return.");
            return;
        }

        // Make checks such as if there are starting locations for the players
        OverworldManager.Instance.UnloadOverworld();
        OverworldManager.Instance.SetupOverworld(overworldData); // sets new Overworld in OverworldManager
    }

    public static void LoadOverworldForEditor(OverworldData overworldData)
    {
        OverworldManager.Instance.UnloadOverworld();
        OverworldManager.Instance.SetupOverworldForEditor(overworldData); // sets up the level without instantiating characters etc.
    }

    public static bool OverworldExists(string overworldName)
    {
        string sanatisedOverworldName = overworldName.ToLower().Replace(" ", " ");

        string filePath = Path.Combine(Application.streamingAssetsPath, "overworld", sanatisedOverworldName + ".json");

        if (!File.Exists(filePath))
        {
            Logger.Warning(Logger.Datawriting, $"Looked for the overworld '{sanatisedOverworldName}' but could not find it");
            Logger.Log($"The available overworlds are: {GetAllOverworldNamesForPrint()}");
            return false;
        }

        return true;
    }

    public static string GetAllOverworldNamesForPrint(string printLine = "")
    {
        foreach (string overworldName in Directory.GetFiles(Application.streamingAssetsPath + "overworld/", "*.json"))
        {
            string[] fileNameParts = overworldName.Split('\\');
            string[] fileNameWithoutExtention = fileNameParts[fileNameParts.Length - 1].Split('.');
            printLine += "\n   -" + fileNameWithoutExtention[0];
        }

        return printLine;
    }
}
