using UnityEngine;

public class OverworldPlayerCharacter : PlayerCharacter
{
    public MazeEntry OccupiedMazeEntry = null;

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
        base.Update();

        if (OccupiedMazeEntry != null && PhotonView.IsMine && MazeLevelInvitation.PendingInvitation == false)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                PerformMazeEntryAction();
            }
        }
    }

    public void PerformMazeEntryAction()
    {
        // Player does not meet entry requirements? Return;

        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
        {
            OverworldManager.Instance.LoadMaze();
        }
        else
        {
            PlayerSendsMazeLevelInvitationEvent playerSendsMazeLevelInvitationEvent = new PlayerSendsMazeLevelInvitationEvent();
            playerSendsMazeLevelInvitationEvent.SendPlayerSendsMazeLevelInvitationEvent(PhotonView.Owner.NickName, "default");

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
