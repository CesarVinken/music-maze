﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TileCorner
{
    RightUp,
    RightDown,
    LeftDown,
    LeftUp
}

public abstract class Tile : MonoBehaviour
{
    [SerializeField] private bool _walkable = true;
    public Transform BackgroundsContainer;
    public string TileId;

    public GridLocation GridLocation;

    public Dictionary<Direction, Tile> Neighbours = new Dictionary<Direction, Tile>();
    public Dictionary<Direction, PathNode> PathNodeNeighbours = new Dictionary<Direction, PathNode>();

    public ITileMainMaterial TileMainMaterial;

    protected List<ITileAttribute> _tileAttributes = new List<ITileAttribute>();
    protected List<ITileBackground> _tileBackgrounds = new List<ITileBackground>();
    protected List<TileCornerFiller> _tileCornerFillers = new List<TileCornerFiller>();
    protected List<TileArea> _tileAreas = new List<TileArea>();

    public PathNode PathNode;

    public bool Walkable { get => _walkable; protected set => _walkable = value; }

    public void Awake()
    {
        Guard.CheckIsNull(BackgroundsContainer, "BackgroundsContainer", gameObject);

        if (transform.position.y < 0) Logger.Error("There is a tile at {0},{1}. Tiles cannot have negative Y values", transform.position.x, transform.position.y);

        PathNode = new PathNode();
        PathNode.SetTile(this);
    }

    public void Start()
    {
        if (EditorManager.InEditor) return;
    }

    public void SetId(string id)
    {
        TileId = id;
    }

    public void SetGridLocation(int x, int y)
    {
        GridLocation = new GridLocation(x, y);
    }

    public void SetWalkable(bool isWalkable)
    {
        Walkable = isWalkable;
    }

    public void SetMainMaterial(ITileMainMaterial tileMainMaterial)
    {
        TileMainMaterial = tileMainMaterial;
    }

    public void AddBackground(ITileBackground tileBackground)
    {
        _tileBackgrounds.Add(tileBackground);
    }

    public void AddBridgeEdge(BridgeEdge bridgeEdge)
    {
        _tileAttributes.Add(bridgeEdge);
    }

    public List<BridgeEdge> GetBridgeEdges()
    {
        List<BridgeEdge> bridgeEdges = new List<BridgeEdge>();
        for (int i = 0; i < _tileAttributes.Count; i++)
        {
            if (_tileAttributes[i] is BridgeEdge)
            {
                bridgeEdges.Add(_tileAttributes[i] as BridgeEdge);
            }
        }
        return bridgeEdges;
    }

    public BridgeEdge GetBridgeEdge(Direction edgeSide)
    {
        List<BridgeEdge> bridgeEdges = GetBridgeEdges();

        for (int i = 0; i < bridgeEdges.Count; i++)
        {
            if(bridgeEdges[i].EdgeSide == edgeSide)
            {
                return bridgeEdges[i];
            }
        }
        return null;
    }

    public void RemoveBridgeEdge(BridgeEdge bridgeEdge)
    {
        _tileAttributes.Remove(bridgeEdge);
    }

    public List<ITileBackground> GetBackgrounds()
    {
        return _tileBackgrounds;
    }

    public List<TileCornerFiller> GetCornerFillers()
    {
        return _tileCornerFillers;
    }

    public void AddTileArea(TileArea tileArea)
    {
        _tileAreas.Add(tileArea);
    }

    public TileArea GetTileArea(TileArea tileArea)
    {
        List<TileArea> tileAreas = GetTileAreas();

        for (int i = 0; i < tileAreas.Count; i++)
        {
            if (tileAreas[i] == tileArea)
            {
                return tileAreas[i];
            }
        }
        return null;
    }


    public List<TileArea> GetTileAreas()
    {
        return _tileAreas;
    }

    public void RemoveTileArea(TileArea tileArea)
    {
        _tileAreas.Remove(tileArea);
    }

    public void RemoveBackground(ITileBackground tileBackground)
    {
        _tileBackgrounds.Remove(tileBackground);
    }

    public void AddAttribute(ITileAttribute tileAttribute)
    {
        _tileAttributes.Add(tileAttribute);
    }

