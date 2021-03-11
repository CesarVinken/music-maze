using UnityEngine;

public class EditorOverworldTileAttributePlacer : OverworldTileAttributePlacer<EditorOverworldTile>
{
    private EditorOverworldTile _tile;

    public override EditorOverworldTile Tile { get => _tile; set => _tile = value; }

    public EditorOverworldTileAttributePlacer(EditorOverworldTile tile)
    {
        Tile = tile;
    }

    public override ITileAttribute InstantiateTileAttributeGO<U>()
    {
        GameObject tileAttributeGO = GameObject.Instantiate(OverworldManager.Instance.GetTileAttributePrefab<U>(), Tile.transform);
        return tileAttributeGO.GetComponent<U>();
    }

    public override MazeLevelEntry PlaceMazeLevelEntry(string mazeLevelName = "")
    {
        MazeLevelEntry mazeLevelEntry = (MazeLevelEntry)InstantiateTileAttributeGO<MazeLevelEntry>();
        mazeLevelEntry.Tile = Tile;

        // When we place the entry from a LoadOverworld situation, we already have a maze level name from the file data.
        if(mazeLevelName == "")
        {
            mazeLevelName = MazeLevelEntryAssigner.Instance.GetCurentDropdownSelection();
            Logger.Warning(mazeLevelName);
        }

        mazeLevelEntry.MazeLevelName = mazeLevelName;

        Tile.Walkable = true;
        Tile.AddAttribute(mazeLevelEntry);

        OverworldManager.Instance.EditorOverworld.MazeEntries.Add(mazeLevelEntry);

        ScreenSpaceOverworldEditorElements.Instance.InstantiateMazeLevelEntryName(mazeLevelEntry);
        return mazeLevelEntry;
    }
}

