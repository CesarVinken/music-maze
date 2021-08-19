using Character;
using System.Collections.Generic;
using UnityEngine;

public class MazeLevelEntry : MonoBehaviour, ITileAttribute
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private int _sortingOrderBase;
    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    public Tile Tile;
    public string ParentId;
    public string MazeLevelName = "";

    private List<OverworldPlayerCharacter> _occupyingPlayers = new List<OverworldPlayerCharacter>();

    public virtual void Awake()
    {
        Guard.CheckIsNull(_spriteRenderer, "_spriteRenderer", gameObject);

        _sortingOrderBase = SpriteSortingOrderRegister.MazeLevelEntry;
        _spriteRenderer.sortingOrder = (int)(_sortingOrderBase - transform.position.y) * 10;
    }

    public void OnMouseDown()
    {
        if (_occupyingPlayers.Count == 0) return;

        //SINGLEPLAYER
        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer &&
            _occupyingPlayers[0].CurrentGridLocation.X == Tile.GridLocation.X &&
            _occupyingPlayers[0].CurrentGridLocation.Y == Tile.GridLocation.Y)
        {
            _occupyingPlayers[0].PerformMazeLevelEntryAction(MazeLevelName);
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
                _occupyingPlayers[i].PerformMazeLevelEntryAction(MazeLevelName);
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
            player.OccupiedMazeLevelEntry = this;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        OverworldPlayerCharacter player = collision.gameObject.GetComponent<OverworldPlayerCharacter>();
        if (player != null)
        {
            _occupyingPlayers.Remove(player);
            player.OccupiedMazeLevelEntry = null;
            MainScreenCameraCanvas.Instance.DestroyMapMapInteractionButton(player);
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
}
