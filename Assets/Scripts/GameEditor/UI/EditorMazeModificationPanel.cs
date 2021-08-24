using DataSerialisation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EditorMazeModificationPanel : EditorGridModificationPanel
{
    public static EditorMazeModificationPanel Instance;

    [SerializeField] private InputField _mazeNameInputField;

    private string _mazeName = "";

    public new void Awake()
    {
        Instance = this;

        base.Awake();

        Guard.CheckIsNull(_mazeNameInputField, "MazeNameInputField", gameObject);
    }

    public void SetMazeName(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            _mazeNameInputField.text = "";
            return;
        }

        _mazeName = input;
    }

    public void GenerateTiles()
    {
        if (_gridWidth < 3)
        {
            Logger.Warning(Logger.Level, "Cannot generate a tile grid with a width of {0}. The minimum generatable grid width is 3", _gridWidth);
            return;
        }

        if (_gridWidth > 28)
        {
            Logger.Warning(Logger.Level, "Cannot generate a tile grid with a width of {0}. The maximum generatable grid width is 28", _gridWidth);
            return;
        }

        if (_gridHeight < 3)
        {
            Logger.Warning(Logger.Level, "Cannot generate a tile grid with a height of {0}. The minimum generatable grid height is 3", _gridHeight);
            return;
        }

        if (_gridHeight > 28)
        {
            Logger.Warning(Logger.Level, "Cannot generate a tile grid with a height of {0}. The maximum generatable grid height is 28", _gridHeight);
            return;
        }

        Logger.Log("Generate tile grid with a width of {0} and a height of {1}", _gridWidth, _gridHeight);

        // remove everything from the currently loaded level
        MazeLevelGameplayManager.Instance.UnloadLevel();

        // Create a new level from scratch with a obstacle ring at the edges
        List<SerialisableTile> tiles = new List<SerialisableTile>();

        for (int i = 0; i < _gridWidth; i++)
        {
            for (int j = 0; j < _gridHeight; j++)
            {
                string tileId = Guid.NewGuid().ToString();

                GridLocation gridLocation = new GridLocation(i, j);
                List<SerialisableTileAttribute> tileAttributes = new List<SerialisableTileAttribute>();
                List<SerialisableTileBackground> tileBackgrounds = new List<SerialisableTileBackground>();
                List<SerialisableTileCornerFiller> tileCornerFillers = new List<SerialisableTileCornerFiller>();

                SerialisableTileMainMaterial mainMaterial = new SerialisableTileMainMaterial("GroundMainMaterial", new SerialisableLandMaterial());

                SerialisableTileObstacleAttribute edgeObstacle = TryAddEdgeObstacle(gridLocation);
                
                if(edgeObstacle != null)
                {
                    string sanatisedAttributeType = edgeObstacle.GetType().ToString().Split('.')[1];
                    tileAttributes.Add(new SerialisableTileAttribute(sanatisedAttributeType, edgeObstacle));
                }

                SerialisableTilePathBackground mazeTilePath = TryAddPathsForNewMaze(gridLocation, tileAttributes);

                if (mazeTilePath != null)
                {
                    string sanatisedPathBackgroundType = mazeTilePath.GetType().ToString().Split('.')[1];
                    tileBackgrounds.Add(new SerialisableTileBackground(sanatisedPathBackgroundType, mazeTilePath));
                }

                SerialisableTileBaseGround baseBackground = TryAddBaseBackgroundForNewMaze(tileBackgrounds, tileAttributes);

                if (baseBackground != null)
                {
                    string sanatisedBackgroundType = baseBackground.GetType().ToString().Split('.')[1];
                    tileBackgrounds.Add(new SerialisableTileBackground(sanatisedBackgroundType, baseBackground));
                }

                SerialisableTile tile = new SerialisableTile(tileId, mainMaterial, tileAttributes, tileBackgrounds, tileCornerFillers, gridLocation.X, gridLocation.Y);
                tiles.Add(tile);
            }
        }

        MazeLevelData newMazeLevelData = new MazeLevelData();
        newMazeLevelData.Tiles = tiles;

        MazeLevelLoader.LoadMazeLevelForEditor(newMazeLevelData);
    }

    public void SaveMaze()
    {
        if (string.IsNullOrWhiteSpace(_mazeName))
        {
            Logger.Warning(Logger.Datawriting, "In order to save the maze level, please fill in a maze name");
            return;
        }

        if (MazeLevelGameplayManager.Instance.EditorLevel == null)
        {
            Logger.Warning(Logger.Datawriting, "Please first generate a level before saving.");
            return;
        }

        if (_mazeName == "levels")
        {
            Logger.Warning(Logger.Datawriting, "A maze level cannot have the name 'levels', as this is already the name of the file that lists all the maze levels");
            return;
        }

        if (MazeLevelNamesData.LevelNameExists(_mazeName))
        {
            // show warning panel for player. "Are you sure you want to save?"
            GameObject twoOptionsPanelGO = GameObject.Instantiate(EditorCanvasUI.Instance.EditorTwoOptionsPanelPrefab, EditorCanvasUI.Instance.transform);
            EditorTwoOptionPanel twoOptionsPanel = twoOptionsPanelGO.GetComponent<EditorTwoOptionPanel>();
            twoOptionsPanel.Initialize(
                $"Are you sure you want to overwrite the maze level {_mazeName}?", 
                EditorUIAction.Close, 
                "Cancel", 
                EditorUIAction.SaveMaze, 
                "Save"
            );
        }
        else
        {
            MazeLevelSaver mazeLevelSaver = new MazeLevelSaver();
            mazeLevelSaver.Save(_mazeName);
        }
    }

    public void LoadMaze()
    {
        Logger.Log("Load maze (in editor)");
        if (string.IsNullOrWhiteSpace(_mazeName))
        {
            Logger.Warning(Logger.Datawriting, "In order to save the maze level, please fill in a maze name");
            return;
        }

        bool mazeLevelNameExists = MazeLevelLoader.MazeLevelExists(_mazeName);

        if (mazeLevelNameExists)
        {
            MazeLevelGameplayManager.Instance.UnloadLevel();

            MazeLevelData mazeLevelData = MazeLevelLoader.LoadMazeLevelData(_mazeName);
            MazeLevelLoader.LoadMazeLevelForEditor(mazeLevelData); 
        }

        EditorSelectedMazeTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer as EditorSelectedMazeTileModifierContainer;
        selectedTileModifierContainer?.SetInitialModifierValues();

        EditorMazeTileModificationPanel.Instance?.Reset();
        EditorMazeTileModificationPanel.Instance?.DestroyModifierActions();
    }

    
    public void TogglePlayableMazesPanel()
    {
        if (EditorCanvasUI.Instance.PlayableLevelsPanelGO.activeSelf)
        {
            EditorCanvasUI.Instance.PlayableLevelsPanelGO.SetActive(false);
        }
        else
        {
            EditorCanvasUI.Instance.PlayableLevelsPanelGO.SetActive(true);
        }
    }

    private SerialisableTileObstacleAttribute TryAddEdgeObstacle(GridLocation gridLocation)
    {
        if (gridLocation.X == 0)
        {
            if (gridLocation.Y == 0) // Bottom left
            {
                return new SerialisableTileObstacleAttribute(
                    new TileConnectionScoreInfo(8));
            }
            else if (gridLocation.Y == _gridHeight - 1) // Top left
            {
                return new SerialisableTileObstacleAttribute(
                    new TileConnectionScoreInfo(6));
            }
            else // Colomn left
            {
                return new SerialisableTileObstacleAttribute(
                    new TileConnectionScoreInfo(10));
            }
        }
        else if (gridLocation.X == _gridWidth - 1)
        {
            if (gridLocation.Y == 0) // Bottom right
            {
                return new SerialisableTileObstacleAttribute(
                    new TileConnectionScoreInfo(11));
            }
            else if (gridLocation.Y == _gridHeight - 1) // Top right
            {
                return new SerialisableTileObstacleAttribute(
                    new TileConnectionScoreInfo(9));
            }
            else // Colomn right
            {
                return new SerialisableTileObstacleAttribute(
                    new TileConnectionScoreInfo(10));
            }
        }
        else if (gridLocation.Y == 0) // Bottom row
        {
            return new SerialisableTileObstacleAttribute(
                new TileConnectionScoreInfo(7));
        }
        else if (gridLocation.Y == _gridHeight - 1) // Top row
        {
            return new SerialisableTileObstacleAttribute(
                new TileConnectionScoreInfo(7));
        }

        return null;
    }

    private SerialisableTilePathBackground TryAddPathsForNewMaze(GridLocation gridLocation, List<SerialisableTileAttribute> tileAttributes)
    {
        for (int i = 0; i < tileAttributes.Count; i++)
        {
            //Type type = Type.GetType("DataSerialisation." + tileAttributes[i].AttributeType);
            Type type = SerialisableTileAttribute.GetType(tileAttributes[i].AttributeType);
            //Logger.Log(tileAttributes[i].AttributeType);
            if (type.Equals(typeof(SerialisableTileObstacleAttribute)))
            {
                return null;
            }
        }

        if (gridLocation.X == 1)
        {
            if (gridLocation.Y == 1) // Bottom left
            {
                return new SerialisableTilePathBackground(23);
            }
            else if (gridLocation.Y == _gridHeight - 2) // Top left
            {
                return new SerialisableTilePathBackground(21);
            }
            else // Colomn left
            {
                return new SerialisableTilePathBackground(32);
            }
        }
        else if (gridLocation.X == _gridWidth - 2)
        {
            if (gridLocation.Y == 1) // Bottom right
            {
                return new SerialisableTilePathBackground(26);
            }
            else if (gridLocation.Y == _gridHeight - 2) // Top right
            {
                return new SerialisableTilePathBackground(25);
            }
            else // Colomn right
            {
                return new SerialisableTilePathBackground(34);
            }
        }
        else if (gridLocation.Y == 1) // Bottom row
        {
            return new SerialisableTilePathBackground(33);
        }
        else if (gridLocation.Y == _gridHeight - 2) // Top row
        {
            return new SerialisableTilePathBackground(31);
        }

        return new SerialisableTilePathBackground(16);
    }

    // return a base background, except for tiles that are completely covered by an obstacle or path (with connections to all sides)
    private SerialisableTileBaseGround TryAddBaseBackgroundForNewMaze(List<SerialisableTileBackground> tileBackgrounds, List<SerialisableTileAttribute> tileAttributes)
    {
        for (int i = 0; i < tileBackgrounds.Count; i++)
        {
            Type type = SerialisableTileBackground.GetType(tileBackgrounds[i].BackgroundType);
            if (type.Equals(typeof(SerialisableTilePathBackground)))
            {
                SerialisableTilePathBackground serialisableTilePathBackground = (SerialisableTilePathBackground)JsonUtility.FromJson(tileBackgrounds[i].SerialisedData, type);
                if(serialisableTilePathBackground.TileConnectionScore == 16)
                {
                    return null;
                }
            }
        }

        SerialisableTileObstacleAttribute obstacleAttribute = tileAttributes.OfType<SerialisableTileObstacleAttribute>().FirstOrDefault();

        if (obstacleAttribute == null)
        {
            return new SerialisableTileBaseGround(16);
        }

        if (obstacleAttribute.ConnectionScore == 16)
        {
            return null;
        }

        return new SerialisableTileBaseGround(16);
    }

    public string GetMazeLevelName()
    {
        return _mazeName;
    }
}
