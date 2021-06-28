using System.Collections.Generic;
using UnityEngine;

public class MusicInstrumentCase : MonoBehaviour, ITileAttribute
{
    public Tile Tile;
    public string ParentId;

    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] _musicInstrumentCaseSprites;

    private bool _isOpen;

    private int _sortingOrderBase = 500;
    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    public void Awake()
    {
        if(_musicInstrumentCaseSprites.Length == 0)
        {
            Logger.Error("Could not find MusicInstrumentCaseSprites");
        }
        
        Guard.CheckIsNull(_musicInstrumentCaseSprites, "_musicInstrumentCaseSprite", gameObject);

        _isOpen = false;

        _spriteRenderer.sprite = _musicInstrumentCaseSprites[0];
    }

    public void PlayerCollisionOnTile(PlayerCharacter player)
    {
        if (player != null)
        {
            if (_isOpen) return;

            Logger.Log("{0} entered tile {1},{2} with music instrument case", player.Name, Tile.GridLocation.X, Tile.GridLocation.Y);
            if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && !player.PhotonView.IsMine) return;

            OpenCase();
            return;
        }
    }

    public void EnemyCollisinOnTie(EnemyCharacter enemy)
    {
        if (_isOpen && enemy != null)
        {
            // an already started enemy should not be affected
            if(enemy.ChasingState == ChasingState.Startled)
            {
                return;
            }
            Logger.Log($"enemy {enemy.CharacterBlueprint.CharacterType} entered tile {Tile.GridLocation.X}, {Tile.GridLocation.Y} with an OPENED music instrument case");

            enemy.BecomeStartled();
        }
    }

    //public void OnCollisionEnter2D(Collision2D collision)
    //{

    //    PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
    //    if (player != null)
    //    {
    //        if (_isOpen) return;

    //        Logger.Log("{0} entered tile {1},{2} with music instrument case", player.Name, Tile.GridLocation.X, Tile.GridLocation.Y);
    //        if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && !player.PhotonView.IsMine) return;

    //        OpenCase();
    //        return;
    //    }
            
    //    EnemyCharacter enemy = collision.gameObject.GetComponent<EnemyCharacter>();
    //    if (_isOpen && enemy != null)
    //    {
    //        Logger.Log($"enemy {enemy.CharacterBlueprint.CharacterType} entered tile {Tile.GridLocation.X}, {Tile.GridLocation.Y} with an OPENED music instrument case");

    //    }
    //}

    public void OpenCase()
    {
        _spriteRenderer.sprite = _musicInstrumentCaseSprites[1];
        _isOpen = true;
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
