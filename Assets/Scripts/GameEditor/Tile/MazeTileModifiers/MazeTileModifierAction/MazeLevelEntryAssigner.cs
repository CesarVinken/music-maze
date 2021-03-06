using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MazeLevelEntryAssigner : MonoBehaviour
{
    public static MazeLevelEntryAssigner Instance;

    [SerializeField] private InputField _mazeLevelNameInputField;
    private void Awake()
    {
        Instance = this;
    }

    public static void AssignMazeLevelEntry()
    {
        Instance.SetMazeLevelEntryName();
    }

    private void SetMazeLevelEntryName()
    {
        if (_mazeLevelNameInputField.text == "") return;

        GridLocation selectedLocation = EditorTileSelector.Instance.CurrentSelectedLocation;
        MazeLevelEntry MazeLevelEntry = null;
        
        for (int i = 0; i < OverworldManager.Instance.EditorOverworld.MazeEntries.Count; i++)
        {
            MazeLevelEntry m = OverworldManager.Instance.EditorOverworld.MazeEntries[i];
            if (m.Tile.GridLocation.X == selectedLocation.X && m.Tile.GridLocation.Y == selectedLocation.Y)
            {
                MazeLevelEntry = m;
                break;
            }
        }
        if(MazeLevelEntry == null)
        {
            Logger.Warning($"Could not find a maze entry tile at location {selectedLocation.X},{selectedLocation.Y}");
            return;
        }

        MazeLevelEntry.MazeLevelName = _mazeLevelNameInputField.text;
        ScreenSpaceOverworldEditorElements.Instance.UpdateMazeLevelEntryName(MazeLevelEntry);

        Logger.Log($"This tile now connects to the maze '{MazeLevelEntry.MazeLevelName}'");
    }

}
