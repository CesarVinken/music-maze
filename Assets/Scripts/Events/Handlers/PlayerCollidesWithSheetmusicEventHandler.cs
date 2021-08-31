using Character;
using System.Linq;

namespace Gameplay
{
    public class PlayerCollidesWithSheetmusicEventHandler : IGameplayEventHandler
    {
        private MazeLevelGameplayManager _mazeLevelGameplayManager;

        public PlayerCollidesWithSheetmusicEventHandler(MazeLevelGameplayManager mazeLevelGameplayManager)
        {
            _mazeLevelGameplayManager = mazeLevelGameplayManager;
        }

        public void Handle(object[] data)
        {
            GridLocation tileLocation = new GridLocation((int)data[0], (int)data[1]);
            PlayerNumber playerNumber = (PlayerNumber)data[2];

            InGameMazeTile tile = _mazeLevelGameplayManager.Level.TilesByLocation[tileLocation] as InGameMazeTile;

            Sheetmusic sheetmusic = (Sheetmusic)tile.GetAttributes().FirstOrDefault(attribute => attribute is Sheetmusic);
            if (sheetmusic == null)
            {
                Logger.Error("Could not find sheetmusic");
            }

            MazePlayerCharacter player = GameManager.Instance.CharacterManager.GetPlayers<MazePlayerCharacter>()[playerNumber];
            sheetmusic.PlayerCollisionOnTile(player);
        }
    }
}