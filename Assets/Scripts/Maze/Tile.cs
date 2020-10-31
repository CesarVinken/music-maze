using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public Sprite Sprite;
    public SpriteRenderer PlayerMark;

    public string TileId;
    public bool Walkable = true; // TODO Automatically set value in Maze Level Editor
    public bool Markable = true;
    public GridLocation GridLocation;

    public List<IMazeTileAttribute> MazeTileAttributes = new List<IMazeTileAttribute>();

    public void Awake()
    {
        Guard.CheckIsNull(SpriteRenderer, "SpriteRenderer", gameObject);

        if (transform.position.y < 0) Logger.Error("There is a tile at {0},{1}. Tiles cannot have negative Y values", transform.position.x, transform.position.y);
        //Sprite = SpriteRenderer.sprite;

        //GridLocation = GridLocation.VectorToGrid(transform.position);// TODO REMOVE
        //gameObject.name = "Tile" + GridLocation.X + ", " + GridLocation.Y;
    }

    public void Start()
    {
        if (EditorManager.InEditor) return;

        if (!Walkable)
        {
            Markable = false;
            //MazeLevelManager.Instance.Level.AddUnwalkableTile(this);
        } else
        {
            if (!Markable) return;

            if (MazeLevelManager.Instance.Level.NumberOfUnmarkedTiles == -1)
            {
                MazeLevelManager.Instance.Level.NumberOfUnmarkedTiles = 1;
            }
            else
            {
                MazeLevelManager.Instance.Level.NumberOfUnmarkedTiles++;
            }
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
            
            if (PlayerMark.sprite != null) return;

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

    public void PlacePlayerExit()
    {
        GameObject playerExitGO = Instantiate(MazeLevelManager.Instance.PlayerExitPrefab, transform);
        PlayerExit playerExit = playerExitGO.GetComponent<PlayerExit>();
        playerExit.SetTile(this);
        Walkable = false;
        MazeTileAttributes.Add(playerExit);
    }

    public void RemovePlayerExit()
    {
        Walkable = true;
        IMazeTileAttribute playerExit = (PlayerExit)MazeTileAttributes.FirstOrDefault(attribute => attribute is PlayerExit);
        if (playerExit == null) return;
        MazeTileAttributes.Remove(playerExit);
        playerExit.Remove();
    }

    public void PlaceTileObstacle()
    {
        GameObject tileObstacleGO = Instantiate(MazeLevelManager.Instance.TileObstaclePrefab, transform);
        TileObstacle tileObstacle = tileObstacleGO.GetComponent<TileObstacle>();
        tileObstacle.SetTile(this);
        Walkable = false;
        MazeTileAttributes.Add(tileObstacle);
    }

    public void RemoveTileObstacle()
    {
        Walkable = true;
        IMazeTileAttribute tileObstacle = (TileObstacle)MazeTileAttributes.FirstOrDefault(attribute => attribute is TileObstacle);
        if (tileObstacle == null) return;
        MazeTileAttributes.Remove(tileObstacle);
        tileObstacle.Remove();
    }

    public void PlacePlayerSpawnpoint()
    {
        GameObject playerSpawnpointGO = Instantiate(MazeLevelManager.Instance.PlayerSpawnpointPrefab, transform);
        PlayerSpawnpoint playerSpawnpoint = playerSpawnpointGO.GetComponent<PlayerSpawnpoint>();
        playerSpawnpoint.SetTile(this);

        MazeTileAttributes.Add(playerSpawnpoint);
    }

    public void RemovePlayerSpawnpoint()
    {
        IMazeTileAttribute playerSpawnpoint = (PlayerSpawnpoint)MazeTileAttributes.FirstOrDefault(attribute => attribute is PlayerSpawnpoint);
        if (playerSpawnpoint == null) return;
        MazeTileAttributes.Remove(playerSpawnpoint);
        playerSpawnpoint.Remove();
    }

    public void PlaceEnemySpawnpoint()
    {
        GameObject enemySpawnpointGO = Instantiate(MazeLevelManager.Instance.EnemySpawnpointPrefab, transform);
        EnemySpawnpoint enemySpawnpoint = enemySpawnpointGO.GetComponent<EnemySpawnpoint>();
        enemySpawnpoint.SetTile(this);

        MazeTileAttributes.Add(enemySpawnpoint);
    }

    public void RemoveEnemySpawnpoint()
    {
        IMazeTileAttribute enemySpawnpoint = (EnemySpawnpoint)MazeTileAttributes.FirstOrDefault(attribute => attribute is EnemySpawnpoint);
        if (enemySpawnpoint == null) return;
        MazeTileAttributes.Remove(enemySpawnpoint);
        enemySpawnpoint.Remove();
    }

    public void SetSprite()
    {
        Sprite = SpriteRenderer.sprite;
    }
}
