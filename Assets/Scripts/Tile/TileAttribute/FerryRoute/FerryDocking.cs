using System.Collections.Generic;
using UnityEngine;

public class FerryDocking : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public Tile DockingTile;

    private FerryRoute _ferryRoute;
    private int _currentSpriteNumber = 0;
    private Direction _dockingDirection;
    private FerryDockingType _dockingType;

    public void Initialise(FerryRoute ferryRoute, FerryDockingType dockingType, int dockingDirection)
    {
        _ferryRoute = ferryRoute;
        _dockingType = dockingType;
        _dockingDirection = dockingType == FerryDockingType.DockingStart ? DirectionHelper.DirectionByInt(dockingDirection) : DirectionHelper.OppositeDirection(DirectionHelper.DirectionByInt(dockingDirection)); ;
        UpdateDockingSprite();

        if (!EditorManager.InEditor)
        {
            List<FerryRoutePoint> ferryRoutePoints = _ferryRoute.GetFerryRoutePoints();
            if (dockingType == FerryDockingType.DockingEnd)
            {
                transform.position = ferryRoutePoints[ferryRoutePoints.Count - 1].Tile.transform.position;
                DockingTile = ferryRoutePoints[ferryRoutePoints.Count - 1].Tile;
            }
            else
            {
                DockingTile = ferryRoutePoints[0].Tile;
            }
        }

        SetActive(true);
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void UpdateDockingDirection()
    {
        List <FerryRoutePoint> ferryRoutePoints = _ferryRoute.GetFerryRoutePoints();
        Logger.Log($"update docking direction for docking type {_dockingType}");
        if (_dockingType == FerryDockingType.DockingStart)
        {
            if(ferryRoutePoints.Count < 2)
            {
                _dockingDirection = Direction.Right;
                return;
            }

            _dockingDirection = Direction.Right;
            Tile dockingTile = ferryRoutePoints[0].Tile;
            Tile neighbourTile;

            if (dockingTile.Neighbours.TryGetValue(Direction.Right, out neighbourTile))
            {
                if (neighbourTile.TileId.Equals(ferryRoutePoints[1].Tile.TileId))
                {
                    _dockingDirection = Direction.Left;
                    return;
                }
            }
            if (dockingTile.Neighbours.TryGetValue(Direction.Down, out neighbourTile))
            {
                if (neighbourTile.TileId.Equals(ferryRoutePoints[1].Tile.TileId))
                {
                    _dockingDirection = Direction.Up;
                    return;
                }
            }
            if (dockingTile.Neighbours.TryGetValue(Direction.Left, out neighbourTile))
            {
                if (neighbourTile.TileId.Equals(ferryRoutePoints[1].Tile.TileId))
                {
                    _dockingDirection = Direction.Right;
                    return;
                }
            }
            if (dockingTile.Neighbours.TryGetValue(Direction.Up, out neighbourTile))
            {
                if (neighbourTile.TileId.Equals(ferryRoutePoints[1].Tile.TileId))
                {
                    _dockingDirection = Direction.Down;
                    return;
                }
            }
            return;
        }
        else
        {
            FerryDocking ferryDockingStart = _ferryRoute.GetFerryDocking(FerryDockingType.DockingStart);
            _dockingDirection = DirectionHelper.OppositeDirection(ferryDockingStart.GetDockingDirection());
        }
    }

    public void UpdateDockingSprite()
    {
        switch (_dockingDirection)
        {
            case Direction.Right:
                _currentSpriteNumber = 0;
                break;
            case Direction.Down:
                _currentSpriteNumber = 1; 
                break;
            case Direction.Left:
                _currentSpriteNumber = 2;
                break;
            case Direction.Up:
                _currentSpriteNumber = 3;
                break;
            default:
                break;
        }

        _spriteRenderer.sprite = MazeSpriteManager.Instance.FerryRouteSprites[_currentSpriteNumber];
    }

    public void TryTurn()
    {
        bool hasTurned = false;

        while (!hasTurned)
        {
            IncreaseSpriteNumber();
            hasTurned = true;
        }

        _spriteRenderer.sprite = MazeSpriteManager.Instance.FerryRouteSprites[_currentSpriteNumber];
    }

    public Direction GetDockingDirection()
    {
        return _dockingDirection;
    }

    private void IncreaseSpriteNumber()
    {
        _currentSpriteNumber++;

        if (_currentSpriteNumber > 3)
        {
            _currentSpriteNumber = 0;
        }

    }
}