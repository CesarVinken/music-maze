using DataSerialisation;
using Photon.Pun;
using UI;
using UnityEngine;

namespace Character
{
    public class OverworldPlayerCharacter : PlayerCharacter
    {
        public MazeLevelEntry OccupiedMazeLevelEntry = null;

        public override void Awake()
        {
            base.Awake();

            OverworldScoreContainer.Instance.UpdateScoreLabel(PlayerNumber);
        }

        public override void Start()
        {
            base.Start();

            // transform the player's starting tile and surrounding tiles
            InGameOverworldTile currentTile = GameManager.Instance.CurrentGameLevel.TilesByLocation[StartingPosition] as InGameOverworldTile;
            SetCurrentGridLocation(currentTile.GridLocation);
        }

        public override void Update()
        {
            if (MazeLevelInvitation.PendingInvitation)
                return;

            base.Update();

            if (OccupiedMazeLevelEntry == null) return;
            if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && !PhotonView.IsMine) return;

            if ((GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
                (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && PhotonView.IsMine)))
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    if (OccupiedMazeLevelEntry.Tile.GridLocation.X == CurrentGridLocation.X && OccupiedMazeLevelEntry.Tile.GridLocation.Y == CurrentGridLocation.Y)
                    {
                        PerformMazeLevelEntryAction(OccupiedMazeLevelEntry.MazeLevelName);
                    }
                }
            }
            else if(GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
            {
                if (PlayerNumber == PlayerNumber.Player1 && Input.GetKeyDown(GameManager.Instance.KeyboardConfiguration.Player1Action) ||
                    PlayerNumber == PlayerNumber.Player2 && Input.GetKeyDown(GameManager.Instance.KeyboardConfiguration.Player2Action))
                {
                    if (OccupiedMazeLevelEntry.Tile.GridLocation.X == CurrentGridLocation.X && OccupiedMazeLevelEntry.Tile.GridLocation.Y == CurrentGridLocation.Y)
                    {
                        PerformMazeLevelEntryAction(OccupiedMazeLevelEntry.MazeLevelName);
                    }
                }
            }

            if (OccupiedMazeLevelEntry.Tile.GridLocation.X == CurrentGridLocation.X && OccupiedMazeLevelEntry.Tile.GridLocation.Y == CurrentGridLocation.Y)
            {
                if (MapInteractionButtonsForPlayer.Count == 0)
                {
                    MainScreenCameraCanvas.Instance.CreateMapInteractionButton(
                        this,
                        new Vector2(OccupiedMazeLevelEntry.Tile.GridLocation.X, OccupiedMazeLevelEntry.Tile.GridLocation.Y),
                        MapInteractionAction.PerformMazeLevelEntryAction,
                        "Enter " + OccupiedMazeLevelEntry.MazeLevelName,
                        null);
                }
            }
        }

        public override bool ValidateTarget(Direction direction, Tile targetTile)
        {
            Tile currentTile = GameManager.Instance.CurrentGameLevel.TilesByLocation[CurrentGridLocation];
            if (targetTile.Walkable)
            {
                BridgePiece bridgePieceOnCurrentTile = currentTile.TryGetAttribute<BridgePiece>();
                BridgePiece bridgePieceOnTarget = targetTile.TryGetAttribute<BridgePiece>(); // optimisation: keep bridge locations of the level in a separate list, so we don't have to go over all the tiles in the level

                // there are no bridges involved
                if (bridgePieceOnCurrentTile == null && bridgePieceOnTarget == null)
                {
                    return true;
                }

                // Make sure we go in the correct bridge direction
                if (bridgePieceOnCurrentTile && bridgePieceOnTarget)
                {

                    if (bridgePieceOnCurrentTile.BridgePieceDirection == BridgePieceDirection.Horizontal &&
                        bridgePieceOnTarget.BridgePieceDirection == BridgePieceDirection.Horizontal &&
                        (direction == Direction.Left || direction == Direction.Right))
                    {
                        return true;
                    }

                    if (bridgePieceOnCurrentTile.BridgePieceDirection == BridgePieceDirection.Vertical &&
                        bridgePieceOnTarget.BridgePieceDirection == BridgePieceDirection.Vertical &&
                        (direction == Direction.Up || direction == Direction.Down))
                    {
                        return true;
                    }

                    return false;
                }

                if ((bridgePieceOnCurrentTile?.BridgePieceDirection == BridgePieceDirection.Horizontal ||
                    bridgePieceOnTarget?.BridgePieceDirection == BridgePieceDirection.Horizontal) &&
                    (direction == Direction.Left || direction == Direction.Right))
                {
                    return true;
                }

                if ((bridgePieceOnCurrentTile?.BridgePieceDirection == BridgePieceDirection.Vertical ||
                    bridgePieceOnTarget?.BridgePieceDirection == BridgePieceDirection.Vertical) &&
                    (direction == Direction.Up || direction == Direction.Down))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public void PerformMazeLevelEntryAction(string mazeName)
        {
            Logger.Log($"Go to maze {mazeName}");

            // Check levels list to see if we have the level. If the level is not found, inform and return. 
            if (!MazeLevelNamesData.LevelNameExists(mazeName))
            {
                MainScreenOverlayCanvas.Instance.ShowPlayerOneOptionMessagePanel($"Maze level '{mazeName}' was not found!", "Close", GameUIAction.ClosePanel);
                return;
            }

            // Player does not meet entry requirements? Return;

            if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
            {
                PersistentGameManager.SetCurrentSceneName(mazeName);
                OverworldGameplayManager.Instance.LoadMaze();
            }
            else if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
            {
                PersistentGameManager.SetCurrentSceneName(mazeName);
                OverworldGameplayManager.Instance.LoadMaze();
            }
            else
            {
                PersistentGameManager.SetCurrentSceneName(mazeName);

                PlayerSendsMazeLevelInvitationEvent playerSendsMazeLevelInvitationEvent = new PlayerSendsMazeLevelInvitationEvent();
                playerSendsMazeLevelInvitationEvent.SendPlayerSendsMazeLevelInvitationEvent(PhotonView.Owner.NickName, mazeName);

                string otherPlayerName = "";
                if (PlayerNumber == PlayerNumber.Player1)
                {
                    otherPlayerName = GameManager.Instance.CharacterManager.GetPlayerCharacter<PlayerCharacter>(PlayerNumber.Player2).PhotonView.Owner.NickName;
                }
                else if (PlayerNumber == PlayerNumber.Player2)
                {
                    otherPlayerName = GameManager.Instance.CharacterManager.GetPlayerCharacter<PlayerCharacter>(PlayerNumber.Player1).PhotonView.Owner.NickName;
                }
                else
                {
                    Logger.Warning($"Unknown player number {PlayerNumber}");
                }

                MainScreenOverlayCanvas.Instance.ShowPlayerZeroOptionMessagePanel($"We are waiting for {otherPlayerName} to accept our invitation...");
                MazeLevelInvitation.PendingInvitation = true;
            }
        }
    }
}
