using System;
using System.Collections.Generic;

public class TileArea
{
    public string Name { get; private set; }
    public string Id { get; private set; }
    public List<Tile> Tiles = new List<Tile>();

    public TileArea(string name)
    {
        Name = name;
        Id = Guid.NewGuid().ToString();
    }

    public void UpdateName(string name)
    {
        Name = name;
    }
}
