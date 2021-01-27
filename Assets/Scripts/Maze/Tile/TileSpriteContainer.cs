using System.Collections;
using UnityEngine;

public class TileSpriteContainer : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;

    public void SetSprite(Sprite sprite)
    {
        SpriteRenderer.sprite = sprite;
    }

    public void SetSortingOrder(int sortingOrder)
    {
        SpriteRenderer.sortingOrder = sortingOrder;
    }

    public void SetRendererAlpha(float alphaValue)
    {
        Color color = SpriteRenderer.color;
        SpriteRenderer.color = new Color(color.r, color.g, color.b, alphaValue);
    }
}
