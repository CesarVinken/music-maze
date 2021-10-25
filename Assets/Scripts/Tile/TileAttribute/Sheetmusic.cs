using Character;
using System.Collections;
using UnityEngine;

public class Sheetmusic : MonoBehaviour, ITileAttribute, IScoreAttribute
{
    public Tile Tile;
    public string ParentId;

    [SerializeField] protected SpriteRenderer _sheetmusicSpriteRenderer;
    [SerializeField] protected SpriteRenderer _playerColourRenderer;
    [SerializeField] private Sprite[] _sheetmusicSprites;
    [SerializeField] private Sprite _playerColourIndicatorSprite;

    private bool _isSetUp = false;
    private MazePlayerCharacter _sheetmusicFinder;

    private int _sortingOrderBase;
    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    // in seconds
    const float READING_FULL_STRENGTH_LIFETIME = 6f;
    const float BLINKING_SPEED = 0.4f;
    const float BLINKING_LIFETIME = 4f;

    public void Awake()
    {
        if(_sheetmusicSprites.Length == 0)
        {
            Logger.Error("Could not find SheetmusicSprites");
        }

        Guard.CheckIsNull(_playerColourIndicatorSprite, "_playerColourIndicatorSprite", gameObject);

        Guard.CheckIsNull(_sheetmusicSpriteRenderer, "_sheetmusicSpriteRenderer", gameObject);
        Guard.CheckIsNull(_playerColourRenderer, "_playerColourRenderer", gameObject);

        _isSetUp = false;

        _playerColourRenderer.sprite = null;
        _playerColourRenderer.gameObject.SetActive(false);

        _sheetmusicSpriteRenderer.sprite = _sheetmusicSprites[0];
        _sheetmusicSpriteRenderer.sortingOrder = SpriteSortingOrderRegister.Sheetmusic;
    }

    public void PlayerCollisionOnTile(MazePlayerCharacter player)
    {
        Logger.Log("Collision enter");
        if (player != null)
        {
            if (_isSetUp) return;

            Logger.Log("{0} entered tile {1},{2} with sheetmusic", player.Name, Tile.GridLocation.X, Tile.GridLocation.Y);

            SetupSheetmusic(player);
            return;
        }
    }

    public void EnemyCollisionOnTile(EnemyCharacter enemy)
    {
        if (_isSetUp && enemy != null)
        {
            // an already startled enemy should not be affected by sheetmusic
            if (enemy.ChasingState == ChasingState.Startled)
            {
                return;
            }
            if (GameRules.GamePlayerType != GamePlayerType.NetworkMultiplayer || enemy.PhotonView?.IsMine == true)
            {
                Logger.Log($"enemy {enemy.CharacterBlueprint.CharacterType} entered tile {Tile.GridLocation.X}, {Tile.GridLocation.Y} with an OPENED music instrument case");
            }

            ReadSheetmusic(enemy);
        }
    }

    private IEnumerator ReadSheetmusicCoroutine()
    {
        _isSetUp = false; // Sheetmusic should trigger only once, so once an enemy is reading, it should not longer be triggered for another enemy.

        yield return new WaitForSeconds(READING_FULL_STRENGTH_LIFETIME);

        float blinkingTimer = 0;
        float alphaValue = 0;

        while (blinkingTimer <= BLINKING_LIFETIME)
        {
            alphaValue = _sheetmusicSpriteRenderer.color.a == 0 ? 1 : 0;
            Color changedAlphaColour = new Color(_sheetmusicSpriteRenderer.color.r, _sheetmusicSpriteRenderer.color.g, _sheetmusicSpriteRenderer.color.b, alphaValue);
            _sheetmusicSpriteRenderer.color = changedAlphaColour;
            _playerColourRenderer.color = changedAlphaColour;

            yield return new WaitForSeconds(BLINKING_SPEED);
            blinkingTimer++;
        }

        GameObject smokeExplosionPrefab = MazeLevelGameplayManager.Instance.GetEffectAnimationPrefab(AnimationEffect.SmokeExplosion);
        GameObject smokeExplosionGO = GameObject.Instantiate(smokeExplosionPrefab, SceneObjectManager.Instance.transform);
        Vector3 smokeSpawnPosition = GridLocation.GridToVector(Tile.GridLocation);
        smokeExplosionGO.transform.position = smokeSpawnPosition;

        EffectController smokeExplosionEffectController = smokeExplosionGO.GetComponent<EffectController>();
        smokeExplosionEffectController.PlayEffect(AnimationEffect.SmokeExplosion);

        Destroy(gameObject);
    }

    private void SetupSheetmusic(MazePlayerCharacter player)
    {
        _sheetmusicFinder = player;

        _sheetmusicSpriteRenderer.sprite = _sheetmusicSprites[1];

        Color playerColor = PlayerColour.GetColor(_sheetmusicFinder.GetCharacterType());
        _playerColourRenderer.sprite = _playerColourIndicatorSprite;
        _playerColourRenderer.color = new Color(playerColor.r, playerColor.g, playerColor.b, 1);
        _playerColourRenderer.sortingOrder = _sheetmusicSpriteRenderer.sortingOrder + 1;
        _playerColourRenderer.gameObject.SetActive(true);

        _isSetUp = true;
    }

    private void ReadSheetmusic(EnemyCharacter enemy)
    {
        enemy.ReadSheetmusic(this, _sheetmusicFinder);
        _sheetmusicFinder.MadeEnemyReadSheetmusic();

        StartCoroutine(ReadSheetmusicCoroutine());
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
