using System.Collections.Generic;
using UnityEngine;

public class TileAreaActionHandler : MonoBehaviour
{
    public static TileAreaActionHandler Instance;

    public GameObject TileAreaEntryPrefab;

    public Transform TileAreaContainer;
    public List<EditorTileAreaEntry> TileAreaEntries = new List<EditorTileAreaEntry>();

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
    }
}
