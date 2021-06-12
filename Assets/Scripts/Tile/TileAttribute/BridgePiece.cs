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

    public List<BridgeEdge> BridgeEdgeConnections = new List<BridgeEdge>();

    private int _sortingOrderBase = 0; 
    private int _sortingOrder; // TODO: Back piece should be behind characters, front piece should be in front of characters.

    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    public void Awake()
    {
        Guard.CheckIsNull(_tileSpriteContainer, "_tileSpriteContainer", gameObject);
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

    public void AddBridgeEdgeConnection(BridgeEdge bridgeEdge)
    {
        BridgeEdgeConnections.Add(bridgeEdge);
    }

    public void RemoveBridgeEdgeConnection(BridgeEdge bridgeEdge)
    {
        BridgeEdgeConnections.Remove(bridgeEdge);
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

    public void HandleBridgeEdges()
    {
        RemoveObsoleteBridgeEdges();

        if(BridgePieceDirection == BridgePieceDirection.Horizontal)
        {
            Tile.AddBridgeEdge(this, ObjectDirection.Left, Direction.Right);
            Tile.AddBridgeEdge(this, ObjectDirection.Right, Direction.Left);
        }
        else if(BridgePieceDirection == BridgePieceDirection.Vertical)
        {
            Tile.AddBridgeEdge(this, ObjectDirection.Down, Direction.Up);
            Tile.AddBridgeEdge(this, ObjectDirection.Up, Direction.Down);
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
                    neighbour.Value?.GetBridgeEdge(Direction.Up)?.Remove();
                    break;
                case ObjectDirection.Left:
                    neighbour.Value?.GetBridgeEdge(Direction.Right)?.Remove();
                    break;
                case ObjectDirection.Right:
                    Logger.Warning($"neighbour.Value?.GetBridgeEdge(Direction.Left) {neighbour.Value?.GetBridgeEdge(Direction.Left)}");
                    neighbour.Value?.GetBridgeEdge(Direction.Left)?.Remove();
                    break;
                case ObjectDirection.Up:
                    neighbour.Value?.GetBridgeEdge(Direction.Down)?.Remove();
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
            Sprite colourfulBridgePieceSprite = MazeSpriteManager.Instance.WoodenBridgeColourful[0];
            if(BridgePieceDirection == BridgePieceDirection.Vertical)
            {
                colourfulBridgePieceSprite = MazeSpriteManager.Instance.WoodenBridgeColourful[1];
            }

            IEnumerator transformToColourful = TransformToColourful(colourfulBridgePieceSprite);
            StartCoroutine(transformToColourful);

            for (int i = 0; i < BridgeEdgeConnections.Count; i++)
            {
                BridgeEdge connectedBridgeEdge = BridgeEdgeConnections[i];
                connectedBridgeEdge.TriggerTransformation();
            }
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
