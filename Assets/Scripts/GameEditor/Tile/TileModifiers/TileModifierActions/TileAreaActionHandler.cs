using System.Collections.Generic;
using UnityEngine;

public class TileAreaActionHandler : MonoBehaviour
{
    public static TileAreaActionHandler Instance;

    public GameObject TileAreaEntryPrefab;

    public Transform TileAreaContainer;
    public List<EditorTileAreaEntry> TileAreaEntries = new List<EditorTileAreaEntry>();

    public EditorTileAreaEntry SelectedTileAreaEntry = null;

    public void Awake()
    {
        Guard.CheckIsNull(TileAreaEntryPrefab, "TileAreaEntryPrefab");

        Guard.CheckIsNull(TileAreaContainer, "TileAreaContainer", gameObject);

        Instance = this;

        TileAreaEntries.Clear();

        LoadExistingTileAreas();
    }

    public void OnDestroy()
    {
        DeselectTileAreaEntry(SelectedTileAreaEntry);
    }

    public void LoadExistingTileAreas()
    {
        Dictionary<string, TileArea> existingTileAreas = GameManager.Instance.CurrentEditorLevel.TileAreas;

        foreach (KeyValuePair<string, TileArea> item in existingTileAreas)
        {
            GameObject tileAreaEntryGO = GameObject.Instantiate(TileAreaEntryPrefab, TileAreaContainer);
            EditorTileAreaEntry tileAreaEntry = tileAreaEntryGO.GetComponent<EditorTileAreaEntry>().WithTileAreaComponent(item.Value);

            tileAreaEntry.SetName(item.Value.Name);
            TileAreaEntries.Add(tileAreaEntry);
        }
    }

    public void CreateNewTileAreaEntry()
    {
        GameObject tileAreaEntryGO = GameObject.Instantiate(TileAreaEntryPrefab, TileAreaContainer);
        EditorTileAreaEntry tileAreaEntry = tileAreaEntryGO.GetComponent<EditorTileAreaEntry>().WithNewTileAreaComponent();

        TileAreaEntries.Add(tileAreaEntry);

        SelectTileAreaEntry(tileAreaEntry);
    }

    public void SelectTileAreaEntry(EditorTileAreaEntry tileAreaEntry)
    {
        if(SelectedTileAreaEntry != null)
        {
            if(SelectedTileAreaEntry == tileAreaEntry)
            {
                return;
            }
            else
            {
                SelectedTileAreaEntry.Deselect();
                SelectedTileAreaEntry = null;
            }
        }

        SelectedTileAreaEntry = tileAreaEntry;
        tileAreaEntry.Select();

        // show all tiles that are already part of this area with a blue overlay
        for (int i = 0; i < GameManager.Instance.CurrentEditorLevel.Tiles.Count; i++)
        {
            if(PersistentGameManager.CurrentSceneType == SceneType.Maze)
            {
                EditorMazeTile tile = GameManager.Instance.CurrentEditorLevel.Tiles[i] as EditorMazeTile;

                if (tile.GetTileArea(tileAreaEntry.TileArea) != null)
                {
                    tile.SetTileOverlayImage(TileOverlayMode.Blue);
                }
            }
        }   
    }

    public void DeselectTileAreaEntry(EditorTileAreaEntry tileAreaEntry)
    {
        if (SelectedTileAreaEntry != null && SelectedTileAreaEntry == tileAreaEntry)
        {
            SelectedTileAreaEntry = null;
            tileAreaEntry.Deselect();

            for (int i = 0; i < GameManager.Instance.CurrentEditorLevel.Tiles.Count; i++)
            {
                if (PersistentGameManager.CurrentSceneType == SceneType.Maze)
                {
                    EditorMazeTile tile = GameManager.Instance.CurrentEditorLevel.Tiles[i] as EditorMazeTile;
                    tile.SetTileOverlayImage(TileOverlayMode.Empty);
                }    
            }
        }
    }
}
