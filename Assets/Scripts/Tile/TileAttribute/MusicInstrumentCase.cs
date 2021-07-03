using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicInstrumentCase : MonoBehaviour, ITileAttribute
{
    public Tile Tile;
    public string ParentId;

    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] _musicInstrumentCaseSprites;

    private bool _isOpen = false;
    private MazePlayerCharacter _caseOpener;

    private int _sortingOrderBase = 500;
    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    // in seconds
    const float OPEN_CASE_FULL_STRENGTH_LIFETIME = 6f;
    const float BLINKING_SPEED = 0.4f;
    const float BLINKING_LIFETIME = 4f;

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

    public void PlayerCollisionOnTile(MazePlayerCharacter player)
    {
        if (player != null)
        {
            if (_isOpen) return;

            Logger.Log("{0} entered tile {1},{2} with music instrument case", player.Name, Tile.GridLocation.X, Tile.GridLocation.Y);
            //if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && !player.PhotonView.IsMine) return;

            OpenCase(player);
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
            if(GameRules.GamePlayerType != GamePlayerType.NetworkMultiplayer || enemy.PhotonView?.IsMine == true)
            {
                Logger.Log($"enemy {enemy.CharacterBlueprint.CharacterType} entered tile {Tile.GridLocation.X}, {Tile.GridLocation.Y} with an OPENED music instrument case");
            }

            enemy.BecomeStartled();
            _caseOpener.MadeEnemyListenToMusicInstrument();
        }
    }

    private IEnumerator OpenedCaseCoroutine()
    {
        _spriteRenderer.sprite = _musicInstrumentCaseSprites[1];
        _isOpen = true;

        GameObject notesPlayMusicPrefab = MazeLevelGameplayManager.Instance.GetEffectAnimationPrefab(AnimationEffect.NotesPlayMusic);
        GameObject notesPlayMusicGO = GameObject.Instantiate(notesPlayMusicPrefab, SceneObjectManager.Instance.transform);
        Vector3 tileVectorPosition = GridLocation.GridToVector(Tile.GridLocation);
        Vector3 notesSpawnPosition = new Vector3(tileVectorPosition.x + 0.22f, tileVectorPosition.y + 0.2f, tileVectorPosition.z);
        notesPlayMusicGO.transform.position = notesSpawnPosition;

        EffectController notesPlayMusicEffectController = notesPlayMusicGO.GetComponent<EffectController>();


        yield return new WaitForSeconds(OPEN_CASE_FULL_STRENGTH_LIFETIME);

        float blinkingTimer = 0;
        float alphaValue = 0;

        while (blinkingTimer <= BLINKING_LIFETIME)
        {
            alphaValue = _spriteRenderer.color.a == 0 ? 1 : 0;
            Color changedAlphaColour = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, alphaValue);
            _spriteRenderer.color = changedAlphaColour;
            notesPlayMusicEffectController.SpriteRenderer.color = changedAlphaColour;

            yield return new WaitForSeconds(BLINKING_SPEED);
            blinkingTimer++;
        }

        GameObject smokeExplosionPrefab = MazeLevelGameplayManager.Instance.GetEffectAnimationPrefab(AnimationEffect.SmokeExplosion);
        GameObject smokeExplosionGO = GameObject.Instantiate(smokeExplosionPrefab, SceneObjectManager.Instance.transform);
        Vector3 smokeSpawnPosition = GridLocation.GridToVector(Tile.GridLocation);
        smokeExplosionGO.transform.position = smokeSpawnPosition;

        EffectController smokExplosionEffectController = smokeExplosionGO.GetComponent<EffectController>();
        smokExplosionEffectController.PlayEffect(AnimationEffect.SmokeExplosion);

        Destroy(gameObject);
        Destroy(notesPlayMusicGO);
    }

    private void OpenCase(MazePlayerCharacter player)
    {
        _caseOpener = player;
        StartCoroutine(OpenedCaseCoroutine());
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
