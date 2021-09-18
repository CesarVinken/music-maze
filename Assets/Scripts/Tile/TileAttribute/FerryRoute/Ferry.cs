using System.Collections.Generic;
using UnityEngine;

public class Ferry : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public FerryDirection FerryDirection;

    public void SetDirection(FerryDirection ferryDirection)
    {
        FerryDirection = ferryDirection;

        switch (FerryDirection)
        {
            case FerryDirection.Horizontal:
                _spriteRenderer.sprite = MazeSpriteManager.Instance.FerryRouteSprites[4];
                break;
            case FerryDirection.Vertical:
                _spriteRenderer.sprite = MazeSpriteManager.Instance.FerryRouteSprites[5];
                break;
            default:
                Logger.Error($"Unknown ferry direction {ferryDirection}");
                break;
        }
    }
}
