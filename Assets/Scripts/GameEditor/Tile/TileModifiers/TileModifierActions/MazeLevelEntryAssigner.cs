using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class MazeLevelEntryAssigner : MonoBehaviour
{
    public static MazeLevelEntryAssigner Instance;

    [SerializeField] private Dropdown _mazeLevelNamesDropdown;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _mazeLevelNamesDropdown.ClearOptions();

        List<OptionData> options = new List<OptionData>();

        for (int i = 0; i < OverworldGameplayManager.Instance.EditorOverworld.MazeLevelNames.Count; i++)
        {
            string levelName = OverworldGameplayManager.Instance.EditorOverworld.MazeLevelNames[i];
            options.Add(new OptionData(levelName));
        }
        _mazeLevelNamesDropdown.AddOptions(options);
    }

    public static void AssignMazeLevelEntry()
    {
        Instance.SetMazeLevelEntryName();
    }

    private void SetMazeLevelEntryName()
    {
        GridLocation selectedLocation = EditorTileSelector.Instance.CurrentlySelectedTile.GridLocation;
        MazeLevelEntry MazeLevelEntry = null;

        for (int i = 0; i < OverworldGameplayManager.Instance.EditorOverworld.MazeEntries.Count; i++)
        {
            MazeLevelEntry m = OverworldGameplayManager.Instance.EditorOverworld.MazeEntries[i];
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

        MazeLevelEntry.MazeLevelName = GetCurrentDropdownSelection();
        ScreenSpaceOverworldEditorElements.Instance.UpdateMazeLevelEntryName(MazeLevelEntry);

        Logger.Log($"This tile now connects to the maze '{MazeLevelEntry.MazeLevelName}'");
    }

    public string GetCurrentDropdownSelection()
    {
        return _mazeLevelNamesDropdown.options[_mazeLevelNamesDropdown.value].text;
    }
}
