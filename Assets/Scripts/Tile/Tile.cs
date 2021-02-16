using System.Collections.Generic;
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

    //public abstract void InitialiseTileAttributes();
    //public abstract void InitialiseTileBackgrounds();

    public abstract TilePath TryGetTilePath();
    public abstract TileObstacle TryGetTileObstacle();

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
}
