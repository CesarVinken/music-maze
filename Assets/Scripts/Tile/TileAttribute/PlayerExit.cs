using System.Collections;
using UnityEngine;

public class PlayerExit : TileObstacle, ITileAttribute, ITileConnectable
{
    public bool IsOpen;

    [SerializeField] private TileSpriteContainer _secondaryTileSpriteContainer; // this sprite always comes in front of things, such as the lower half of a door that is viewed from the side.

    private int _secondarySpriteNumber;
    private int _secondaryGateSpriteSortingOrderBase = 501; // should be in front of tile marker and path layers
    private const float _secondaryGateSpriteSortingOrderCalculationOffset = .5f;
    private int _secondarySpriteSortingOrder;

    public int SecondaryGateSpriteSortingOrderBase { get => _secondaryGateSpriteSortingOrderBase; set => _secondaryGateSpriteSortingOrderBase = value; }

    public override void Awake()
    {
        Guard.CheckIsNull(_secondaryTileSpriteContainer, "TileSpriteContainer", gameObject);

        base.Awake();
    }

    public void Start()
    {
        if (!EditorManager.InEditor)
        {
            MazeLevelGameplayManager.Instance.Level.MazeExits.Add(this);
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

        Sprite primarySprite = MazeSpriteManager.Instance.DefaultDoor[SpriteNumber - 1];
        _tileSpriteContainer.SetSprite(primarySprite);
        _tileSpriteContainer.SetRendererAlpha(1);

        Sprite secondarySprite = MazeSpriteManager.Instance.DefaultDoor[_secondarySpriteNumber - 1];
        _secondaryTileSpriteContainer.SetSprite(secondarySprite);
        _secondaryTileSpriteContainer.SetRendererAlpha(1);

        if (_secondaryTileSpriteContainer.SpriteRenderer.sprite)
        {
            _secondarySpriteSortingOrder = (int)(_secondaryGateSpriteSortingOrderBase - transform.position.y - _secondaryGateSpriteSortingOrderCalculationOffset) * 10;
            _secondaryTileSpriteContainer.SetSortingOrder(_secondarySpriteSortingOrder);
        }

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        MazePlayerCharacter player = collision.gameObject.GetComponent<MazePlayerCharacter>();
        if (player != null)
        {
            Logger.Log("{0} reached the exit! {1},{2}", player.name, Tile.GridLocation.X, Tile.GridLocation.Y);
            GameManager.Instance.CharacterManager.ExitCharacter(player);
        }
    }

    public override void TriggerTransformation()
    {
        if (ObstacleType == ObstacleType.Bush && !IsOpen)
        {
            Sprite primarySprite = MazeSpriteManager.Instance.DefaultDoorColourful[SpriteNumber - 1];
            Sprite secondarySprite = MazeSpriteManager.Instance.DefaultDoorColourful[_secondarySpriteNumber - 1];

            IEnumerator transformToColourful = TransformToColourful(primarySprite, secondarySprite);
            StartCoroutine(transformToColourful);
        }
        else
        {
            Sprite primarySprite = MazeSpriteManager.Instance.DefaultDoorColourful[SpriteNumber - 1 + 3]; // + 3 to get to the 'open' version of the sprite
            Sprite secondarySprite = MazeSpriteManager.Instance.DefaultDoorColourful[_secondarySpriteNumber - 1 + 3];

            IEnumerator transformToColourful = TransformToColourful(primarySprite, secondarySprite);
            StartCoroutine(transformToColourful);
        }
    }

    public void OpenExit()
    {
        MazeTile tile = Tile as MazeTile;
        tile.SetWalkable(true);
        IsOpen = true;

        _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.DefaultDoorColourful[SpriteNumber - 1 + 3]); // + 3 to get to the 'open' version of the sprite
        _secondaryTileSpriteContainer.SetSprite(MazeSpriteManager.Instance.DefaultDoorColourful[_secondarySpriteNumber - 1 + 3]);
        
        gameObject.layer = 9; // set layer to PlayerOnly, which is layer 9. Should not be hardcoded
        _tileSpriteContainer.gameObject.layer = 9;

        // Refresh pathfinding. TODO: only refresh tile for pathfinding and not the whole graph.
        AstarPath.active.Scan();
    }

    public void CloseExit()
    {
        MazeTile tile = Tile as MazeTile;
        tile.SetWalkable(false);
        IsOpen = false;

        _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.DefaultDoor[SpriteNumber - 1]);
        _secondaryTileSpriteContainer.SetSprite(MazeSpriteManager.Instance.DefaultDoor[_secondarySpriteNumber - 1]);

        gameObject.layer = 8; // set layer to Unwalkable, which is layer 8. Should not be hardcoded
        _tileSpriteContainer.gameObject.layer = 8;

        // Refresh pathfinding. TODO: only refresh tile for pathfinding and not the whole graph.
        AstarPath.active.Scan();
    }  

    public IEnumerator TransformToColourful(Sprite colourfulSprite, Sprite secondaryColourfulSprite)
    {
        TileSpriteContainer transformedPrimarySpriteContainer = TileSpriteContainerPool.Instance.Get();
        transformedPrimarySpriteContainer.transform.SetParent(transform);
        transformedPrimarySpriteContainer.SetSprite(colourfulSprite);
        transformedPrimarySpriteContainer.SetSortingOrder(_sortingOrder);
        transformedPrimarySpriteContainer.gameObject.SetActive(true);
        transformedPrimarySpriteContainer.gameObject.layer = _tileSpriteContainer.gameObject.layer;
        transformedPrimarySpriteContainer.transform.position = transform.position;

        TileSpriteContainer transformedSecondarySpriteContainer = TileSpriteContainerPool.Instance.Get();
        transformedSecondarySpriteContainer.transform.SetParent(transform);
        transformedSecondarySpriteContainer.SetSprite(secondaryColourfulSprite);
        transformedSecondarySpriteContainer.SetSortingOrder(_sortingOrder);
        transformedSecondarySpriteContainer.gameObject.SetActive(true);
        transformedSecondarySpriteContainer.gameObject.layer = _secondaryTileSpriteContainer.gameObject.layer;
        transformedSecondarySpriteContainer.transform.position = transform.position;

        _tileSpriteContainer.SetSortingOrder(_sortingOrder - 1);
        _secondaryTileSpriteContainer.SetSortingOrder(_sortingOrder - 1);

        float fadeSpeed = 1f;
        float alphaAmount = 0;

        while (alphaAmount < 1)
        {
            alphaAmount = alphaAmount + (fadeSpeed * Time.deltaTime);
            transformedPrimarySpriteContainer.SetRendererAlpha(alphaAmount);
            transformedSecondarySpriteContainer.SetRendererAlpha(alphaAmount);

            yield return null;
        }

        _tileSpriteContainer.gameObject.layer = 0;
        _secondaryTileSpriteContainer.gameObject.layer = 0;

        TileSpriteContainerPool.Instance.ReturnToPool(_tileSpriteContainer);
        TileSpriteContainerPool.Instance.ReturnToPool(_secondaryTileSpriteContainer);

        _tileSpriteContainer = transformedPrimarySpriteContainer;
        _secondaryTileSpriteContainer = transformedSecondarySpriteContainer;
    }
}