    public List<ITileAttribute> GetAttributes()
    {
        return _tileAttributes;
    }

    public void RemoveAttribute(ITileAttribute tileAttribute)
    {
        _tileAttributes.Remove(tileAttribute);
    }

    public void AddCornerFiller(TileCornerFiller cornerFiller)
    {
        _tileCornerFillers.Add(cornerFiller);
    }

    public void TryRemoveCornerFiller(TileCorner tileCorner)
    {
        TileCornerFiller cornerFiller = TryGetCornerFiller(tileCorner);

        if (cornerFiller)
        {
            _tileCornerFillers.Remove(cornerFiller);
            cornerFiller.Remove();
        }
    }

    public TilePath TryGetTilePath()
    {
        TilePath tilePath = (TilePath)_tileBackgrounds.FirstOrDefault(background => background is TilePath);

        if (tilePath == null)
        {
            return null;
        }
        return tilePath;
    }

    public TileWater TryGetTileWater()
    {
        TileWater tileWater = (TileWater)_tileBackgrounds.FirstOrDefault(background => background is TileWater);

        if (tileWater == null)
        {
            return null;
        }

        return tileWater;
    }

    public TileBaseGround TryGetTileGround()
    {
        TileBaseGround tileGround = (TileBaseGround)_tileBackgrounds.FirstOrDefault(background => background is TileBaseGround);

        if (tileGround == null)
        {
            return null;
        }

        return tileGround;
    }

    public T TryGetAttribute<T>() where T : MonoBehaviour, ITileAttribute
    {
        T attribute = (T)_tileAttributes.FirstOrDefault(a => a is T);

        if (attribute == null)
        {
            return null;
        }

        return attribute;
    }

    //  Retrieves any MusicInstrumentCase, Sheetmusic, etc..
    public IScoreAttribute TryGetScoreAttribute()
    {
        IScoreAttribute attribute = (IScoreAttribute)_tileAttributes.FirstOrDefault(a => a is IScoreAttribute);

        if (attribute == null)
        {
            return null;
        }

        return attribute;
    }

    public TileCornerFiller TryGetCornerFiller(TileCorner tileCorner)
    {
        TileCornerFiller cornerFiller = (TileCornerFiller)_tileCornerFillers.FirstOrDefault(background => background is TileCornerFiller && background.TileCorner == tileCorner);

        if (cornerFiller == null)
        {
            return null;
        }

        return cornerFiller;
    }

    public void InitialiseTileBackgrounds()
    {
        for (int i = 0; i < _tileBackgrounds.Count; i++)
        {
            _tileBackgrounds[i].SetTile(this);
        }
    }

    public void InitialiseTileAttributes()
    {
        for (int i = 0; i < _tileAttributes.Count; i++)
        {
            _tileAttributes[i].SetTile(this);
        }
    }

    public void AddNeighbours<T>(T level) where T : IGameScene<Tile>
    {
        //Add RIGHT
        if (GridLocation.X < level.LevelBounds.X)
        {
            //TODO:: Find an efficient way if a tile is a point on a ferryRoute
            Tile neighbourTile = level.TilesByLocation[new GridLocation(GridLocation.X + 1, GridLocation.Y)];
            Neighbours.Add(Direction.Right, neighbourTile);
            TryAddTileNode(Direction.Right, neighbourTile);
        }
        else
        {
            Neighbours.Add(Direction.Right, null);
        }

        //Add DOWN
        if (GridLocation.Y > 0)
        {
            Tile neighbourTile = level.TilesByLocation[new GridLocation(GridLocation.X, GridLocation.Y - 1)];
            Neighbours.Add(Direction.Down, neighbourTile);
            TryAddTileNode(Direction.Down, neighbourTile);
        }
        else
        {
            Neighbours.Add(Direction.Down, null);
        }
        //Add LEFT
        if (GridLocation.X > 0)
        {
            Tile neighbourTile = level.TilesByLocation[new GridLocation(GridLocation.X - 1, GridLocation.Y)];
            Neighbours.Add(Direction.Left, neighbourTile);
            TryAddTileNode(Direction.Left, neighbourTile);
        }
        else
        {
            Neighbours.Add(Direction.Left, null);
        }
        //Add UP
        if (GridLocation.Y < level.LevelBounds.Y)
        {
            Tile neighbourTile = level.TilesByLocation[new GridLocation(GridLocation.X, GridLocation.Y + 1)];
            Neighbours.Add(Direction.Up, neighbourTile);
            TryAddTileNode(Direction.Up, neighbourTile);
        }
        else
        {
            Neighbours.Add(Direction.Up, null);
        }
    }

