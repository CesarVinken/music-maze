using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FerryRouteDrawingModeAccessor : MonoBehaviour
{
    public static FerryRouteDrawingModeAccessor Instance;

    private static bool _inDrawingMode = false;

    [SerializeField] private Button _ferryRouteDrawingButton;
    [SerializeField] private Text _buttonText;

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
    }

    public void CloseFerryRouteDrawingMode()
    {
        Logger.Log("Close Ferry Route Drawing Mode");
        _inDrawingMode = false;
        _buttonText.text = "Edit Ferry Route";
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
            _ferryRouteDrawingButton.gameObject.SetActive(true);
        }
    }
}
