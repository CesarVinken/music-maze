using UnityEngine;

public class CharacterSpawnpoint : MonoBehaviour, IMazeTileAttribute
{
    public CharacterType CharacterType;
    public CharacterBlueprint CharacterBlueprint;
    public GridLocation GridLocation;

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
        throw new System.NotImplementedException();
    }
}
