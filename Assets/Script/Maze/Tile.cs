using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public Sprite Sprite;

    public void Awake()
    {
        if (SpriteRenderer == null)
            Logger.Error(Logger.Initialisation, "Could not find sprite renderer on tile prefab");

        Sprite = SpriteRenderer.sprite;
    }
}
