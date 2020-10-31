using UnityEngine;

public class CharacterSpawnpoint : MonoBehaviour, IMazeTileAttribute
{
    public CharacterType CharacterType;
    public CharacterBlueprint CharacterBlueprint;
    public GridLocation GridLocation;

    public Tile Tile;
    public string ParentId;

    public void Awake()
    {
        CharacterBlueprint = new CharacterBlueprint(CharacterType);

        RegisterSpawnpoint();
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
    }
}
