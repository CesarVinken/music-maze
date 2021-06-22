using System.Collections.Generic;

public class OverworldData
{
    public string Name = "Overworld";

    public List<SerialisableTile> Tiles = new List<SerialisableTile>();
    public List<SerialisableTileArea> TileAreas = new List<SerialisableTileArea>();

    public OverworldData()
    {

    }

    public OverworldData(EditorOverworld overworld)
    {
        for (int i = 0; i < overworld.Tiles.Count; i++)
        {
            SerialisableTile tile = new SerialisableTile(overworld.Tiles[i]);
            Tiles.Add(tile);
        }

        for (int j = 0; j < overworld.TileAreas.Count; j++)
        {
            SerialisableTileArea tileArea = new SerialisableTileArea(overworld.TileAreas[j]);
            TileAreas.Add(tileArea);
        }
    }

    public OverworldData WithName(string overworldName)
    {
        Name = overworldName.ToLower().Replace(" ", "-");
        return this;
    }
}
