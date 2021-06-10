using System.Collections;
using UnityEngine;

public class BridgePiece : MonoBehaviour, ITileAttribute, ITransformable
{
    public Tile Tile;
    public string ParentId;

    public BridgeType BridgeType;
    public BridgePieceDirection BridgePieceDirection;

    [SerializeField] private TileSpriteContainer _tileSpriteContainer;

    private int _sortingOrderBase = 500; 
    private int _sortingOrder; // TODO: Back piece should be behind characters, front piece should be in front of characters.

    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    public void Awake()
    {
        Guard.CheckIsNull(_tileSpriteContainer, "_tileSpriteContainer", gameObject);

        _sortingOrder = (int)(_sortingOrderBase - transform.position.y) * 10 + 1;
    }

    public void WithBridgeType(BridgeType bridgeType)
    {
        BridgeType = bridgeType;
    }

    public void WithBridgePieceDirection(BridgePieceDirection bridgePieceDirection)
    {
        BridgePieceDirection = bridgePieceDirection;

        if(bridgePieceDirection == BridgePieceDirection.Horizontal)
        {
            _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.WoodenBridge[0]);
        }
        else if(bridgePieceDirection == BridgePieceDirection.Vertical)
        {
            _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.WoodenBridge[1]);
        }
        else
        {
            Logger.Error($"Have no sprite set up for the direction {bridgePieceDirection}");
        }
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

    public void TriggerTransformation()
    {

        if (BridgeType == BridgeType.Wooden)
        {
            Sprite colourfulSprite = MazeSpriteManager.Instance.WoodenBridgeColourful[0];
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
