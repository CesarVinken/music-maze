using System.Collections;
using UnityEngine;

public class MazeTileBaseWater : TileWater, ITileBackground, ITransformable
{
    [SerializeField] private TileSpriteContainer _tileSpriteContainer;

    private int _sortingOrder;

    public override void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;

        _sortingOrder = SpriteSortingOrderRegister.BaseWaterSortingOrder;
        _tileSpriteContainer.SetSortingOrder(_sortingOrder);

        _currentWaterSpriteNumber = 0;
         Sprite sprite = MazeSpriteManager.Instance.DefaultMazeTileWater[_currentWaterSpriteNumber];
        _tileSpriteContainer.SetSprite(sprite);

        IEnumerator animateWaterCoroutine = AnimateWater();
        StartCoroutine(animateWaterCoroutine);
    }

    public void TriggerTransformation()
    {
        IEnumerator transformToColourful = TransformToColourful();
        StartCoroutine(transformToColourful);
    }

    private IEnumerator AnimateWater()
    {
        _animateWater = true;
        yield return new WaitForSeconds(1);

        while (_animateWater)
        {
            _currentWaterSpriteNumber = GetNextWaterSpriteNumber();
            _tileSpriteContainer.SetSprite(MazeSpriteManager.Instance.DefaultMazeTileWater[_currentWaterSpriteNumber]);

            yield return new WaitForSeconds(1);
        }
    }

    private int GetNextWaterSpriteNumber()
    {
        if(_currentWaterSpriteNumber < MazeSpriteManager.Instance.DefaultMazeTileWater.Length - 1)
        {
            return _currentWaterSpriteNumber + 1;
        }
        else
        {
            return 0;
        }
    }

    public IEnumerator TransformToColourful()
    {
        Sprite colourfulSprite = MazeSpriteManager.Instance.DefaultMazeTileWaterColourful[0];

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
