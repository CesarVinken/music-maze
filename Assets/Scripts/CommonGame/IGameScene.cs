using System.Collections.Generic;

public interface IGameScene
{
    string Name { get; set; }
    GridLocation LevelBounds { get; set; }
    Dictionary<GridLocation, Tile> TilesByLocation { get; set; }
}
