using Character;
using System.Collections.Generic;

public interface IGameScene<T> where T : Tile
{
    string Name { get; set; }
    GridLocation LevelBounds { get; set; }

    List<T> Tiles { get; set; }
    Dictionary<string, TileArea> TileAreas { get; set; } // id, tileArea
    Dictionary<GridLocation, Tile> TilesByLocation { get; set; }

    Dictionary<PlayerNumber, CharacterSpawnpoint> PlayerCharacterSpawnpoints { get; set; }
}
