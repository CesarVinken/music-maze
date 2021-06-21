using System.Collections.Generic;
using UnityEngine;

public class TileArea
{
    public string Name { get; private set; }
    public List<Tile> Tiles = new List<Tile>();

    public TileArea(string name)
    {
        Name = name;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }

}
