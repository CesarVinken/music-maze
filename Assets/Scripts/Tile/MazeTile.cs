using System.Linq;
using UnityEngine;

public class MazeTile : Tile, IMazeLevel
{
    public SpriteRenderer PlayerMarkRenderer;
    public SpriteRenderer PlayerMarkEndsRenderer;

    public bool Markable = false;
    public TransformationState TransformationState = TransformationState.Bleak;

    public PlayerMark PlayerMark = null;

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

        if (MazeLevelGameplayManager.Instance.NumberOfUnmarkedTiles == -1)
        {
            MazeLevelGameplayManager.Instance.NumberOfUnmarkedTiles = 1;
        }
        else
        {
            MazeLevelGameplayManager.Instance.NumberOfUnmarkedTiles++;
        }
    }


    public void TryMakeMarkable(bool isMarkable)
    {
        MazeTilePath mazeTilePath = (MazeTilePath)_tileBackgrounds.FirstOrDefault(background => background is MazeTilePath);

        if (mazeTilePath == null)
        {
            Markable = false;
            return;
        }

        for (int i = 0; i < _tileAttributes.Count; i++)
        {
            if (_tileAttributes[i] is PlayerExit)
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