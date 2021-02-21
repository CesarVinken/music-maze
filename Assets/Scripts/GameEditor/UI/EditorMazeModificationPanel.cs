using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EditorMazeModificationPanel : EditorGridModificationPanel
{
    private string _mazeName = "";

    [SerializeField] private InputField _mazeNameInputField;

    public new void Awake()
    {
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

                SerialisableTileAttribute edgeObstacle = TryAddEdgeObstacle(gridLocation);

                if(edgeObstacle != null)
                {
                    tileAttributes.Add(edgeObstacle);
                }

                SerialisableTilePathBackground mazeTilePath = TryAddPathsForNewMaze(gridLocation, tileAttributes);

                if (mazeTilePath != null)
                {
                    tileBackgrounds.Add(mazeTilePath);
                }

                SerialisableTileBaseBackground baseBackground = TryAddBaseBackgroundForNewMaze(tileBackgrounds, tileAttributes);

                if (baseBackground != null)
                {
                    tileBackgrounds.Add(baseBackground);
                }

                SerialisableTile tile = new SerialisableTile(tileId, tileAttributes, tileBackgrounds, gridLocation.X, gridLocation.Y);
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

        if (MazeLevelManager.Instance.EditorLevel == null)
        {
            Logger.Warning(Logger.Datawriting, "Please first generate a level before saving.");
            return;
        }

        if (_mazeName == "levels")
        {
            Logger.Warning(Logger.Datawriting, "A maze level cannot have the name 'levels', as this is already the name of the file that lists all the maze levels");
            return;
        }

        CheckForTilesWithoutTransformationTriggerers();

        SaveMazeLevelData();
        AddMazeToMazeList();

        Logger.Log(Logger.Datawriting, "Level {0} Saved.", _mazeName);
    }

    private void SaveMazeLevelData()
    {
        MazeLevelData mazeLevelData = new MazeLevelData(MazeLevelManager.Instance.EditorLevel).WithName(_mazeName);
        JsonMazeLevelFileWriter fileWriter = new JsonMazeLevelFileWriter();
        fileWriter.SerialiseData(mazeLevelData);
    }

    public void LoadMaze()
    {
        if (string.IsNullOrWhiteSpace(_mazeName))
        {
            Logger.Warning(Logger.Datawriting, "In order to save the maze level, please fill in a maze name");
            return;
        }

        bool mazeLevelNameExists = MazeLevelLoader.MazeLevelExists(_mazeName);

        if (mazeLevelNameExists)
        {
            MazeLevelManager.Instance.UnloadLevel();

            MazeLevelData mazeLevelData = MazeLevelLoader.LoadMazeLevelData(_mazeName);
            MazeLevelLoader.LoadMazeLevelForEditor(mazeLevelData);
        }
    }

    private void AddMazeToMazeList()
    {
        MazeLevelNamesData levelNamesData = new MazeLevelNamesData(_mazeName).AddLevelName(_mazeName);

        JsonMazeLevelListFileWriter fileWriter = new JsonMazeLevelListFileWriter();
        fileWriter.SerialiseData(levelNamesData);
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

    private SerialisableTileAttribute TryAddEdgeObstacle(GridLocation gridLocation)
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
        bool hasObstacleAttribute = tileAttributes.Any(attribute => attribute.TileAttributeId == SerialisableTileAttribute.ObstacleAttributeCode);

        if (hasObstacleAttribute)
        {
            return null;
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
    private SerialisableTileBaseBackground TryAddBaseBackgroundForNewMaze(List<SerialisableTileBackground> tileBackgrounds, List<SerialisableTileAttribute> tileAttributes)
    {
        SerialisableTileBackground pathBackgrounds = tileBackgrounds.FirstOrDefault(background => background.TileConnectionScore == 16);
        if (pathBackgrounds != null)
        {
            return null;
        }

        SerialisableTileAttribute obstacleAttribute = tileAttributes.FirstOrDefault(attribute => attribute.TileAttributeId == SerialisableTileAttribute.ObstacleAttributeCode);

        if (obstacleAttribute == null)
        {
            return new SerialisableTileBaseBackground(-1);
        }

        if (obstacleAttribute.ObstacleConnectionScore == 16)
        {
            return null;
        }

        return new SerialisableTileBaseBackground(-1);
    }

    private void CheckForTilesWithoutTransformationTriggerers()
    {
        for (int i = 0; i < MazeLevelManager.Instance.EditorLevel.Tiles.Count; i++)
        {
            EditorMazeTile tile = MazeLevelManager.Instance.EditorLevel.Tiles[i];

            if(!tile.Markable && tile.BeautificationTriggerers.Count == 0)
            {
                tile.SetTileOverlayImage(TileOverlayMode.Yellow);
                Logger.Warning($"No transformation triggerer was set up for the tile at {tile.GridLocation.X},{tile.GridLocation.Y}");
            }
        }
    }
}
