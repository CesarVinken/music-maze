using System.Collections;
using UnityEngine;

public class BridgeEdge : MonoBehaviour, ITileAttribute, ITransformable
{
    public Tile Tile;
    public string ParentId;

    public BridgeType BridgeType;
    public Direction EdgeSide;
    public BridgePiece BridgePieceConnection;

    [SerializeField] private TileSpriteContainer _tileSpriteContainer;
    private int _sortingOrderBase = 500;
    private int _sortingOrder;

    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    public void Awake()
    {
        Guard.CheckIsNull(_tileSpriteContainer, "_tileSpriteContainer", gameObject);

        _sortingOrder = (int)(_sortingOrderBase - transform.position.y) * 10 + 1;
    }

    public void Remove()
    {
        Logger.Log($"Destory bridge edge at {Tile.GridLocation.X}, {Tile.GridLocation.Y}");
        BridgePieceConnection.RemoveBridgeEdgeConnection(this);
        Tile.RemoveBridgeEdge(this);
        Destroy(this);
        Destroy(gameObject);
    }

    public void WithBridgeEdgeSide(Direction edgeSide)
    {
        EdgeSide = edgeSide;
    }

    public void WithBridgeType(BridgeType bridgeType)
    {
        BridgeType = bridgeType;
    }

    public void WithBridgePieceConnection(BridgePiece bridgePiece)
    {
        BridgePieceConnection = bridgePiece;
        BridgePieceConnection.AddBridgeEdgeConnection(this);
    }

    public void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;
    }

    public void SetSprite()
    {
        switch (EdgeSide)
        {
            case Direction.Up:
                _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.WoodenBridge[5]);
                break;
            case Direction.Right:
                _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.WoodenBridge[2]);
                break;
            case Direction.Down:
                _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.WoodenBridge[4]);
                break;
            case Direction.Left:
                _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.WoodenBridge[3]);
                break;
            default:
                break;
        }
    }

    public void TriggerTransformation()
    {

        if (BridgeType == BridgeType.Wooden)
        {
            Sprite colourfulSprite;
            switch (EdgeSide)
            {
                case Direction.Up:
                    colourfulSprite = MazeSpriteManager.Instance.WoodenBridgeColourful[5];
                    break;
                case Direction.Right:
                    colourfulSprite = MazeSpriteManager.Instance.WoodenBridgeColourful[2];
                    break;
                case Direction.Down:
                    colourfulSprite = MazeSpriteManager.Instance.WoodenBridgeColourful[4];
                    break;
                case Direction.Left:
                    colourfulSprite = MazeSpriteManager.Instance.WoodenBridgeColourful[3];
                    break;
                default:
                    Logger.Error($"Could not find a case for direction {EdgeSide}");
                    return;
            }

            IEnumerator transformToColourful = TransformToColourful(colourfulSprite);
            StartCoroutine(transformToColourful);
        }
        else
        {
            Logger.Error($"Colourful mode not implemented for BridgeType {BridgeType}");
        }
    }

    public IEnumerator TransformToColourful(Sprite colourfulSprite)
    {
        TileSpriteContainer transformedSpriteContainer = TileSpriteContainerPool.Instance.Get();
        transformedSpriteContainer.transform.SetParent(transform);
        transformedSpriteContainer.SetSprite(colourfulSprite);
        transformedSpriteContainer.SetSortingOrder(_sortingOrder);
        transformedSpriteContainer.gameObject.SetActive(true);
        transformedSpriteContainer.transform.position = transform.position;

        _tileSpriteContainer.SetSortingOrder(_sortingOrder - 1);

        float fadeSpeed = 1f;
        float alphaAmount = 0;

        while (alphaAmount < 1)
        {
            alphaAmount = alphaAmount + (fadeSpeed * Time.deltaTime);
            transformedSpriteContainer.SetRendererAlpha(alphaAmount);

            yield return null;
        }

        TileSpriteContainerPool.Instance.ReturnToPool(_tileSpriteContainer);
        _tileSpriteContainer = transformedSpriteContainer;
    }
}
