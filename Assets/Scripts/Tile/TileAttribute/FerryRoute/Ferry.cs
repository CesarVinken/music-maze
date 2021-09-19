using System.Collections.Generic;
using UnityEngine;

public class Ferry : MonoBehaviour
{
    public static List<Ferry> Ferries = new List<Ferry>();
        
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public FerryDirection FerryDirection;
    public MazeTile CurrentLocationTile = null;

    public void Initialise()
    {
        GridLocation location = GridLocation.FindClosestGridTile(transform.position);
        SetNewCurrentLocation(location);
        Ferries.Add(this);
    }

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

    public void SetNewCurrentLocation(GridLocation newCurrentLocation)
    {
        if (CurrentLocationTile != null)
        {
            CurrentLocationTile.SetWalkable(false);
        }

        CurrentLocationTile = GameManager.Instance.CurrentGameLevel.TilesByLocation[newCurrentLocation] as MazeTile;
        CurrentLocationTile.SetWalkable(true);
    }
}
