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
            string ferryRouteId = (string)data[0];
            GridLocation playerTileLocation = new GridLocation((int)data[1], (int)data[2]);
            PlayerNumber playerNumber = (PlayerNumber)data[3];

            bool isControlling = (int)data[4] == 1 ? true : false;

            PlayerCharacter playerCharacter = CharacterHelper.GetUnbiasedPlayerCharacter(playerNumber);
            Ferry ferry = Ferry.Ferries.FirstOrDefault(f => f.FerryRoute.Id.Equals(ferryRouteId));
            if (ferry != null && !isControlling)
            {
                Logger.Log($"player {playerCharacter.Name} stopped controlling while on tile {playerTileLocation.X} {playerTileLocation.Y}");
                ferry.SetNewCurrentLocation(ferry.FerryRoute.GetFerryRoutePointByLocation(playerTileLocation));
            }
            //if(ferry == null)
            //{

            //    for (int i = 0; i < Ferry.Ferries.Count; i++)
            //    {
            //        Logger.Log($"Found location of ferry: {Ferry.Ferries[i].CurrentLocationTile.GridLocation.X}, {Ferry.Ferries[i].CurrentLocationTile.GridLocation.Y}. ControllingPlayerCharacter? {Ferry.Ferries[i].ControllingPlayerCharacter != null}");
            //    }
            //    //Logger.Error($"Expected but could not find a ferry on the tile {tileLocation.X}, {tileLocation.Y}");
            //}

            playerCharacter.ToggleFerryControlOnOthers(ferry, isControlling);
        }
    }
}