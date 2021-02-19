using System.Collections.Generic;
using UnityEngine;

public class OverworldData
{
    public string Name = "Overworld";

    public List<SerialisableTile> Tiles = new List<SerialisableTile>();

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
    }

    public OverworldData WithName(string overworldName)
    {
        Name = overworldName.ToLower().Replace(" ", "-");
        return this;
    }
}
