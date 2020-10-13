using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public Sprite Sprite;
    public SpriteRenderer PlayerMark;

    public bool Walkable = true; // TODO Automatically set value in Maze Level Editor
    public GridLocation GridLocation;

    public void Awake()
    {
        if (SpriteRenderer == null)
            Logger.Error(Logger.Initialisation, "Could not find sprite renderer on tile prefab");

        Sprite = SpriteRenderer.sprite;

        GridLocation = GridLocation.VectorToGrid(transform.position);
        gameObject.name = "Tile" + GridLocation.X + ", " + GridLocation.Y;
    }

    public void Start()
    {
        if (!Walkable)
        {
            MazeLevelManager.Instance.Level.AddUnwalkableTile(this);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        if (player != null)
        {
            Logger.Log("{0} entered tile {1},{2}", player.name, GridLocation.X, GridLocation.Y);
            if (PlayerMark.sprite != null) return;

            MazeLevelManager.Instance.SetTileMarker(this, player);        }
    }
}

