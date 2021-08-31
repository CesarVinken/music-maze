
using System;
using UI;

namespace Gameplay
{
    public class PlayerRejectsMazeLevelInvitationEventHandler : IGameplayEventHandler
    {
        private OverworldGameplayManager _overworldGameplayManager;

        public PlayerRejectsMazeLevelInvitationEventHandler(OverworldGameplayManager overworldGameplayManager)
        {
            _overworldGameplayManager = overworldGameplayManager;
        }

        public void Handle(object[] data)
        {
            string rejectorName = (string)data[0];
            string mazeName = (string)data[1];
            if (!Enum.TryParse((string)data[2], out ReasonForRejection reason))
            {
                reason = ReasonForRejection.PlayerRejected;
            }

            Logger.Log($"received event that {rejectorName} rejected the invitation with the reason {reason}");
            OverworldMainScreenOverlayCanvas.Instance.ShowMazeInvitationRejection(rejectorName, mazeName, reason);
        }
    }
}