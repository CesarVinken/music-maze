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

    public override MazeLevelEntry PlaceMazeLevelEntry()
    {
        MazeLevelEntry mazeLevelEntry = (MazeLevelEntry)InstantiateTileAttributeGO<MazeLevelEntry>();
        mazeLevelEntry.Tile = Tile;

        Tile.Walkable = true;
        Tile.TileAttributes.Add(mazeLevelEntry);

        OverworldManager.Instance.EditorOverworld.MazeEntries.Add(mazeLevelEntry);

        ScreenSpaceOverworldEditorElements.Instance.InstantiateMazeLevelEntryName(mazeLevelEntry);
        return mazeLevelEntry;
    }
}

