using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FerryRouteDrawingModeAccessor : MonoBehaviour
{
    public static FerryRouteDrawingModeAccessor Instance;

    private static bool _inDrawingMode = false;
    public static bool InDrawingMode { get => _inDrawingMode; private set => _inDrawingMode = value; }

    [SerializeField] private Button _ferryRouteDrawingButton;
    [SerializeField] private Text _buttonText;

    private FerryRoute _ferryRouteOnTile;
    public static FerryRoute SelectedFerryRoute;

    private List<EditorMazeTile> _colouredTiles = new List<EditorMazeTile>();


    void Awake()
    {
        Instance = this;

        _inDrawingMode = false;
    }

    public static void ToggleFerryRouteDrawingMode()
    {
        if (!_inDrawingMode)
        {
            Instance.AccessFerryRouteDrawingMode();
        }
        else
        {
            Instance.CloseFerryRouteDrawingMode();
        }
    }

    public void AccessFerryRouteDrawingMode()
    {
        Logger.Log("Access Ferry Route Drawing Mode");
        _inDrawingMode = true;
        _buttonText.text = "Close Ferry Route Drawing Mode";
        SelectedFerryRoute = _ferryRouteOnTile;

        ResetColouredTiles();
        ColourAddableTiles();
    }

    public void ResetColouredTiles()
    {
        // Make sure old coloured tiles are reset
        for (int i = 0; i < _colouredTiles.Count; i++)
        {
            _colouredTiles[i].SetTileOverlayImage(TileOverlayMode.Empty);
        }
    }

    public void ColourAddableTiles()
    {
        if (SelectedFerryRoute == null) return;

        //show green tiles around the addable water tiles around the last tile of the route
        List<FerryRoutePoint> ferryRoutePoints = SelectedFerryRoute.GetFerryRoutePoints();
        EditorMazeTile lastTile = ferryRoutePoints[ferryRoutePoints.Count - 1].Tile as EditorMazeTile;

        lastTile.SetTileOverlayImage(TileOverlayMode.Blue);
        _colouredTiles.Add(lastTile);

        foreach (KeyValuePair<Direction, Tile> neighbour in lastTile.Neighbours)
        {
            EditorMazeTile neighbourTile = neighbour.Value as EditorMazeTile;

            if (neighbourTile == null)
            {
                continue;
            }

            if (neighbourTile.TileMainMaterial.GetType() != typeof(WaterMainMaterial))
            {
                continue;
            }

            bool tileIsAlreadyInList = false;
            for (int j = 0; j < ferryRoutePoints.Count; j++)
            {
                Logger.Log($"{ferryRoutePoints[j].Tile.TileId} and {neighbourTile.TileId}. Are they equal? {ferryRoutePoints[j].Tile.TileId.Equals(neighbourTile.TileId)}");
                if (ferryRoutePoints[j].Tile.TileId.Equals(neighbourTile.TileId))
                {
                    Logger.Log(neighbourTile.TileOverlayMode);
                    tileIsAlreadyInList = true;
                    break;
                }
            }

            if (tileIsAlreadyInList)
            {
                continue;
            }

            if (neighbourTile.TryGetAttribute<BridgePiece>() || neighbourTile.TryGetAttribute<FerryRoute>())
            {
                continue;
            }

            neighbourTile.SetTileOverlayImage(TileOverlayMode.Green);
            _colouredTiles.Add(neighbourTile);
        }
    }

    public void CloseFerryRouteDrawingMode()
    {
        Logger.Log("Close Ferry Route Drawing Mode");
        _inDrawingMode = false;
        _buttonText.text = "Edit Ferry Route";
        SelectedFerryRoute = null;
        
        for (int i = 0; i < _colouredTiles.Count; i++)
        {
            _colouredTiles[i].SetTileOverlayImage(TileOverlayMode.Empty);
        }
    }

    public void CheckForFerryRouteOnTile()
    {
        MazeTile selectedTile = EditorTileSelector.Instance.CurrentlySelectedTile as MazeTile;
        FerryRoute ferryRoute = selectedTile?.TryGetAttribute<FerryRoute>();

        if (ferryRoute == null && !_inDrawingMode)
        {
            _ferryRouteDrawingButton.gameObject.SetActive(false);
        }
        else
        {
            _ferryRouteOnTile = ferryRoute;
            _ferryRouteDrawingButton.gameObject.SetActive(true);
        }
    }
}
