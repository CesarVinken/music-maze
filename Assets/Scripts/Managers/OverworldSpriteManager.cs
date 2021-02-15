
using UnityEngine;

public class OverworldSpriteManager : SpriteManager
{
    public static OverworldSpriteManager Instance;

    public Sprite[] DefaultMazeTileBackground;

    public void Awake()
    {
        Instance = this;

        Guard.CheckLength(DefaultMazeTileBackground, "DefaultMazeTileBackground");
    }
}
