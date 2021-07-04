using CharacterType;
using UnityEngine;

public class CharacterSpawnpoint : MonoBehaviour, ITileAttribute
{
    public ICharacter CharacterType;
    public CharacterBlueprint CharacterBlueprint;
    public GridLocation GridLocation;

    public Tile Tile;
    public string ParentId;

    [SerializeField] protected SpriteRenderer _spriteRenderer;

    private int _sortingOrderBase;
    private const float _sortingOrderCalculationOffset = .5f;

    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    public virtual void Awake()
    {
        Guard.CheckIsNull(_spriteRenderer, "_spriteRenderer", gameObject);

        CharacterBlueprint = new CharacterBlueprint(new Emmon());

        _sortingOrderBase = SpriteSortingOrderRegister.Spawnpoint;
        _spriteRenderer.sortingOrder = (int)(_sortingOrderBase - transform.position.y - _sortingOrderCalculationOffset) * 10;
    }

    public void Start()
    {
    }

    public virtual void RegisterSpawnpoint() { }

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

        RegisterSpawnpoint();
    }
}
