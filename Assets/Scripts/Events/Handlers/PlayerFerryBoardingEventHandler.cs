using Character;
using System.Linq;

namespace Gameplay
{
    public class PlayerFerryBoardingEventHandler : IGameplayEventHandler
    {
        private IGameplayManager _gameplayManager;

        public PlayerFerryBoardingEventHandler(IGameplayManager gameplayManager)
        {
            _gameplayManager = gameplayManager;
        }

        public void Handle(object[] data)
        {
            string ferryRouteId = (string)data[0];
            PlayerNumber playerNumber = (PlayerNumber)data[1];

            bool isBoarding = (int)data[2] == 1 ? true : false;

            PlayerCharacter playerCharacter = CharacterHelper.GetUnbiasedPlayerCharacter(playerNumber);
            Ferry ferry = Ferry.Ferries.FirstOrDefault(f => f.FerryRoute.Id.Equals(ferryRouteId));

            if (ferry == null)
            {
                Logger.Error($"Expected but could not find a ferry with the ferry route id {ferryRouteId}");
            }

            if (isBoarding)
            {
                ferry.AddPlayerOnFerry(playerCharacter);
            }
            else
            {
                ferry.RemovePlayerOnFerry(playerCharacter);
            }
        }
    }
}