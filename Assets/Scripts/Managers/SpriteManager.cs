using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance;

    public Sprite[] DefaultDoor;
    public Sprite[] DefaultPath;
    public Sprite[] DefaultWall;
    public Sprite[] DefaultMazeTileBackground;

    public void Awake()
    {
        Instance = this;
    }
}
