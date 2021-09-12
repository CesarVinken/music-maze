using System.Collections.Generic;
using UnityEngine;

public class EditorMazeTile : MazeTile
{
    [Header("Editor")]

    [SerializeField] private SpriteRenderer _overlaySpriteRenderer;
    private TileOverlayMode _tileOverlayMode = TileOverlayMode.Empty;
    public TileOverlayMode TileOverlayMode { get => _tileOverlayMode; private set => _tileOverlayMode = value; }

    public List<EditorMazeTile> BeautificationTriggerers = new List<EditorMazeTile>(); // used in the editor for non-markable tiles and lists their triggerer.


    public void RemoveTileAsBeautificationTrigger()
    {
        for (int i = 0; i < MazeLevelGameplayManager.Instance.EditorLevel.Tiles.Count; i++)
        {
            EditorMazeTile tile = MazeLevelGameplayManager.Instance.EditorLevel.Tiles[i] as EditorMazeTile;
            if (tile.BeautificationTriggerers.Contains(this))
            {
                tile.BeautificationTriggerers.Remove(this);
            }
        }
    }

    public void RemoveBeautificationTriggerers()
    {
        BeautificationTriggerers.Clear();
    }

    public void SetTileOverlayImage(TileOverlayMode tileOverlayMode)
    {
        switch (tileOverlayMode)
        {
            case TileOverlayMode.Empty:
                _overlaySpriteRenderer.color = new Color(0, 0, 0, 0);
                break;
            case TileOverlayMode.Blue:
                _overlaySpriteRenderer.color = new Color(0, 0, 1, 0.5f);
                break;
            case TileOverlayMode.Green:
                _overlaySpriteRenderer.color = new Color(0, 1, 0, 0.5f);
                break;
            case TileOverlayMode.Yellow:
                _overlaySpriteRenderer.color = new Color(1, 1, 0, 0.5f);
                break;
            default:
                Logger.Error($"Tile overlay mode {tileOverlayMode} was not yet implemented");
                break;
        }

        _tileOverlayMode = tileOverlayMode;
    }
}
