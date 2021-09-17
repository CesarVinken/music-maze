using System.Collections.Generic;
using UnityEngine;

public class FerryDocking : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private FerryRoute _ferryRoute;
    private int _currentSpriteNumber = 0;
    private Direction _dockingDirection;
    private FerryDockingType _dockingType;

    public void Initialise(FerryRoute ferryRoute, FerryDockingType dockingType)
    {
        _ferryRoute = ferryRoute;
        _dockingType = dockingType;
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    private Direction FindDockingDirection()
    {
        List<FerryRoutePoint> ferryRoutePoints = _ferryRoute.GetFerryRoutePoints();
        Tile neighbourTile;
        Tile dockingTile = ferryRoutePoints[0].Tile;

        if (_dockingType == FerryDockingType.DockingStart)
        {
            dockingTile = ferryRoutePoints[0].Tile;
            if (dockingTile.Neighbours.TryGetValue(Direction.Right, out neighbourTile))
            {
                if (neighbourTile != null && neighbourTile.Walkable && neighbourTile.TileMainMaterial is GroundMainMaterial)
                {
                    return Direction.Right;
                }
            }
            if (dockingTile.Neighbours.TryGetValue(Direction.Down, out neighbourTile))
            {
                if (neighbourTile != null && neighbourTile.Walkable && neighbourTile.TileMainMaterial is GroundMainMaterial)
                {
                    return Direction.Down;
                }
            }
            if (dockingTile.Neighbours.TryGetValue(Direction.Left, out neighbourTile))
            {
                if (neighbourTile != null && neighbourTile.Walkable && neighbourTile.TileMainMaterial is GroundMainMaterial)
                {
                    return Direction.Left;
                }
            }
            if (dockingTile.Neighbours.TryGetValue(Direction.Up, out neighbourTile))
            {
                if (neighbourTile != null && neighbourTile.Walkable && neighbourTile.TileMainMaterial is GroundMainMaterial)
                {
                    return Direction.Up;
                }
            }
            return Direction.Right;
        }

        // It is the second docking point
        dockingTile = ferryRoutePoints[ferryRoutePoints.Count - 1].Tile;
        Tile previousPointTile = ferryRoutePoints[ferryRoutePoints.Count - 2].Tile;

        Logger.Log($"previousPointTile at {previousPointTile.GridLocation.X}, {previousPointTile.GridLocation.Y}");
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
                        Logger.Warning($"Return {neighboursOfNeighbour.Key}");
                        return neighboursOfNeighbour.Key;
                    }
                }

                // If the desired docking direction is not walkable try to find another neighbour land tile that is walkable
                foreach (KeyValuePair<Direction, Tile> dockingTileNeighbour in dockingTile.Neighbours)
                {
                    if (dockingTileNeighbour.Value.Walkable && dockingTileNeighbour.Value.TileMainMaterial is GroundMainMaterial)
                    {
                        Logger.Warning($"Return {dockingTileNeighbour.Key}");
                        return dockingTileNeighbour.Key;
                    }
                }
            }
        }
        Logger.Warning($"Return BLURGGH");

        return Direction.Right;
    }

    public void UpdateDockingSprite()
    {
        _dockingDirection = FindDockingDirection();

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
        Logger.Log("Try turning");
        bool hasTurned = false;

        while (!hasTurned)
        {
            IncreaseSpriteNumber();
            hasTurned = true;
        }

        _spriteRenderer.sprite = MazeSpriteManager.Instance.FerryRouteSprites[_currentSpriteNumber];

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