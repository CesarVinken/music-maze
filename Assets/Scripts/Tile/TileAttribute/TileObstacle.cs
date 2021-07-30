using System.Collections;
using UnityEngine;

public class TileObstacle : MonoBehaviour, ITileAttribute, ITileConnectable, ITransformable
{
    public Tile Tile;
    public string ParentId;
    
    public ObstacleType ObstacleType;

    [SerializeField] protected TileSpriteContainer _tileSpriteContainer;

    private int _connectionScore = -1;
    private int _spriteNumber = -1;

    private int _sortingOrderBase;
    private const float _sortingOrderCalculationOffset = 1.5f;
    protected int _sortingOrder;

    public int SortingOrderBase { get => _sortingOrderBase; set => _sortingOrderBase = value; }

    public int ConnectionScore { get => _connectionScore; set => _connectionScore = value; }
    public int SpriteNumber { get => _spriteNumber; set => _spriteNumber = value; }

    public virtual void Awake()
    {
        Guard.CheckIsNull(_tileSpriteContainer, "_tileSpriteContainer", gameObject);

        if(ObstacleType == ObstacleType.Bush)
        {
            _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.DefaultWall[0]);
        }

        _sortingOrderBase = SpriteSortingOrderRegister.TileObstacle;
        _sortingOrder = (int)(_sortingOrderBase - transform.position.y - _sortingOrderCalculationOffset) * 10;
        _tileSpriteContainer.SetSortingOrder(_sortingOrder);
    }

    public virtual void WithConnectionScoreInfo(TileConnectionScoreInfo obstacleConnectionScoreInfo)
    {
        ConnectionScore = obstacleConnectionScoreInfo.RawConnectionScore;
        SpriteNumber = obstacleConnectionScoreInfo.SpriteNumber;

        _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.DefaultWall[SpriteNumber - 1]);
    }

    public void WithObstacleType(ObstacleType obstacleType)
    {
        ObstacleType = obstacleType;
    }

    public void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;
    }
    
    public void Remove()
    {
        Destroy(this);
        Destroy(gameObject);
    }

    public string GetSubtypeAsString()
    {
        return ObstacleType.ToString();
    }

    public virtual void TriggerTransformation()
    {

        if (ObstacleType == ObstacleType.Bush)
        {
            Sprite colourfulSprite = MazeSpriteManager.Instance.DefaultWallColourful[SpriteNumber - 1];
            IEnumerator transformToColourful = TransformToColourful(colourfulSprite);
            StartCoroutine(transformToColourful);
        }
        else
        {
            Logger.Error($"Colourful mode not implemented for ObstacleType {ObstacleType}");
        }
    }

    public virtual IEnumerator TransformToColourful(Sprite colourfulSprite)
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

        // Fade out old image
        while (alphaAmount > 0)
        {
            alphaAmount = alphaAmount - (fadeSpeed * Time.deltaTime);
            _tileSpriteContainer.SetRendererAlpha(alphaAmount);
            yield return null;
        }

        TileSpriteContainerPool.Instance.ReturnToPool(_tileSpriteContainer);
        _tileSpriteContainer = transformedSpriteContainer;
    }
}
