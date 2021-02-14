using UnityEngine;

public class OverworldLoader : MonoBehaviour
{
    public static OverworldData LoadOverworldData(string mazeName)
    {
        Logger.Log("TODO: implement loading of data"); return null;
        //JsonMazeLevelFileReader levelReader = new JsonMazeLevelFileReader();
        //MazeLevelData mazeLevelData = levelReader.ReadLevelData(mazeName);

        //return mazeLevelData;
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
}
