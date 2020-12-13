using UnityEngine;

public class PlayerExit : TileObstacle, IMazeTileAttribute, ITileConnectable
{
    public bool IsOpen;

    [SerializeField] private Sprite[] _defaultWallDoor;


    public void Awake()
    {
        Guard.CheckIsNull(_spriteRenderer, "_spriteRenderer", gameObject);
    }

    public void Start()
    {
        if (!EditorManager.InEditor)
        {
            MazeLevelManager.Instance.Level.MazeExits.Add(this);
        }
    }

    public override void WithConnectionScoreInfo(TileConnectionScoreInfo obstacleConnectionScoreInfo)
    {
        ConnectionScore = obstacleConnectionScoreInfo.RawConnectionScore;
        SpriteNumber = ConnectionScore;

        if (ConnectionScore <= 0 || ConnectionScore > 8)
        {
            Logger.Warning($"obstacleConnectionScore {ConnectionScore} should be between 1 and 8");
            _spriteRenderer.sprite = SpriteManager.Instance.DefaultDoor[0];
            return;
        }

        _spriteRenderer.sprite = SpriteManager.Instance.DefaultDoor[SpriteNumber - 1];
    }

    public void OpenExit()
    {
        Tile.Walkable = true;
        IsOpen = true;
        
        _spriteRenderer.sprite = SpriteManager.Instance.DefaultDoor[ConnectionScore - 1 + 8]; // + 8 because the last two rows of the sprites are the Opened versions of the same sprites

        gameObject.layer = 9; // set layer to PlayerOnly, which is layer 9. Should not be hardcoded
        _spriteRenderer.gameObject.layer = 9;

        // Refresh pathfinding. TODO: only refresh tile for pathfinding and not the whole graph.
        AstarPath.active.Scan();
    }

    public void CloseExit()
    {
        Tile.Walkable = false;
        IsOpen = false;

        gameObject.layer = 8; // set layer to Unwalkable, which is layer 8. Should not be hardcoded
        _spriteRenderer.gameObject.layer = 8;

        // Refresh pathfinding. TODO: only refresh tile for pathfinding and not the whole graph.
        AstarPath.active.Scan();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        if (player != null)
        {
            Logger.Log("{0} reached the exit! {1},{2}", player.name, Tile.GridLocation.X, Tile.GridLocation.Y);
            CharacterManager.Instance.CharacterExit(player);
        }
    }
}
