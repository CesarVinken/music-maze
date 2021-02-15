using System.Collections;
using UnityEngine;

public class PlayerOnly : MonoBehaviour, ITileAttribute, ITransformable
{
    public Tile Tile;
    public string ParentId; 
    
    public PlayerOnlyType PlayerOnlyType;

    [SerializeField] private TileSpriteContainer _tileSpriteContainer;

    private int _sortingOrderBase = 500; // MAKE SURE that tile should be in front of tile marker and path layers AND player
    private const float _sortingOrderCalculationOffset = .5f;
    private int _sortingOrder;
    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    public void Awake()
    {
        Guard.CheckIsNull(_tileSpriteContainer, "_tileSpriteContainer", gameObject);

        if (PlayerOnlyType == PlayerOnlyType.Bush)
        {
            _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.Bush[0]);
        }

        _sortingOrder = (int)(_sortingOrderBase - transform.position.y - _sortingOrderCalculationOffset) * 10 + 1;
        _tileSpriteContainer.SetSortingOrder(_sortingOrder); // plus 1 should place it before a character when it is on the same y as the character

    }

    public void WithPlayerOnlyType(PlayerOnlyType playerOnlyType)
    {
        PlayerOnlyType = playerOnlyType;
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

        if (PlayerOnlyType == PlayerOnlyType.Bush)
        {
            Sprite colourfulSprite = MazeSpriteManager.Instance.BushColourful[0];
            IEnumerator transformToColourful = TransformToColourful(colourfulSprite);
            StartCoroutine(transformToColourful);
        }
        else
        {
            Logger.Error($"Colourful mode not implemented for PlayerOnlyType {PlayerOnlyType}");
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
