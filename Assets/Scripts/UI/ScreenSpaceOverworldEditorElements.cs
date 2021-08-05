using System.Collections.Generic;
using UnityEngine;

public class ScreenSpaceOverworldEditorElements : MonoBehaviour
{
    public static ScreenSpaceOverworldEditorElements Instance;

    [SerializeField] private GameObject _mazeEntryNameContainer;
    private List<EditorMazeLevelEntryName> _mazeEntryNames = new List<EditorMazeLevelEntryName>();

    [SerializeField] private GameObject _editorMazeLevelEntryNamePrefab;
    [SerializeField] private GameObject _editorIssuePrefab;

    private List<EditorIssueUI> _editorIssues = new List<EditorIssueUI>();

    private void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(_mazeEntryNameContainer, "_mazeEntryNameContainer", gameObject);
        Guard.CheckIsNull(_editorMazeLevelEntryNamePrefab, "_editorMazeLevelEntryNamePrefab", gameObject);
        Guard.CheckIsNull(_editorIssuePrefab, "EditorIssuePrefab", gameObject);
    }

    public void InstantiateMazeLevelEntryName(MazeLevelEntry mazeLevelEntry)
    {
        Vector3 mazeEntryPos = mazeLevelEntry.Tile.transform.position;

        GameObject mazeLevelEntryNameGO = Instantiate(_editorMazeLevelEntryNamePrefab, _mazeEntryNameContainer.transform);

        EditorMazeLevelEntryName editorMazeLevelEntryName = mazeLevelEntryNameGO.GetComponent<EditorMazeLevelEntryName>();
        editorMazeLevelEntryName.SetWorldPosition(new Vector2(mazeEntryPos.x + 1.2f, mazeEntryPos.y + 0.5f));

        if (editorMazeLevelEntryName == null)
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

    public void CreateEditorIssue(GridLocation spawnLocation, string issueDescription, EditorIssueType editorIssueType)
    {
        Logger.Warning("Create editor issue");
        Vector3 spawnLocationVector = GridLocation.GridToVector(spawnLocation);

        GameObject editorIssueGO = GameObject.Instantiate(_editorIssuePrefab, transform);

        EditorIssueUI editorIssue = editorIssueGO.GetComponent<EditorIssueUI>();
        editorIssue.SetGridLocation(spawnLocation);
        editorIssue.SetWorldPosition(new Vector2(spawnLocationVector.x, spawnLocationVector.y - .7f));
        editorIssue.SetDescription(issueDescription);
        editorIssue.SetEditorIssueType(editorIssueType);
        _editorIssues.Add(editorIssue);
    }

    public bool TrySolveEditorIssue(EditorIssueType editorIssueType, GridLocation issueLocation)
    {
        for (int i = 0; i < _editorIssues.Count; i++)
        {
            EditorIssueUI editorIssue = _editorIssues[i];
            if (editorIssue.EditorIssueType != editorIssueType) continue;

            if(editorIssue.EditorIssueGridLocation.X == issueLocation.X && editorIssue.EditorIssueGridLocation.Y == issueLocation.Y)
            {
                _editorIssues.Remove(editorIssue);
                editorIssue.Delete();
                return true;
            }
        }
        return false;
    }

    public void CleanOut()
    {
        for (int i = 0; i < _editorIssues.Count; i++)
        {
            _editorIssues[i].Delete();
        }
        _editorIssues.Clear();

        for (int j = 0; j < _mazeEntryNames.Count; j++)
        {
            _mazeEntryNames[j].Destroy();
        }
        _mazeEntryNames.Clear();
    }
}
