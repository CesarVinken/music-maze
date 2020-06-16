using System.Collections.Generic;
using UnityEngine;

public class MazeLevelManager : MonoBehaviour
{
    public static MazeLevelManager Instance;
    public MazeLevel Level;

    public void Awake()
    {
        Instance = this;

        Level = LoadLevel();
    }

    public MazeLevel LoadLevel()
    {
        return MazeLevel.Create();
    }
}
