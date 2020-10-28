using UnityEngine;

public class MazeExit : MonoBehaviour, IMazeTileAttribute
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GridLocation _gridLocation;

    public void Awake()
    {
        Guard.CheckIsNull(_spriteRenderer, "_spriteRenderer", gameObject);

        _gridLocation = GridLocation.VectorToGrid(transform.position);
    }

    public void Start()
    {
        MazeLevelManager.Instance.Level.MazeExits.Add(this);
    }

    public void OpenExit()
    {
        Tile tile = MazeLevelManager.Instance.Level.TilesByLocation[_gridLocation];

        if (tile == null) Logger.Error("Could not find a tile for grid location {0},{1}", _gridLocation.X, _gridLocation.Y);

        tile.Walkable = true;
        _spriteRenderer.enabled = false;

        gameObject.layer = 9; // set layer to PlayerOnly, which is layer 9. Should not be hardcoded
        _spriteRenderer.gameObject.layer = 9;

        // Refresh pathfinding. TODO: only refresh tile for pathfinding and not the whole graph.
        AstarPath.active.Scan();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        if (player != null)
        {
            Logger.Log("{0} reached the exit! {1},{2}", player.name, _gridLocation.X, _gridLocation.Y);
            CharacterManager.Instance.CharacterExit(player);
        }
    }

    public void Remove()
    {
        Destroy(this);
        Destroy(gameObject);
    }
}
