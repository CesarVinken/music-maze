using System.Collections;
using UnityEngine;

public class MazeTileBaseGround : TileBaseGround, ITransformable
{
    public override void SetTile(Tile tile)
    {
        if (string.IsNullOrEmpty(tile.TileId)) Logger.Error("This tile does not have an Id");

        Tile = tile;
        ParentId = tile.TileId;

        _sortingOrder = SpriteSortingOrderRegister.BaseGroundSortingOrder;
        _tileSpriteContainer.SetSortingOrder(_sortingOrder);
    }

    public override void WithConnectionScoreInfo(TileConnectionScoreInfo connectionScoreInfo)
    {
        ConnectionScore = connectionScoreInfo.RawConnectionScore;
        SpriteNumber = connectionScoreInfo.SpriteNumber;

        _sprite = MazeSpriteManager.Instance.DefaultMazeTileGround[SpriteNumber - 1];
        _tileSpriteContainer.SetSprite(_sprite);
    }

    public void TriggerTransformation()
    {
        IEnumerator transformToColourful = TransformToColourful();
        StartCoroutine(transformToColourful);
    }

    public IEnumerator TransformToColourful()
    {
        Sprite colourfulSprite = MazeSpriteManager.Instance.DefaultMazeTileGroundColourful[SpriteNumber - 1];

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
