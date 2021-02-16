
using UnityEngine;

public class OverworldSpriteManager : SpriteManager
{
    public static OverworldSpriteManager Instance;

    public Sprite[] DefaultOverworldTileBackground;
    public Sprite[] Path;

    public void Awake()
    {
        Instance = this;

        Guard.CheckLength(DefaultOverworldTileBackground, "DefaultOverworldTileBackground");
        Guard.CheckLength(Path, "Path");
    }
}
