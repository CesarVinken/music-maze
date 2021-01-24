using UnityEngine;

public class PlayerExit : TileObstacle, IMazeTileAttribute, ITileConnectable
{
    public bool IsOpen;

    [SerializeField] private SpriteRenderer _secondarySpriteRenderer; // this sprite always comes in front of things, such as the lower half of a door that is viewed from the side.

    private int _secondarySpriteNumber;
    private int _secondaryGateSpriteSortingOrderBase = 501; // should be in front of tile marker and path layers
    private const float _secondaryGateSpriteSortingOrderCalculationOffset = .5f;

    public int SecondaryGateSpriteSortingOrderBase { get => _secondaryGateSpriteSortingOrderBase; set => _secondaryGateSpriteSortingOrderBase = value; }
    public override void Awake()
    {
        Guard.CheckIsNull(_spriteRenderer, "_spriteRenderer", gameObject);
        Guard.CheckIsNull(_secondarySpriteRenderer, "_spriteRendererForInFrontOfThings", gameObject);
        base.Awake();
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

        int[] spriteNumbers = TileDoorRegister._closedDoorSpriteNumberRegister[ConnectionScore];
        if(spriteNumbers.Length != 2)
        {
            spriteNumbers = new[] { 1, 7};
        }

        SpriteNumber = spriteNumbers[0];
        _secondarySpriteNumber = spriteNumbers[1];

        _spriteRenderer.sprite = SpriteManager.Instance.DefaultDoor[SpriteNumber - 1];
        _secondarySpriteRenderer.sprite = SpriteManager.Instance.DefaultDoor[_secondarySpriteNumber - 1];

        if (_secondarySpriteRenderer.sprite)
        {
            _secondarySpriteRenderer.sortingOrder = (int)(_secondaryGateSpriteSortingOrderBase - transform.position.y - _secondaryGateSpriteSortingOrderCalculationOffset) * 10;
        }
    }

    public override void TriggerTransformation()
    {
        if (ObstacleType == ObstacleType.Bush)
        {
            _spriteRenderer.sprite = SpriteManager.Instance.DefaultDoorColourful[SpriteNumber - 1];
        }
    }

    public void OpenExit()
    {
        Tile.Walkable = true;
        IsOpen = true;
        
        _spriteRenderer.sprite = SpriteManager.Instance.DefaultDoor[SpriteNumber - 1 + 3]; // + 3 to get to the 'open' version of the sprite
        _secondarySpriteRenderer.sprite = SpriteManager.Instance.DefaultDoor[_secondarySpriteNumber - 1 + 3];

        gameObject.layer = 9; // set layer to PlayerOnly, which is layer 9. Should not be hardcoded
        _spriteRenderer.gameObject.layer = 9;

        // Refresh pathfinding. TODO: only refresh tile for pathfinding and not the whole graph.
        AstarPath.active.Scan();
    }

    public void CloseExit()
    {
        Tile.Walkable = false;
        IsOpen = false;

        _spriteRenderer.sprite = SpriteManager.Instance.DefaultDoor[SpriteNumber - 1];
        _secondarySpriteRenderer.sprite = SpriteManager.Instance.DefaultDoor[_secondarySpriteNumber - 1];

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
