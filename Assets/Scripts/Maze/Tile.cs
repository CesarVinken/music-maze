using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public Sprite Sprite;
    public SpriteRenderer PlayerMark;

    public bool Walkable = true; // TODO Automatically set value in Maze Level Editor
    public bool Markable = true;
    public GridLocation GridLocation;

    public void Awake()
    {
        if (SpriteRenderer == null)
            Logger.Error(Logger.Initialisation, "Could not find sprite renderer on tile prefab");

        if (transform.position.y < 0) Logger.Error("There is a tile at {0},{1}. Tiles cannot have negative Y values", transform.position.x, transform.position.y);
        Sprite = SpriteRenderer.sprite;

        GridLocation = GridLocation.VectorToGrid(transform.position);
        gameObject.name = "Tile" + GridLocation.X + ", " + GridLocation.Y;
    }

    public void Start()
    {
        if (!Walkable)
        {
            Markable = false;
            MazeLevelManager.Instance.Level.AddUnwalkableTile(this);
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
            if (PlayerMark.sprite != null) return;

            if (!Markable) return;

            if (GameManager.Instance.GameType == GameType.Multiplayer && !player.PhotonView.IsMine) return;
            MazeLevelManager.Instance.SetTileMarker(this, player);        
        }
    }
}

