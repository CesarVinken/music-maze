
using Character;
using System.Linq;

namespace Gameplay
{
    public class PlayerCollidesWithMusicInstrumentCaseEventHandler : IGameplayEventHandler
    {
        private MazeLevelGameplayManager _mazeLevelGameplayManager;

        public PlayerCollidesWithMusicInstrumentCaseEventHandler(MazeLevelGameplayManager mazeLevelGameplayManager)
        {
            _mazeLevelGameplayManager = mazeLevelGameplayManager;
        }

        public void Handle(object[] data)
        {
            GridLocation tileLocation = new GridLocation((int)data[0], (int)data[1]);
            PlayerNumber playerNumber = (PlayerNumber)data[2];

            InGameMazeTile tile = _mazeLevelGameplayManager.Level.TilesByLocation[tileLocation] as InGameMazeTile;

            MusicInstrumentCase musicInstrumentCase = (MusicInstrumentCase)tile.GetAttributes().FirstOrDefault(attribute => attribute is MusicInstrumentCase);
            if (musicInstrumentCase == null)
            {
                Logger.Error("Could not find musicInstrumentCase");
            }

            MazePlayerCharacter player = GameManager.Instance.CharacterManager.GetPlayers<MazePlayerCharacter>()[playerNumber];
            musicInstrumentCase.PlayerCollisionOnTile(player);
        }
    }
}