using System.Collections;
using UnityEngine;

public class MazeTileBaseWater : MonoBehaviour, ITileBackground, ITransformable
{
    public Tile Tile;
    public string ParentId;

    [SerializeField] private TileSpriteContainer _tileSpriteContainer;

    private int _sortingOrder;

    public void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;

        _sortingOrder = SpriteManager.BaseBackgroundSortingOrder;
        _tileSpriteContainer.SetSortingOrder(_sortingOrder);

        Sprite sprite = MazeSpriteManager.Instance.DefaultMazeTileWater[0];
        _tileSpriteContainer.SetSprite(sprite);
    }

    public void Remove()
    {
        Destroy(this);
        Destroy(gameObject);
    }

    public void TriggerTransformation()
    {
        IEnumerator transformToColourful = TransformToColourful();
        StartCoroutine(transformToColourful);
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
