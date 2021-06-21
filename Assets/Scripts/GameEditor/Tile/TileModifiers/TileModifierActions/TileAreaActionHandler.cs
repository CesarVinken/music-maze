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
    }

    public void CreateNewTileAreaEntry()
    {
        GameObject tileAreaEntryGO = GameObject.Instantiate(TileAreaEntryPrefab, TileAreaContainer);
        EditorTileAreaEntry tileAreaEntry = tileAreaEntryGO.GetComponent<EditorTileAreaEntry>();

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
