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

    public void LoadExistingTileAreas()
    {
        List<TileArea> existingTileAreas = GameManager.Instance.CurrentEditorLevel.TileAreas;

        for (int i = 0; i < existingTileAreas.Count; i++)
        {
            GameObject tileAreaEntryGO = GameObject.Instantiate(TileAreaEntryPrefab, TileAreaContainer);
            EditorTileAreaEntry tileAreaEntry = tileAreaEntryGO.GetComponent<EditorTileAreaEntry>();
            tileAreaEntry.SetName(existingTileAreas[i].Name);

            TileAreaEntries.Add(tileAreaEntry);
        }
    }

    public void CreateNewTileAreaEntry()
    {
        GameObject tileAreaEntryGO = GameObject.Instantiate(TileAreaEntryPrefab, TileAreaContainer);
        EditorTileAreaEntry tileAreaEntry = tileAreaEntryGO.GetComponent<EditorTileAreaEntry>().WithTileAreaComponent();

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
    }

    public void DeselectTileAreaEntry(EditorTileAreaEntry tileAreaEntry)
    {
        if(SelectedTileAreaEntry != null && SelectedTileAreaEntry == tileAreaEntry)
        {
            SelectedTileAreaEntry = null;
            tileAreaEntry.Deselect();
        }
    }
}
