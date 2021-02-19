using System.Collections.Generic;
using UnityEngine;

public class Overworld : IGameScene
{
    public string Name { get; set; }

    protected GridLocation _levelBounds = new GridLocation(0, 0);
    public GridLocation LevelBounds { get => _levelBounds; set => _levelBounds = value; }

    protected Dictionary<GridLocation, Tile> _tilesByLocation = new Dictionary<GridLocation, Tile>();
    public Dictionary<GridLocation, Tile> TilesByLocation { get => _tilesByLocation; set => _tilesByLocation = value; }

    protected GameObject _overworldContainer;

}
