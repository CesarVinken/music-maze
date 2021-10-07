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

    void Awake()
    {
        Instance = this;

        _inDrawingMode = false;
        CheckForFerryRouteOnTile();
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

    private void ResetColouredTiles()
    {
        EditorTileSelector.Instance.ResetColouredTiles();
    }

    public void ColourAddableTiles()
    {
        if (SelectedFerryRoute == null) return;

        Logger.Log($"What is the direction of the ferry route? {SelectedFerryRoute.FerryRouteDirection}");

        //show green tiles around the addable water tiles around the last tile of the route
        List<FerryRoutePoint> ferryRoutePoints = SelectedFerryRoute.GetFerryRoutePoints();
        EditorMazeTile lastTile = ferryRoutePoints[ferryRoutePoints.Count - 1].Tile as EditorMazeTile;

        lastTile.SetTileOverlayImage(TileOverlayMode.Blue);
        EditorTileSelector.Instance.AddColouredTile(lastTile);

        foreach (KeyValuePair<Direction, Tile> neighbour in lastTile.Neighbours)
        {
            EditorMazeTile neighbourTile = neighbour.Value as EditorMazeTile;

            if (neighbourTile == null)
            {
                continue;
            }

            if (SelectedFerryRoute.FerryRouteDirection == FerryRouteDirection.Vertical &&
                ferryRoutePoints.Count > 1 &&
                (neighbour.Key == Direction.Right || neighbour.Key == Direction.Left))
            {
                continue;
            }

            if (SelectedFerryRoute.FerryRouteDirection == FerryRouteDirection.Horizontal &&
               ferryRoutePoints.Count > 1 &&
               (neighbour.Key == Direction.Up || neighbour.Key == Direction.Down))
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
                if (ferryRoutePoints[j].Tile.TileId.Equals(neighbourTile.TileId))
                {
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
            EditorTileSelector.Instance.AddColouredTile(neighbourTile);
        }
    }

    public void CloseFerryRouteDrawingMode()
    {
        Logger.Log("Close Ferry Route Drawing Mode");
        _inDrawingMode = false;
        _buttonText.text = "Edit Ferry Route";
        SelectedFerryRoute = null;

        EditorTileSelector.Instance.ResetColouredTiles();
        CheckForFerryRouteOnTile();
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
