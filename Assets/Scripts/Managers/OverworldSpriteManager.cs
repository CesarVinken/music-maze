
using UnityEngine;

public class OverworldSpriteManager : SpriteManager
{
    public static OverworldSpriteManager Instance;

    public Sprite[] DefaultOverworldTileBackground;
    public Sprite[] Path;
    public Sprite[] DefaultOverworldTileWater;

    public void Awake()
    {
        Instance = this;

        Guard.CheckLength(DefaultOverworldTileBackground, "DefaultOverworldTileBackground");
        Guard.CheckLength(Path, "Path");
        Guard.CheckLength(DefaultOverworldTileWater, "DefaultOverworldTileWater");

        GameManager.Instance.SpriteManager = this;
    }
}
