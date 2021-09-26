using Character;
using System.Linq;

namespace Gameplay
{
    public class PlayerControlsFerryEventHandler : IGameplayEventHandler
    {
        private IGameplayManager _gameplayManager;

        public PlayerControlsFerryEventHandler(IGameplayManager gameplayManager)
        {
            _gameplayManager = gameplayManager;
        }

        public void Handle(object[] data)
        {
            GridLocation tileLocation = new GridLocation((int)data[0], (int)data[1]);
            PlayerNumber playerNumber = (PlayerNumber)data[2];

            bool isControlling = (int)data[3] == 1 ? true : false;

            PlayerCharacter playerCharacter = CharacterHelper.GetUnbiasedPlayerCharacter(playerNumber);
            Ferry ferry = Ferry.Ferries.FirstOrDefault(f => f.CurrentLocationTile.GridLocation.X == tileLocation.X && f.CurrentLocationTile.GridLocation.Y == tileLocation.Y);

            if(ferry == null)
            {
                Logger.Error($"Expected but could not find a ferry on the tie {tileLocation.X}, {tileLocation.Y}");
            }

            playerCharacter.ToggleFerryControlOnOthers(ferry, isControlling);
        }
    }
}