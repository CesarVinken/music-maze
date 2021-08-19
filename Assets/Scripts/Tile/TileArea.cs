using DataSerialisation;
using System;
using System.Collections.Generic;

public class TileArea
{
    public string Name { get; private set; }
    public string Id { get; private set; }
    public List<Tile> Tiles = new List<Tile>();

    public TileArea(string tileAreaName)
    {
        Name = tileAreaName;
        Id = Guid.NewGuid().ToString();
    }

    public TileArea(SerialisableTileArea serialisableTileArea)
    {
        Name = serialisableTileArea.Name;
        Id = serialisableTileArea.Id;
    }

    public void UpdateName(string tileAreaName)
    {
        Name = tileAreaName;
    }
}
