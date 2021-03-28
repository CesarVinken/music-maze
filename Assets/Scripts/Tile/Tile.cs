using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    private bool _walkable = true;
    public Transform BackgroundsContainer;
    public string TileId;

    public GridLocation GridLocation;

    public Dictionary<ObjectDirection, Tile> Neighbours = new Dictionary<ObjectDirection, Tile>();

    public ITileMainMaterial TileMainMaterial;

    protected List<ITileAttribute> _tileAttributes = new List<ITileAttribute>();
    protected List<ITileBackground> _tileBackgrounds = new List<ITileBackground>();

    public bool Walkable { get => _walkable; protected set => _walkable = value; }

    public void Awake()
    {
        Guard.CheckIsNull(BackgroundsContainer, "BackgroundsContainer", gameObject);

        if (transform.position.y < 0) Logger.Error("There is a tile at {0},{1}. Tiles cannot have negative Y values", transform.position.x, transform.position.y);
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

    public List<ITileBackground> GetBackgrounds()
    {
        return _tileBackgrounds;
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

    public abstract TileObstacle TryGetTileObstacle();

    public TilePath TryGetTilePath()
    {
        TilePath tilePath = (TilePath)_tileBackgrounds.FirstOrDefault(background => background is TilePath);

        if (tilePath == null)
        {
            //Logger.Log($"did NOT find a tile path on {GridLocation.X},{GridLocation.Y}");
            return null;
        }
        //Logger.Log($"found tile path {tilePath.TilePathType} on {GridLocation.X},{GridLocation.Y} with score {tilePath.ConnectionScore}");
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
        //Add Right
        if (GridLocation.X < level.LevelBounds.X)
        {
            Neighbours.Add(ObjectDirection.Right, level.TilesByLocation[new GridLocation(GridLocation.X + 1, GridLocation.Y)]);
        } 
        else
        {
            Neighbours.Add(ObjectDirection.Right, null);
        }

        //Add Down
        if (GridLocation.Y > 0)
        {
            Neighbours.Add(ObjectDirection.Down, level.TilesByLocation[new GridLocation(GridLocation.X, GridLocation.Y - 1)]);
        }
        else
        {
            Neighbours.Add(ObjectDirection.Down, null);
        }
        //Add Left
        if (GridLocation.X > 0)
        {
            Neighbours.Add(ObjectDirection.Left, level.TilesByLocation[new GridLocation(GridLocation.X - 1, GridLocation.Y)]);
        }
        else
        {
            Neighbours.Add(ObjectDirection.Left, null);
        }
        //Add Up
        if (GridLocation.Y < level.LevelBounds.Y)
        {
            Neighbours.Add(ObjectDirection.Up, level.TilesByLocation[new GridLocation(GridLocation.X, GridLocation.Y + 1)]);
        }
        else
        {
            Neighbours.Add(ObjectDirection.Up, null);
        }
    }
}
