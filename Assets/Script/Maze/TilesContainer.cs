using System.Collections.Generic;
using UnityEngine;

public class TilesContainer : MonoBehaviour
{
    public static TilesContainer Instance;

    public List<Tile> Tiles;
    public List<GameObject> TileGOs;

    public void Awake()
    {
        Instance = this;    
    }

    // TODO: Make editor button to automatically capture all tile children. That should update the lists on this container.

    // When loading a level this container should be updated with the correct tiles that belong to the loaded level.
}
