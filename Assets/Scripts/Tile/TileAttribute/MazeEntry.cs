using System.Collections.Generic;
using UnityEngine;

public class MazeEntry : MonoBehaviour, ITileAttribute
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private bool _hasPlayerOnTile = false;

    private int _sortingOrderBase = 500;
    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    public Tile Tile;
    public string ParentId;

    private List<OverworldPlayerCharacter> _occupyingPlayers = new List<OverworldPlayerCharacter>();

    public virtual void Awake()
    {
        Guard.CheckIsNull(_spriteRenderer, "_spriteRenderer", gameObject);

        _spriteRenderer.sortingOrder = (int)(_sortingOrderBase - transform.position.y) * 10;
    }

    //public void Update()
    //{
    //    if (_hasPlayerOnTile)
    //    {
    //        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
    //        {
    //            if(GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
    //            {
    //                EnterMaze();
    //            }
    //            else
    //            {
    //                PlayerSendsMazeLevelInvitationEvent playerSendsMazeLevelInvitationEvent = new PlayerSendsMazeLevelInvitationEvent();
    //                playerSendsMazeLevelInvitationEvent.SendPlayerSendsMazeLevelInvitationEvent("temporary");
    //                MainScreenOverlayCanvas.Instance.ShowMazeInvitation(PhotonNetwork.);
    //            }
    //        }
    //    }
    //}

    public void OnMouseDown()
    {
        if (_occupyingPlayers.Count == 0) return;

        //SINGLEPLAYER
        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer &&
            _occupyingPlayers[0].CurrentGridLocation.X == Tile.GridLocation.X &&
            _occupyingPlayers[0].CurrentGridLocation.Y == Tile.GridLocation.Y)
        {
            _occupyingPlayers[0].PerformMazeEntryAction();
            return;
        }

        // MULTIPLAYER
        for (int i = 0; i < _occupyingPlayers.Count; i++)
        {
            if(!MazeLevelInvitation.PendingInvitation &&
                _occupyingPlayers[i].PhotonView.IsMine && 
                _occupyingPlayers[i].CurrentGridLocation.X == Tile.GridLocation.X && 
                _occupyingPlayers[i].CurrentGridLocation.Y == Tile.GridLocation.Y)
            {
                _occupyingPlayers[i].PerformMazeEntryAction();
                break;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        OverworldPlayerCharacter player = collision.gameObject.GetComponent<OverworldPlayerCharacter>();
        if (player != null)
        {
            _occupyingPlayers.Add(player);
            player.OccupiedMazeEntry = this;
            MainScreenCameraCanvas.Instance.ShowMapInteractionButton(player, transform.position, "Enter default maze");
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        OverworldPlayerCharacter player = collision.gameObject.GetComponent<OverworldPlayerCharacter>();
        if (player != null)
        {
            _occupyingPlayers.Remove(player);
            player.OccupiedMazeEntry = null;
            MainScreenCameraCanvas.Instance.HideMapMapInteractionButton();
        }
    }

    public void Remove()
    {
        Destroy(this);
        Destroy(gameObject);
    }

    public void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;
    }

    //private void EnterMaze()
    //{
    //    OverworldManager.Instance.LoadMaze();
    //}
}
