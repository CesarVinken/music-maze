﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public bool Walkable = true;
    public Transform BackgroundsContainer;
    public string TileId;

    public GridLocation GridLocation;

    public Dictionary<ObjectDirection, Tile> Neighbours = new Dictionary<ObjectDirection, Tile>();

    [SerializeField] public List<ITileBackground> TileBackgrounds = new List<ITileBackground>();
    [SerializeField] public List<ITileAttribute> TileAttributes = new List<ITileAttribute>();

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

    public abstract TileObstacle TryGetTileObstacle();

    public TilePath TryGetTilePath()
    {
        TilePath tilePath = (TilePath)TileBackgrounds.FirstOrDefault(background => background is TilePath);

        if (tilePath == null)
        {
            Logger.Log($"did NOT find a tile path on {GridLocation.X},{GridLocation.Y}");
            return null;
        }
        Logger.Log($"found tile path {tilePath.TilePathType} on {GridLocation.X},{GridLocation.Y} with score {tilePath.ConnectionScore}");
        return tilePath;
    }

    public void InitialiseTileAttributes()
    {
        for (int i = 0; i < TileAttributes.Count; i++)
        {
            TileAttributes[i].SetTile(this);
        }
    }

    public void InitialiseTileBackgrounds()
    {
        for (int i = 0; i < TileBackgrounds.Count; i++)
        {
            TileBackgrounds[i].SetTile(this);
        }
    }

    public void AddNeighbours(IEditorLevel level)
    {
        //Add Right
        if (GridLocation.X < level.LevelBounds.X)
        {
            Neighbours.Add(ObjectDirection.Right, level.TilesByLocation[new GridLocation(GridLocation.X + 1, GridLocation.Y)]);
        }

        //Add Down
        if (GridLocation.Y > 0)
        {
            Neighbours.Add(ObjectDirection.Down, level.TilesByLocation[new GridLocation(GridLocation.X, GridLocation.Y - 1)]);
        }

        //Add Left
        if (GridLocation.X > 0)
        {
            Neighbours.Add(ObjectDirection.Left, level.TilesByLocation[new GridLocation(GridLocation.X - 1, GridLocation.Y)]);
        }

        //Add Up
        if (GridLocation.Y < level.LevelBounds.Y)
        {
            Neighbours.Add(ObjectDirection.Up, level.TilesByLocation[new GridLocation(GridLocation.X, GridLocation.Y + 1)]);
        }
    }
}
