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
            Logger.Warning(Logger.Initialisation, "Cannot generate a tile grid with a width of {0}. The minimum generatable grid width is 3", _gridWidth);
            return;
        }

        if (_gridWidth > 25)
        {
            Logger.Warning(Logger.Initialisation, "Cannot generate a tile grid with a width of {0}. The maximum generatable grid width is 20", _gridWidth);
            return;
        }

        if (_gridHeight < 3)
        {
            Logger.Warning(Logger.Initialisation, "Cannot generate a tile grid with a height of {0}. The minimum generatable grid height is 3", _gridHeight);
            return;
        }

        if (_gridHeight > 25)
        {
            Logger.Warning(Logger.Initialisation, "Cannot generate a tile grid with a height of {0}. The maximum generatable grid height is 20", _gridHeight);
            return;
        }



        Logger.Log("Generate tile grid with a width of {0} and a height of {1}", _gridWidth, _gridHeight);

        // remove everything from the currently loaded level
        MazeLevelManager.Instance.UnloadLevel();
        GameObject EditorLevelContainer = new GameObject("EditorLevelContainer");
        EditorLevelContainer.AddComponent<TilesContainer>();

        EditorLevelContainer.transform.SetParent(GameManager.Instance.GridGO.transform);

        MazeLevel newEditorLevel = new MazeLevel();

        // Create tiles
        for (int i = 0; i < _gridWidth; i++)
        {
            for (int j = 0; j < _gridHeight; j++)
            {
                GameObject tileGO = Instantiate(EmptyTilePrefab, new Vector3(0 + i, 0 + j, 0), Quaternion.identity, EditorLevelContainer.transform);
                Tile tile = tileGO.GetComponent<Tile>();
                newEditorLevel.Tiles.Add(tile);
                newEditorLevel.TilesByLocation.Add(tile.GridLocation, tile);
            }
        }

        for (int k = 0; k < newEditorLevel.Tiles.Count; k++)
        {
            Tile tile = newEditorLevel.Tiles[k];
            if (tile.GridLocation.X == 0 || tile.GridLocation.X == _gridWidth - 1 ||
                tile.GridLocation.Y == 0 || tile.GridLocation.Y == _gridHeight - 1)
            {
                GameObject tileObstacle = Instantiate(TileBlockerPrefab, tile.gameObject.transform);
            }
        }
    }
}
