using System;
using System.Collections.Generic;

// Each maze level gets its own json file describing all the tiles and their attributes
[Serializable]
public class MazeLevelData
{
    public string Name;
    public List<SerialisableTile> Tiles = new List<SerialisableTile>();

    public MazeLevelData(MazeLevel level)
    {
        for (int i = 0; i < level.Tiles.Count; i++)
        {
            SerialisableTile tile = new SerialisableTile(level.Tiles[i]);
            Tiles.Add(tile);
        }
    }

    public MazeLevelData WithName(string name)
    {
        Name = name;
        return this;
    }
}
