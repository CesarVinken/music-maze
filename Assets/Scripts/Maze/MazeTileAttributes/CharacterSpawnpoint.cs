using UnityEngine;

public class CharacterSpawnpoint : MonoBehaviour, IMazeTileAttribute
{
    public CharacterType CharacterType;
    public CharacterBlueprint CharacterBlueprint;
    public GridLocation GridLocation;

    public Tile Tile;
    public string ParentId;

    [SerializeField] protected SpriteRenderer _spriteRenderer;

    public void Awake()
    {
        Guard.CheckIsNull(_spriteRenderer, "_spriteRenderer", gameObject);

        CharacterBlueprint = new CharacterBlueprint(CharacterType);
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
