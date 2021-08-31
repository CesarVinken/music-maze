using Character;
using System.Linq;

namespace Gameplay
{
    public class PlayerMarksTileEventHandler : IGameplayEventHandler
    {
        private MazeLevelGameplayManager _mazeLevelGameplayManager;

        public PlayerMarksTileEventHandler(MazeLevelGameplayManager mazeLevelGameplayManager)
        {
            _mazeLevelGameplayManager = mazeLevelGameplayManager;
        }

        public void Handle(object[] data)
        {
            GridLocation tileLocation = new GridLocation((int)data[0], (int)data[1]);
            PlayerNumber playerNumber = (PlayerNumber)data[2];

            InGameMazeTile tile = _mazeLevelGameplayManager.Level.TilesByLocation[tileLocation] as InGameMazeTile;

            MazeTilePath mazeTilePath = (MazeTilePath)tile.GetBackgrounds().FirstOrDefault(background => background is MazeTilePath);
            if (mazeTilePath == null) return;

            PlayerMark playerMark = new PlayerMark(mazeTilePath.ConnectionScore);

            _mazeLevelGameplayManager.HandlePlayerMarkerSprite(tile, playerNumber, playerMark);
            _mazeLevelGameplayManager.HandlePlayerTileMarkerEnds(tile);
            _mazeLevelGameplayManager.HandleNumberOfUnmarkedTiles();

            tile.ResetPlayerMarkEndsRenderer();

            tile.TriggerTransformations();
        }
    }
}