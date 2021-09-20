using Character;
using DataSerialisation;
using System.Collections.Generic;
using UnityEngine;

public class MazeLevel : IGameScene<Tile>
{
    public string Name { get; set; }
    protected List<Tile> _tiles = new List<Tile>();
    public List<Tile> Tiles { get => _tiles; set => _tiles = value; }

    protected Dictionary<string, TileArea> _tileAreas = new Dictionary<string, TileArea>();
    public Dictionary<string, TileArea> TileAreas { get => _tileAreas; set => _tileAreas = value; }

    protected GridLocation _levelBounds = new GridLocation(0, 0);
    public GridLocation LevelBounds { get => _levelBounds; set => _levelBounds = value; }

    protected Dictionary<GridLocation, Tile> _tilesByLocation = new Dictionary<GridLocation, Tile>();
    public Dictionary<GridLocation, Tile> TilesByLocation { get => _tilesByLocation; set => _tilesByLocation = value; }

    public List<PlayerExit> MazeExits = new List<PlayerExit>();

    public List<FerryRoute> FerryRoutes { get => _ferryRoutes; set => _ferryRoutes = value; }
    private List<FerryRoute> _ferryRoutes = new List<FerryRoute>();

    private Dictionary<PlayerNumber, CharacterSpawnpoint> _playerCharacterSpawnpoints = new Dictionary<PlayerNumber, CharacterSpawnpoint>();
    public Dictionary<PlayerNumber, CharacterSpawnpoint> PlayerCharacterSpawnpoints { get => _playerCharacterSpawnpoints; set => _playerCharacterSpawnpoints = value;}

    public List<CharacterSpawnpoint> EnemyCharacterSpawnpoints = new List<CharacterSpawnpoint>();

    protected GameObject _mazeContainer;

    public virtual void BuildTiles(MazeLevelData mazeLevelData) { }
    protected virtual void InitialiseEditorTileAreas(MazeLevelData mazeLevelData) { }

}

public struct CharacterStartLocation
{
    public GridLocation GridLocation;
    public CharacterBlueprint Character;

    public CharacterStartLocation(GridLocation gridLocation, CharacterBlueprint character)
    {
        GridLocation = gridLocation;
        Character = character;
    }
}