using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSpaceOverworldEditorElements : MonoBehaviour
{
    public static ScreenSpaceOverworldEditorElements Instance;

    [SerializeField] private GameObject _mazeEntryNameContainer;
    private List<EditorMazeLevelEntryName> _mazeEntryNames = new List<EditorMazeLevelEntryName>();

    [SerializeField] private GameObject _editorMazeLevelEntryNamePrefab;

    private void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(_mazeEntryNameContainer, "_mazeEntryNameContainer", gameObject);
        Guard.CheckIsNull(_editorMazeLevelEntryNamePrefab, "_editorMazeLevelEntryNamePrefab", gameObject);
    }

    public void InstantiateMazeLevelEntryName(MazeLevelEntry mazeLevelEntry)
    {
        GameObject mazeLevelEntryGO = Instantiate(_editorMazeLevelEntryNamePrefab, _mazeEntryNameContainer.transform);
        mazeLevelEntryGO.transform.position = new Vector2(mazeLevelEntry.Tile.transform.position.x + 1.2f, mazeLevelEntry.Tile.transform.position.y + 0.5f);
        EditorMazeLevelEntryName editorMazeLevelEntryName = mazeLevelEntryGO.GetComponent<EditorMazeLevelEntryName>();

        if(editorMazeLevelEntryName == null)
        {
            Logger.Error("Could not find EditorMazeLevelEntryName script on game object");
        }

        _mazeEntryNames.Add(editorMazeLevelEntryName);
        editorMazeLevelEntryName.SetMazeLevelEntry(mazeLevelEntry);
    }

    public void UpdateMazeLevelEntryName(MazeLevelEntry mazeLevelEntry)
    {
        for (int i = 0; i < _mazeEntryNames.Count; i++)
        {
            EditorMazeLevelEntryName editorMazeLevelEntryName = _mazeEntryNames[i];
            if (mazeLevelEntry.Tile.GridLocation.X == editorMazeLevelEntryName.MazeLevelEntry.Tile.GridLocation.X &&
                mazeLevelEntry.Tile.GridLocation.Y == editorMazeLevelEntryName.MazeLevelEntry.Tile.GridLocation.Y)
            {
                editorMazeLevelEntryName.SetText(mazeLevelEntry.MazeLevelName);
                break;
            }
        }
    }

    public void RemoveMazeLevelEntryName(MazeLevelEntry mazeLevelEntry)
    {
        for (int i = 0; i < _mazeEntryNames.Count; i++)
        {
            EditorMazeLevelEntryName editorMazeLevelEntryName = _mazeEntryNames[i];
            if(mazeLevelEntry.Tile.GridLocation.X == editorMazeLevelEntryName.MazeLevelEntry.Tile.GridLocation.X &&
                mazeLevelEntry.Tile.GridLocation.Y == editorMazeLevelEntryName.MazeLevelEntry.Tile.GridLocation.Y)
            {
                _mazeEntryNames.Remove(editorMazeLevelEntryName);
                editorMazeLevelEntryName.Destroy();
                break;
            }
        }
    }
}
