using System;
using System.Collections.Generic;

// Each maze level gets its own json file describing all the tiles and their attributes
[Serializable]
public class MazeLevelData
{
    public string Name;
    //public Vector2 PlayerLocation;
    public List<SerialisableTile> Tiles = new List<SerialisableTile>();
    //public List<DictionaryToJsonItem> GameEventStatuses = new List<DictionaryToJsonItem>();
    //public List<DictionaryToJsonItem> DialogueSeriesPassed = new List<DictionaryToJsonItem>();
    //public List<DictionaryToJsonItem> InteractableObjectsInScene = new List<DictionaryToJsonItem>();

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
