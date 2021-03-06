using UnityEngine;

public class OverworldPlayerCharacter : PlayerCharacter
{
    public MazeLevelEntry OccupiedMazeLevelEntry = null;

    public override void Awake()
    {
        base.Awake();

        GameManager.Instance.CharacterManager.AddPlayer(PlayerNumber, this);
    }

    public override void Start()
    {
        base.Start();

        //transform the player's starting tile and surrounding tiles
        InGameOverworldTile currentTile = GameManager.Instance.CurrentGameLevel.TilesByLocation[StartingPosition] as InGameOverworldTile;
        CurrentGridLocation = currentTile.GridLocation;
    }

    public override void Update()
    {
        if (MazeLevelInvitation.PendingInvitation)
            return;
        
        base.Update();


        if (OccupiedMazeLevelEntry != null && (GameRules.GamePlayerType == GamePlayerType.SinglePlayer || PhotonView.IsMine))
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) )
            {
                if(OccupiedMazeLevelEntry.Tile.GridLocation.X == CurrentGridLocation.X && OccupiedMazeLevelEntry.Tile.GridLocation.Y == CurrentGridLocation.Y)
                {
                    PerformMazeLevelEntryAction(OccupiedMazeLevelEntry.MazeLevelName);
                }
            }
        }
    }

    public void PerformMazeLevelEntryAction(string mazeName)
    {
        // Player does not meet entry requirements? Return;

        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
        {
            PersistentGameManager.SetCurrentSceneName(mazeName);
            OverworldManager.Instance.LoadMaze();
        }
        else
        {
            PersistentGameManager.SetCurrentSceneName(mazeName);

            PlayerSendsMazeLevelInvitationEvent playerSendsMazeLevelInvitationEvent = new PlayerSendsMazeLevelInvitationEvent();
            playerSendsMazeLevelInvitationEvent.SendPlayerSendsMazeLevelInvitationEvent(PhotonView.Owner.NickName, mazeName);

            string otherPlayerName = "";
            if(PlayerNumber == PlayerNumber.Player1)
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

            OverworldMainScreenOverlayCanvas.Instance.ShowPlayerMessagePanel($"We are waiting for {otherPlayerName} to accept our invitation...");
            MazeLevelInvitation.PendingInvitation = true;
        }
    }

    public override bool ValidateTarget(GridLocation targetGridLocation)
    {
        if (OverworldManager.Instance.Overworld.TilesByLocation.TryGetValue(targetGridLocation, out Tile tile))
        {
            if (tile.Walkable)
                return true;
        }
        return false;
    }
}
