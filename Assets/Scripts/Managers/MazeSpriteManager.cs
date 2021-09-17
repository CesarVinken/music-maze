using UnityEngine;

public class MazeSpriteManager : SpriteManager
{
    public static MazeSpriteManager Instance;

    public Sprite[] DefaultDoor;
    public Sprite[] DefaultDoorColourful;
    public Sprite[] DefaultPath;
    public Sprite[] DefaultWall;
    public Sprite[] DefaultWallColourful;
    public Sprite[] DefaultMazeTileGround;
    public Sprite[] DefaultMazeTileGroundColourful;
    public Sprite[] Bush;
    public Sprite[] BushColourful;
    public Sprite[] DefaultMazeTileWater;
    public Sprite[] DefaultMazeTileWaterColourful;
    public Sprite[] WoodenBridge;
    public Sprite[] WoodenBridgeColourful;
    public Sprite[] FerryRouteSprites;

    [Space(10)]
    [Header("Player graphics")]
    public Sprite[] Player1TileMarker;
    public Sprite[] Player2TileMarker;
    public Sprite[] PlayerTileMarkerEdge;

    public void Awake()
    {
        Instance = this;

        Guard.CheckLength(DefaultDoor, "DefaultDoor");
        Guard.CheckLength(DefaultDoorColourful, "DefaultDoorColourful");
        Guard.CheckLength(DefaultPath, "DefaultPath");
        Guard.CheckLength(DefaultWall, "DefaultWall");
        Guard.CheckLength(DefaultWallColourful, "DefaultWallColourful");
        Guard.CheckLength(DefaultMazeTileGround, "DefaultMazeTileGround");
        Guard.CheckLength(DefaultMazeTileGroundColourful, "DefaultMazeTileGroundColourful");
        Guard.CheckLength(Bush, "Bush");
        Guard.CheckLength(BushColourful, "BushColourful");
        Guard.CheckLength(DefaultMazeTileWater, "DefaultMazeTileWater");
        Guard.CheckLength(DefaultMazeTileWaterColourful, "DefaultMazeTileWaterColourful");
        Guard.CheckLength(WoodenBridge, "WoodenBridge");
        Guard.CheckLength(WoodenBridgeColourful, "WoodenBridgeColourful");
        Guard.CheckLength(FerryRouteSprites, "FerryRouteSprites");

        Guard.CheckLength(Player1TileMarker, "Player1TileMarker");
        Guard.CheckLength(Player2TileMarker, "Player2TileMarker");
        Guard.CheckLength(PlayerTileMarkerEdge, "PlayerTileMarkerEdge");

        GameManager.Instance.SpriteManager = this;
    }
}
