using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Transform BackgroundsContainer;
    public SpriteRenderer PlayerMarkRenderer;

    public string TileId;
    public bool Walkable = true;
    public bool Markable = true;
    public GridLocation GridLocation;
    public PlayerMark PlayerMark = null;

    [SerializeField] public List<IMazeTileBackground> MazeTileBackgrounds = new List<IMazeTileBackground>();
    [SerializeField] public List<IMazeTileAttribute> MazeTileAttributes = new List<IMazeTileAttribute>();
    public Dictionary<ObjectDirection, Tile> Neighbours = new Dictionary<ObjectDirection, Tile>();

    public void Awake()
    {
        Guard.CheckIsNull(BackgroundsContainer, "BackgroundsContainer", gameObject);

        if (transform.position.y < 0) Logger.Error("There is a tile at {0},{1}. Tiles cannot have negative Y values", transform.position.x, transform.position.y);
    }

    public void Start()
    {
        if (EditorManager.InEditor) return;

        if (!Markable) return;

        PlayerMark = new PlayerMark();

        if (MazeLevelManager.Instance.NumberOfUnmarkedTiles == -1)
        {
            MazeLevelManager.Instance.NumberOfUnmarkedTiles = 1;
        }
        else
        {
            MazeLevelManager.Instance.NumberOfUnmarkedTiles++;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Walkable) return;

        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        if (player != null)
        {
            //Logger.Log("{0} entered tile {1},{2}", player.name, GridLocation.X, GridLocation.Y);
            if (GameManager.Instance.GameType == GameType.Multiplayer && !player.PhotonView.IsMine) return;

            player.UpdateCurrentGridLocation(GridLocation);
            
            if (PlayerMarkRenderer.sprite != null) return;

            if (!Markable) return;

            MazeLevelManager.Instance.SetTileMarker(this, player);        
        }
    }

    public void SetId(string id)
    {
        TileId = id;
    }

    public void SetGridLocation(int x, int y)
    {
        GridLocation = new GridLocation(x, y);
    }

    public void InitialiseTileAttributes()
    {
        for (int i = 0; i < MazeTileAttributes.Count; i++)
        {
            MazeTileAttributes[i].SetTile(this);
        }
    }

    public void InitialiseTileBackgrounds()
    {
        for (int i = 0; i < MazeTileBackgrounds.Count; i++)
        {
            MazeTileBackgrounds[i].SetTile(this);
        }
    }

    public void SetBackgroundSprites()
    {
        



        //TODO: pass on connection score

        int connectionScore = 1;

        //TODO path sprite should not be set if there is no path.
        GameObject pathGO = Instantiate(MazeLevelManager.Instance.TilePathPrefab, transform);
        MazeTilePath mazeTilePath = pathGO.GetComponent<MazeTilePath>();
        mazeTilePath.WithPathConnectionScore(connectionScore);
        MazeTileBackgrounds.Add(mazeTilePath as IMazeTileBackground);

        if (connectionScore != 15) // TODO also don't add background for fully covering wall tiles
        {
            GameObject backgroundGO = Instantiate(MazeLevelManager.Instance.TileBackgroundPrefab, transform);
            MazeTileBackground background = backgroundGO.GetComponent<MazeTileBackground>();
            background.WithPathConnectionScore(-1); // background is currently always the default grass background. Connection score of -1 is temporary
            MazeTileBackgrounds.Add(background as IMazeTileBackground);
        }
    }

    public void AddNeighbours(MazeLevel level)
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

    public TileObstacle TryGetTileObstacle()
    {
        for (int i = 0; i < MazeTileAttributes.Count; i++)
        {
            Logger.Log($"found attribute for {GridLocation.X}, {GridLocation.Y} is {MazeTileAttributes[i].GetType()}");
        }
        TileObstacle tileObstacle = (TileObstacle)MazeTileAttributes.FirstOrDefault(attribute => attribute is TileObstacle);

        if (tileObstacle == null)
        {
            Logger.Log($"did not find a tile obstacle on {GridLocation.X},{GridLocation.Y}");
            return null;
        }
        Logger.Log($"found tileObstacle {tileObstacle.ObstacleType} on {GridLocation.X},{GridLocation.Y}");
        return tileObstacle;
    }

    public MazeTilePath TryGetTilePath()
    {
        //for (int i = 0; i < MazeTileBackgrounds.Count; i++)
        //{
        //    Logger.Log($"found background for {GridLocation.X}, {GridLocation.Y} is {MazeTileBackgrounds[i].GetType()}!");
        //}
        MazeTilePath mazeTilePath = (MazeTilePath)MazeTileBackgrounds.FirstOrDefault(background => background is MazeTilePath);
        
        if (mazeTilePath == null)
        {
            Logger.Log($"did NOT find a maze tile path on {GridLocation.X},{GridLocation.Y}");
            return null;
        }
        Logger.Log($"found maze tile path {mazeTilePath.MazeTilePathType} on {GridLocation.X},{GridLocation.Y} with score {mazeTilePath.PathConnectionScore}");
        return mazeTilePath;
    }
}
