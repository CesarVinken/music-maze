using System.Collections.Generic;
using UnityEngine;

public class Overworld : IGameScene<Tile>
{
    public string Name { get; set; }
    protected  List<Tile> _tiles = new List<Tile>(); 
    public List<Tile> Tiles { get => _tiles; set => _tiles = value; }

    protected List<TileArea> _tileAreas = new List<TileArea>();
    public List<TileArea> TileAreas { get => _tileAreas; set => _tileAreas = value; }

    public List<MazeLevelEntry> MazeEntries = new List<MazeLevelEntry>();

    protected GridLocation _levelBounds = new GridLocation(0, 0);
    public GridLocation LevelBounds { get => _levelBounds; set => _levelBounds = value; }

    protected Dictionary<GridLocation, Tile> _tilesByLocation = new Dictionary<GridLocation, Tile>();
    public Dictionary<GridLocation, Tile> TilesByLocation { get => _tilesByLocation; set => _tilesByLocation = value; }

    private Dictionary<PlayerNumber, CharacterSpawnpoint> _playerCharacterSpawnpoints = new Dictionary<PlayerNumber, CharacterSpawnpoint>();
    public Dictionary<PlayerNumber, CharacterSpawnpoint> PlayerCharacterSpawnpoints { get => _playerCharacterSpawnpoints; set => _playerCharacterSpawnpoints = value; }

    protected GameObject _overworldContainer;
}
