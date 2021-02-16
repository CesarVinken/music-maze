using System.Linq;
using UnityEngine;

public class MazeTile : Tile
{
    public SpriteRenderer PlayerMarkRenderer;
    public SpriteRenderer PlayerMarkEndsRenderer;

    public bool Markable = false;
    public TransformationState TransformationState = TransformationState.Bleak;

    public PlayerMark PlayerMark = null;

    //public Dictionary<ObjectDirection, MazeTile> Neighbours = new Dictionary<ObjectDirection, MazeTile>();



    public new void Awake()
    {
        base.Awake();

        Guard.CheckIsNull(PlayerMarkRenderer, "PlayerMarkRenderer", gameObject);
        Guard.CheckIsNull(PlayerMarkEndsRenderer, "PlayerMarkEndsRenderer", gameObject);
    }

    public new void Start()
    {
        base.Start();

        if (!Markable) return;

        if (MazeLevelManager.Instance.NumberOfUnmarkedTiles == -1)
        {
            MazeLevelManager.Instance.NumberOfUnmarkedTiles = 1;
        }
        else
        {
            MazeLevelManager.Instance.NumberOfUnmarkedTiles++;
        }
    }

    //public override void InitialiseTileAttributes()
    //{
    //    for (int i = 0; i < TileAttributes.Count; i++)
    //    {
    //        TileAttributes[i].SetTile(this);
    //    }
    //}

    //public override void InitialiseTileBackgrounds()
    //{
    //    for (int i = 0; i < TileBackgrounds.Count; i++)
    //    {
    //        TileBackgrounds[i].SetTile(this);
    //    }
    //}

    public override TileObstacle TryGetTileObstacle()
    {
        for (int i = 0; i < TileAttributes.Count; i++)
        {
            Logger.Log($"found attribute for {GridLocation.X}, {GridLocation.Y} is {TileAttributes[i].GetType()}");
        }
        TileObstacle tileObstacle = (TileObstacle)TileAttributes.FirstOrDefault(attribute => attribute is TileObstacle);

        if (tileObstacle == null)
        {
            Logger.Log($"did not find a tile obstacle on {GridLocation.X},{GridLocation.Y}");
            return null;
        }
        Logger.Log($"found tileObstacle {tileObstacle.ObstacleType} on {GridLocation.X},{GridLocation.Y}");
        return tileObstacle;
    }

    public override TilePath TryGetTilePath()
    {
        MazeTilePath mazeTilePath = (MazeTilePath)TileBackgrounds.FirstOrDefault(background => background is MazeTilePath);

        if (mazeTilePath == null)
        {
            Logger.Log($"did NOT find a maze tile path on {GridLocation.X},{GridLocation.Y}");
            return null;
        }
        Logger.Log($"found maze tile path {mazeTilePath.TilePathType} on {GridLocation.X},{GridLocation.Y} with score {mazeTilePath.ConnectionScore}");
        return mazeTilePath;
    }

    public void TryMakeMarkable(bool isMarkable)
    {
        MazeTilePath mazeTilePath = (MazeTilePath)TileBackgrounds.FirstOrDefault(background => background is MazeTilePath);

        if (mazeTilePath == null)
        {
            Markable = false;
            return;
        }

        for (int i = 0; i < TileAttributes.Count; i++)
        {
            if (TileAttributes[i] is PlayerExit)
            {
                Markable = false;
                return;
            }
        }
        Markable = isMarkable;
    }

    public void ResetPlayerMarkEndsRenderer()
    {
        PlayerMarkEndsRenderer.sprite = null;
    }
}