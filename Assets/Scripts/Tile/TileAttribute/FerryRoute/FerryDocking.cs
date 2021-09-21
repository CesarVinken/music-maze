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
        _dockingDirection = DockingDirectionFromInt(dockingDirection);
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
        Tile neighbourTile;
        Tile dockingTile = ferryRoutePoints[0].Tile;
        _dockingDirection = Direction.Right;

        if (_dockingType == FerryDockingType.DockingStart)
        {
            dockingTile = ferryRoutePoints[0].Tile;

            if (dockingTile.Neighbours.TryGetValue(Direction.Right, out neighbourTile))
            {
                if (neighbourTile != null && neighbourTile.Walkable && neighbourTile.TileMainMaterial is GroundMainMaterial)
                {
                    _dockingDirection = Direction.Right;
                    return;
                }
            }
            if (dockingTile.Neighbours.TryGetValue(Direction.Down, out neighbourTile))
            {
                if (neighbourTile != null && neighbourTile.Walkable && neighbourTile.TileMainMaterial is GroundMainMaterial)
                {
                    _dockingDirection = Direction.Down;
                    return;
                }
            }
            if (dockingTile.Neighbours.TryGetValue(Direction.Left, out neighbourTile))
            {
                if (neighbourTile != null && neighbourTile.Walkable && neighbourTile.TileMainMaterial is GroundMainMaterial)
                {
                    _dockingDirection = Direction.Left;
                    return;
                }
            }
            if (dockingTile.Neighbours.TryGetValue(Direction.Up, out neighbourTile))
            {
                if (neighbourTile != null && neighbourTile.Walkable && neighbourTile.TileMainMaterial is GroundMainMaterial)
                {
                    _dockingDirection = Direction.Up;
                    return;
                }
            }
            return;
        }

        // It is the second docking point
        dockingTile = ferryRoutePoints[ferryRoutePoints.Count - 1].Tile;
        Tile previousPointTile = ferryRoutePoints[ferryRoutePoints.Count - 2].Tile;

        foreach (KeyValuePair<Direction, Tile> neighboursOfNeighbour in previousPointTile.Neighbours)
        {
            string tileIdOfPossibleDockingTile = neighboursOfNeighbour.Value.TileId;

            if (tileIdOfPossibleDockingTile.Equals(dockingTile.TileId))
            {
                // We found that the tile opposite to the last way point. Try if it is walkable
                if (dockingTile.Neighbours.TryGetValue(neighboursOfNeighbour.Key, out neighbourTile))
                {
                    if (neighbourTile != null && neighbourTile.Walkable && neighbourTile.TileMainMaterial is GroundMainMaterial)
                    {
                        _dockingDirection = neighboursOfNeighbour.Key;
                        return;
                    }
                }

                // If the desired docking direction is not walkable try to find another neighbour land tile that is walkable
                foreach (KeyValuePair<Direction, Tile> dockingTileNeighbour in dockingTile.Neighbours)
                {
                    if (dockingTileNeighbour.Value.Walkable && dockingTileNeighbour.Value.TileMainMaterial is GroundMainMaterial)
                    {
                        _dockingDirection = dockingTileNeighbour.Key;
                        return;
                    }
                }
            }
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

    private Direction DockingDirectionFromInt(int dockingDirection)
    {
        switch (dockingDirection)
        {
            case 0:
                return Direction.Right;
            case 1:
                return Direction.Down;
            case 2:
                return Direction.Left;
            case 3:
                return Direction.Up;
            default:
                return Direction.Right;
        }
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