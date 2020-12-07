using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance;

    public Sprite[] DefaultDoor;
    public Sprite[] DefaultPath;
    public Sprite[] DefaultWall;
    public Sprite[] DefaultMazeTileBackground;
    public Sprite[] Bush;

    [Space(10)]
    [Header("Player graphics")]
    public Sprite[] Player1TileMarker;
    public Sprite[] Player2TileMarker;
    public Sprite[] PlayerTileMarkerEdge;

    public const int BaseBackgroundSortingOrder = -20;
    public const int PathSortingOrder = -10;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(Player1TileMarker, "Player1TileMarker", gameObject);
        Guard.CheckIsNull(Player2TileMarker, "Player2TileMarker", gameObject);
        Guard.CheckIsNull(PlayerTileMarkerEdge, "PlayerTileMarkerEdge", gameObject);
        Guard.CheckIsNull(Bush, "Bush", gameObject);
    }
}
