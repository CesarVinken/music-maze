using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgePiece : MonoBehaviour, ITileAttribute, ITransformable
{
    public Tile Tile;
    public string ParentId;

    public BridgeType BridgeType;
    public BridgePieceDirection BridgePieceDirection;

    [SerializeField] private TileSpriteContainer _tileSpriteContainer;
    [SerializeField] private GameObject _bridgeEdgePrefab;

    private int _sortingOrderBase = 500; 
    private int _sortingOrder; // TODO: Back piece should be behind characters, front piece should be in front of characters.

    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    public void Awake()
    {
        Guard.CheckIsNull(_tileSpriteContainer, "_tileSpriteContainer", gameObject);
        Guard.CheckIsNull(_bridgeEdgePrefab, "_bridgeEdgePrefab", gameObject);

        _sortingOrder = (int)(_sortingOrderBase - transform.position.y) * 10 + 1;
    }

    public void WithBridgeType(BridgeType bridgeType)
    {
        BridgeType = bridgeType;
    }

    public void WithBridgePieceDirection(BridgePieceDirection bridgePieceDirection)
    {
        BridgePieceDirection = bridgePieceDirection;
    }

    public void SetSprite()
    {
        if (BridgePieceDirection == BridgePieceDirection.Horizontal)
        {
            _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.WoodenBridge[0]);
        }
        else if (BridgePieceDirection == BridgePieceDirection.Vertical)
        {
            _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.WoodenBridge[1]);
        }
        else
        {
            Logger.Error($"Have no sprite set up for the direction {BridgePieceDirection}");
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

    public void HandleBridgeEntrances()
    {
        RemoveObsoleteBridgeEdges();

        if(BridgePieceDirection == BridgePieceDirection.Horizontal)
        {
            Tile tileLeft = Tile.Neighbours[ObjectDirection.Left];

            if (tileLeft != null)
            {
                if (tileLeft.TryGetBridgePiece() == null)
                {
                    GameObject bridgeEdgeGO = GameObject.Instantiate(_bridgeEdgePrefab, tileLeft.transform);

                    BridgeEdge bridgeEdge = bridgeEdgeGO.GetComponent<BridgeEdge>();
                    bridgeEdge.WithBridgeEdgeSide(Direction.Right);
                    bridgeEdge.WithBridgeType(BridgeType.Wooden);
                    bridgeEdge.SetSprite();
                    bridgeEdge.SetTile(tileLeft);

                    tileLeft.AddBridgeEdge(bridgeEdge);
                }
            }

            Tile tileRight = Tile.Neighbours[ObjectDirection.Right];

            if (tileRight != null)
            {
                if (tileRight.TryGetBridgePiece() == null)
                {
                    GameObject bridgeEdgeGO = GameObject.Instantiate(_bridgeEdgePrefab, tileRight.transform);

                    BridgeEdge bridgeEdge = bridgeEdgeGO.GetComponent<BridgeEdge>();
                    bridgeEdge.WithBridgeEdgeSide(Direction.Left);
                    bridgeEdge.WithBridgeType(BridgeType.Wooden);
                    bridgeEdge.SetSprite();
                    bridgeEdge.SetTile(tileRight);

                    tileRight.AddBridgeEdge(bridgeEdge);
                }
            }
        }
        else if(BridgePieceDirection == BridgePieceDirection.Vertical)
        {
            Tile tileDown = Tile.Neighbours[ObjectDirection.Down];

            if (tileDown != null)
            {
                if (tileDown.TryGetBridgePiece() == null)
                {
                    GameObject bridgeEdgeGO = GameObject.Instantiate(_bridgeEdgePrefab, tileDown.transform);

                    BridgeEdge bridgeEdge = bridgeEdgeGO.GetComponent<BridgeEdge>();
                    bridgeEdge.WithBridgeEdgeSide(Direction.Up);
                    bridgeEdge.WithBridgeType(BridgeType.Wooden);
                    bridgeEdge.SetSprite();
                    bridgeEdge.SetTile(tileDown);

                    tileDown.AddBridgeEdge(bridgeEdge);
                }
            }

            Tile tileUp = Tile.Neighbours[ObjectDirection.Up];

            if (tileUp != null)
            {
                if (tileUp.TryGetBridgePiece() == null)
                {
                    GameObject bridgeEdgeGO = GameObject.Instantiate(_bridgeEdgePrefab, tileUp.transform);

                    BridgeEdge bridgeEdge = bridgeEdgeGO.GetComponent<BridgeEdge>();
                    bridgeEdge.WithBridgeEdgeSide(Direction.Down);
                    bridgeEdge.WithBridgeType(BridgeType.Wooden);
                    bridgeEdge.SetSprite();
                    bridgeEdge.SetTile(tileUp);

                    tileUp.AddBridgeEdge(bridgeEdge);
                }
            }
        }
    }

    public void RemoveObsoleteBridgeEdges()
    {
        List<ITileAttribute> tileAttributes = Tile.GetAttributes();
        for (int i = 0; i < tileAttributes.Count; i++)
        {
            if(tileAttributes[i] is BridgeEdge)
            {
                tileAttributes[i].Remove();
            }
        }

        foreach (KeyValuePair<ObjectDirection, Tile> neighbour in Tile.Neighbours)
        {
            if (neighbour.Value == null) continue;

            switch (neighbour.Key)
            {
                case ObjectDirection.Down:
                    neighbour.Value.GetBridgeEdge(Direction.Up)?.Remove();
                    break;
                case ObjectDirection.Left:
                    neighbour.Value.GetBridgeEdge(Direction.Right)?.Remove();
                    break;
                case ObjectDirection.Right:
                    neighbour.Value.GetBridgeEdge(Direction.Left)?.Remove();
                    break;
                case ObjectDirection.Up:
                    neighbour.Value.GetBridgeEdge(Direction.Down)?.Remove();
                    break;
                default:
                    break;
            }
        }
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