    private void TryAddTileNode(Direction direction, Tile neighbourTile)
    {
        if (Walkable && neighbourTile.Walkable)
        {
            PathNodeNeighbours.Add(direction, neighbourTile.PathNode);
            return;
        }

        // Water tiles on ferry routes are possibly walkable
        List<FerryRoute> ferryRoutes = GameManager.Instance.CurrentGameLevel.FerryRoutes;

        for (int i = 0; i < ferryRoutes?.Count; i++)
        {
            List<FerryRoutePoint> ferryRoutePoints = ferryRoutes[i].GetFerryRoutePoints();
            FerryRoutePoint ferryRoutePointThisTile = null;
            FerryRoutePoint ferryRoutePointNeighbourTile = null;
            bool thisTileIsDockingPlace = false;
            bool neighbourTileIsDockingPlace = false;

            for (int j = 0; j < ferryRoutePoints.Count; j++)
            {
                GridLocation ferryRoutePointLocation = ferryRoutePoints[j].Tile.GridLocation;
                if(ferryRoutePointLocation.X == GridLocation.X && ferryRoutePointLocation.Y == GridLocation.Y)
                {
                    ferryRoutePointThisTile = ferryRoutePoints[j];
                    if(j == 0 || j == ferryRoutePoints.Count - 1)
                    {
                        thisTileIsDockingPlace = true;
                    }
                }
                if (ferryRoutePointLocation.X == neighbourTile.GridLocation.X && ferryRoutePointLocation.Y == neighbourTile.GridLocation.Y)
                {
                    ferryRoutePointNeighbourTile = ferryRoutePoints[j];
                    if(j == 0 || j == ferryRoutePoints.Count - 1)
                    {
                        neighbourTileIsDockingPlace = true;
                    }
                }
            }

            if(ferryRoutePointThisTile != null && ferryRoutePointNeighbourTile != null)
            {
                // We add a pathnode because we established that WE are a ferryRoutePoint, and the neighbour is a ferryRoutePoint on the same route as well!
                PathNodeNeighbours.Add(direction, neighbourTile.PathNode);
            }
            else if ((neighbourTileIsDockingPlace && ferryRoutePointThisTile == null && Walkable) ||
                thisTileIsDockingPlace && ferryRoutePointNeighbourTile == null && neighbourTile.Walkable)
            {
                Logger.Log($"We want to add direction {direction} for the tile at {GridLocation.X}, {GridLocation.Y}");
                // We add a pathnoed because we establised that the we are at the intersection between a docking place and the next (walkable) tile
                PathNodeNeighbours.Add(direction, neighbourTile.PathNode);
            }
        }

    }

    public void AddBridgeEdge(BridgePiece bridgePieceConnection, Direction neighbourSide, Direction edgeSide)
    {
        Tile neighbourTile = Neighbours[neighbourSide];

        if (neighbourTile != null)
        {
            if (neighbourTile.TryGetAttribute<BridgeEdge>() == null)
            {
                GameObject bridgeEdgeGO = GameObject.Instantiate(GameManager.Instance.GameplayManager.GetTileAttributePrefab<BridgeEdge>(), neighbourTile.transform);

                BridgeEdge bridgeEdge = bridgeEdgeGO.GetComponent<BridgeEdge>();
                bridgeEdge.WithBridgeEdgeSide(edgeSide);
                bridgeEdge.WithBridgeType(BridgeType.Wooden);
                bridgeEdge.WithBridgePieceConnection(bridgePieceConnection);
                bridgeEdge.SetSprite();
                bridgeEdge.SetTile(neighbourTile);

                neighbourTile.AddBridgeEdge(bridgeEdge);
            }
        }
    }
}
