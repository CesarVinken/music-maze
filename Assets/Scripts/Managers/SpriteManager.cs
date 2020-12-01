using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance;

    public Sprite[] DefaultDoor;
    public Sprite[] DefaultPath;
    public Sprite[] DefaultWall;
    public Sprite[] DefaultMazeTileBackground;

    public const int BaseBackgroundSortingOrder = -20;
    public const int PathSortingOrder = -10;

    public void Awake()
    {
        Instance = this;
    }
}
