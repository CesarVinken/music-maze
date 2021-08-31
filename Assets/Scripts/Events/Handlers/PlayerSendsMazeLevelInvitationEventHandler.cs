
using DataSerialisation;
using UI;

namespace Gameplay
{
    public class PlayerSendsMazeLevelInvitationEventHandler : IGameplayEventHandler
    {
        private OverworldGameplayManager _overworldGameplayManager;

        public PlayerSendsMazeLevelInvitationEventHandler(OverworldGameplayManager overworldGameplayManager)
        {
            _overworldGameplayManager = overworldGameplayManager;
        }

        public void Handle(object[] data)
        {
            string invitorName = (string)data[0];
            string mazeName = (string)data[1];

            Logger.Log($"received event for invitation from {invitorName}");

            MazeLevelInvitation.PendingInvitation = true;

            OverworldMainScreenOverlayCanvas.Instance.ShowMazeInvitation(invitorName, mazeName);

            // check if the player has the level. If not, reject the invitation and inform the player
            if (!MazeLevelNamesData.LevelNameExists(mazeName))
            {
                OverworldMainScreenOverlayCanvas.Instance.ShowPlayerWarning($"We rejected the invitation to play the maze level '{mazeName}' because it was not found!");
                if (MazeLevelInvitation.Instance != null)
                {
                    MazeLevelInvitation.Instance.Reject(ReasonForRejection.LevelNotFound);
                }
                else
                {
                    Logger.Error("This means that we rejected the invitation before even loading it. Revise code.");
                }
                return;
            }
        }
    }
}