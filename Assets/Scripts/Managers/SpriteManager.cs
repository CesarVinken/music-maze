using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance;

    public Sprite[] DefaultDoor;
    public Sprite[] DefaultDoorColourful;
    public Sprite[] DefaultPath;
    public Sprite[] DefaultWall;
    public Sprite[] DefaultWallColourful;
    public Sprite[] DefaultMazeTileBackground;
    public Sprite[] DefaultMazeTileBackgroundColourful;
    public Sprite[] Bush;
    public Sprite[] BushColourful;

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

        Guard.CheckLength(DefaultDoor, "DefaultDoor");
        Guard.CheckLength(DefaultDoorColourful, "DefaultDoorColourful");
        Guard.CheckLength(DefaultPath, "DefaultPath");
        Guard.CheckLength(DefaultWall, "DefaultWall");
        Guard.CheckLength(DefaultWallColourful, "DefaultWallColourful");
        Guard.CheckLength(DefaultMazeTileBackground, "DefaultMazeTileBackground");
        Guard.CheckLength(DefaultMazeTileBackgroundColourful, "DefaultMazeTileBackgroundColourful");
        Guard.CheckLength(Bush, "Bush");
        Guard.CheckLength(BushColourful, "BushColourful");

        Guard.CheckLength(Player1TileMarker, "Player1TileMarker");
        Guard.CheckLength(Player2TileMarker, "Player2TileMarker");
        Guard.CheckLength(PlayerTileMarkerEdge, "PlayerTileMarkerEdge");
    }
}
