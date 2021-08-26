using Character;
using System.Collections;
using UnityEngine;

public class Sheetmusic : MonoBehaviour, ITileAttribute
{
    public Tile Tile;
    public string ParentId;

    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] _sheetmusicSprites;

    private bool _isSetUp = false;
    private MazePlayerCharacter _sheetmusicFinder;

    private int _sortingOrderBase;
    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    // in seconds
    const float OPEN_CASE_FULL_STRENGTH_LIFETIME = 6f;
    const float BLINKING_SPEED = 0.4f;
    const float BLINKING_LIFETIME = 4f;

    public void Awake()
    {
        if(_sheetmusicSprites.Length == 0)
        {
            Logger.Error("Could not find MusicInstrumentCaseSprites");
        }

        Guard.CheckIsNull(_sheetmusicSprites, "_musicInstrumentCaseSprite", gameObject);

        _isSetUp = false;

        _spriteRenderer.sprite = _sheetmusicSprites[0];
        _spriteRenderer.sortingOrder = SpriteSortingOrderRegister.Sheetmusic;
    }

    public void PlayerCollisionOnTile(MazePlayerCharacter player)
    {
        if (player != null)
        {
            if (_isSetUp) return;

            Logger.Log("{0} entered tile {1},{2} with sheetmusic", player.Name, Tile.GridLocation.X, Tile.GridLocation.Y);

            SetupSheetmusic(player);
            return;
        }
    }

    public void EnemyCollisinOnTile(EnemyCharacter enemy)
    {
        if (_isSetUp && enemy != null)
        {
            // an already XXXX enemy should not be affected
            //if(enemy.ChasingState == ChasingState.Startled)
            //{
            //    return;
            //}
            //if(GameRules.GamePlayerType != GamePlayerType.NetworkMultiplayer || enemy.PhotonView?.IsMine == true)
            //{
            //    Logger.Log($"enemy {enemy.CharacterBlueprint.CharacterType} entered tile {Tile.GridLocation.X}, {Tile.GridLocation.Y} with an OPENED music instrument case");
            //}

            // TODO: enemy.readSheetmusic
            // TODO: _caseOpener.MadeEnemyReadSheetmusic();
        }
    }

    //private IEnumerator OpenedCaseCoroutine()
    //{
    //    _spriteRenderer.sprite = _sheetmusicSprites[1];
    //    _isSetUp = true;

    //    GameObject notesPlayMusicPrefab = MazeLevelGameplayManager.Instance.GetEffectAnimationPrefab(AnimationEffect.NotesPlayMusic);
    //    GameObject notesPlayMusicGO = GameObject.Instantiate(notesPlayMusicPrefab, SceneObjectManager.Instance.transform);
    //    Vector3 tileVectorPosition = GridLocation.GridToVector(Tile.GridLocation);
    //    Vector3 notesSpawnPosition = new Vector3(tileVectorPosition.x + 0.22f, tileVectorPosition.y + 0.2f, tileVectorPosition.z);
    //    notesPlayMusicGO.transform.position = notesSpawnPosition;

    //    EffectController notesPlayMusicEffectController = notesPlayMusicGO.GetComponent<EffectController>();

    //    yield return new WaitForSeconds(OPEN_CASE_FULL_STRENGTH_LIFETIME);

    //    float blinkingTimer = 0;
    //    float alphaValue = 0;

    //    while (blinkingTimer <= BLINKING_LIFETIME)
    //    {
    //        alphaValue = _spriteRenderer.color.a == 0 ? 1 : 0;
    //        Color changedAlphaColour = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, alphaValue);
    //        _spriteRenderer.color = changedAlphaColour;
    //        notesPlayMusicEffectController.SpriteRenderer.color = changedAlphaColour;

    //        yield return new WaitForSeconds(BLINKING_SPEED);
    //        blinkingTimer++;
    //    }

    //    GameObject smokeExplosionPrefab = MazeLevelGameplayManager.Instance.GetEffectAnimationPrefab(AnimationEffect.SmokeExplosion);
    //    GameObject smokeExplosionGO = GameObject.Instantiate(smokeExplosionPrefab, SceneObjectManager.Instance.transform);
    //    Vector3 smokeSpawnPosition = GridLocation.GridToVector(Tile.GridLocation);
    //    smokeExplosionGO.transform.position = smokeSpawnPosition;

    //    EffectController smokeExplosionEffectController = smokeExplosionGO.GetComponent<EffectController>();
    //    smokeExplosionEffectController.PlayEffect(AnimationEffect.SmokeExplosion);

    //    Destroy(gameObject);
    //    Destroy(notesPlayMusicGO);
    //}

    private void SetupSheetmusic(MazePlayerCharacter player)
    {
        _sheetmusicFinder = player;
        //StartCoroutine(OpenedCaseCoroutine());
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
