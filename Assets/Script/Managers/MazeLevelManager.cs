using System.Collections.Generic;
using UnityEngine;

public class MazeLevelManager : MonoBehaviour
{
    public static MazeLevelManager Instance;
    public MazeLevel Level;

    public void Awake()
    {
        Instance = this;

        LoadLevel();
    }

    public void LoadLevel(MazeName mazeName = MazeName.Blank6x6)
    {
        Level = MazeLevel.Create(mazeName);
    }

    public void UnloadLevel()
    {
        Destroy(TilesContainer.Instance.gameObject);
        TilesContainer.Instance = null;

        Level.Tiles.Clear();
    }
}
