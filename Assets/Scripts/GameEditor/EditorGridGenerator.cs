using UnityEngine;
using UnityEngine.UI;

public class EditorGridGenerator : MonoBehaviour
{
    public InputField WidthInputField;
    public InputField HeightInputField;

    private int _gridWidth = 8;
    private int _gridHeight = 8;

    [SerializeField] private GameObject EmptyTilePrefab;
    [SerializeField] private GameObject TileBlockerPrefab;

    public void Awake()
    {
        //if (WidthInputField == null)
        //    Logger.Error(Logger.Initialisation, "Could not find WidthInputField component on EditorGridGenerator");
        //if (HeightInputField == null)
        //    Logger.Error(Logger.Initialisation, "Could not find HeightInputField component on EditorGridGenerator");

        Guard.CheckIsNull(HeightInputField, "HeightInputField", gameObject);
        Guard.CheckIsNull(WidthInputField, "WidthInputField", gameObject);

        Guard.CheckIsNull(TileBlockerPrefab, "TileBlockerPrefab", gameObject);
        Guard.CheckIsNull(EmptyTilePrefab, "EmptyTilePrefab", gameObject);
    }

    public void SetWidth(string input)
    {
        if (!ValidateInput(input)) {
            WidthInputField.text = _gridWidth.ToString();
            return;
        }

        _gridWidth = int.Parse(input);
    }

    public void SetHeight(string input)
    {
        if (!ValidateInput(input))
        {
            HeightInputField.text = _gridHeight.ToString();
            return;
        }

        _gridHeight = int.Parse(input);
    }


    public bool ValidateInput(string input)
    {
        if (int.TryParse(input, out int result)) return true;

        Logger.Warning("Could not parse the input {0}. Make sure to only give numeric values.", input);
        return false;
    }

    public void GenerateTiles()
    {
        if (_gridWidth < 3)
        {
            Logger.Warning(Logger.Initialisation, "Cannot generate a tile grid with a width of {0}. The minimum generatable grid is 3", _gridWidth);
            return;
        }

        if (_gridHeight < 3)
        {
            Logger.Warning(Logger.Initialisation, "Cannot generate a tile grid with a height of {0}. The minimum generatable grid is 3", _gridHeight);
            return;
        }

        Logger.Log("Generate tile grid with a width of {0} and a height of {1}", _gridWidth, _gridHeight);

        // remove everything from the currently loaded level
        MazeLevelManager.Instance.UnloadLevel();
        GameObject EditorLevelContainer = new GameObject("EditorLevelContainer");
        EditorLevelContainer.transform.SetParent(GameManager.Instance.GridGO.transform);

        GameObject tile = Instantiate(EmptyTilePrefab, EditorLevelContainer.transform);
    }
}
