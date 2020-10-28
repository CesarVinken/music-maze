using System;
using UnityEngine;
using UnityEngine.UI;

public class EditorGridGenerator : MonoBehaviour
{
    public InputField WidthInputField;
    public InputField HeightInputField;
    public InputField MazeNameInputField;

    private int _gridWidth = 8;
    private int _gridHeight = 8;

    private string _mazeName = "";

    [SerializeField] private GameObject EmptyTilePrefab;
    [SerializeField] private GameObject TileObstaclePrefab;

    public void Awake()
    {
        Guard.CheckIsNull(HeightInputField, "HeightInputField", gameObject);
        Guard.CheckIsNull(WidthInputField, "WidthInputField", gameObject);
        Guard.CheckIsNull(MazeNameInputField, "MazeNameInputField", gameObject);

        Guard.CheckIsNull(TileObstaclePrefab, "TileObstaclePrefab", gameObject);
        Guard.CheckIsNull(EmptyTilePrefab, "EmptyTilePrefab", gameObject);
    }

    public void SetWidth(string input)
    {
        if (!ValidateNumericInput(input)) {
            WidthInputField.text = _gridWidth.ToString();
            return;
        }

        _gridWidth = int.Parse(input);
    }

    public void SetHeight(string input)
    {
        if (!ValidateNumericInput(input))
        {
            HeightInputField.text = _gridHeight.ToString();
            return;
        }

        _gridHeight = int.Parse(input);
    }

    public void SetMazeName(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            MazeNameInputField.text = "";
            return;
        }

        _mazeName = input;
    }

    public bool ValidateNumericInput(string input)
    {
        if (int.TryParse(input, out int result)) return true;

        Logger.Warning("Could not parse the input {0}. Make sure to only give numeric values.", input);
        return false;
    }

    public void GenerateTiles()
    {
        if (_gridWidth < 3)
        {
            Logger.Warning(Logger.Level, "Cannot generate a tile grid with a width of {0}. The minimum generatable grid width is 3", _gridWidth);
            return;
        }

        if (_gridWidth > 25)
        {
            Logger.Warning(Logger.Level, "Cannot generate a tile grid with a width of {0}. The maximum generatable grid width is 20", _gridWidth);
            return;
        }

        if (_gridHeight < 3)
        {
            Logger.Warning(Logger.Level, "Cannot generate a tile grid with a height of {0}. The minimum generatable grid height is 3", _gridHeight);
            return;
        }

        if (_gridHeight > 25)
        {
            Logger.Warning(Logger.Level, "Cannot generate a tile grid with a height of {0}. The maximum generatable grid height is 20", _gridHeight);
            return;
        }

        Logger.Log("Generate tile grid with a width of {0} and a height of {1}", _gridWidth, _gridHeight);

        // remove everything from the currently loaded level
        MazeLevelManager.Instance.UnloadLevel();
        GameObject EditorLevelContainer = new GameObject("EditorLevelContainer");
        EditorLevelContainer.AddComponent<TilesContainer>();

        EditorLevelContainer.transform.SetParent(GameManager.Instance.GridGO.transform);

        MazeLevel newEditorLevel = new MazeLevel();
        newEditorLevel.LevelBounds = new GridLocation(_gridWidth - 1, _gridHeight - 1);

        // Create tiles
        for (int i = 0; i < _gridWidth; i++)
        {
            for (int j = 0; j < _gridHeight; j++)
            {
                GameObject tileGO = Instantiate(EmptyTilePrefab, new Vector3(0 + i, 0 + j, 0), Quaternion.identity, EditorLevelContainer.transform);
                Tile tile = tileGO.GetComponent<Tile>();
                tile.TileId = Guid.NewGuid().ToString();
                tile.SetGridLocation(0 + i, 0 + j);

                newEditorLevel.Tiles.Add(tile);
                newEditorLevel.TilesByLocation.Add(tile.GridLocation, tile);

                if (tile.GridLocation.X == 0 || tile.GridLocation.X == _gridWidth - 1 ||
                tile.GridLocation.Y == 0 || tile.GridLocation.Y == _gridHeight - 1)
                {
                    tile.BuildTileObstacle();
                }
            }
        }

        EditorManager.EditorLevel = newEditorLevel;

        //Update UI for the newly generated level
        EditorWorldContainer.Instance.ShowTileSelector();
        CameraController.Instance.SetPanLimits(EditorManager.EditorLevel.LevelBounds);
        CameraController.Instance.ResetCamera();
    }

    public void SaveMaze()
    {
        if (string.IsNullOrWhiteSpace(_mazeName))
        {
            Logger.Warning(Logger.Datawriting, "In order to save the maze level, please fill in a maze name");
            return;
        }

        if (EditorManager.EditorLevel == null)
        {
            Logger.Warning(Logger.Datawriting, "Please first generate a level before saving.");
            return;
        }

        MazeLevelData mazeLevelData = new MazeLevelData(EditorManager.EditorLevel).WithName(_mazeName);

        JsonMazeLevelFileWriter fileWriter = new JsonMazeLevelFileWriter();
        fileWriter.SerialiseData(mazeLevelData);

        Logger.Log(Logger.Datawriting, "Level {0} Saved.", _mazeName);
    }
}
