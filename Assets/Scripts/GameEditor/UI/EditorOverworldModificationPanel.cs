using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EditorOverworldModificationPanel : EditorGridModificationPanel
{
    [SerializeField] private InputField _overworldNameInputField;

    private string _overworldName = "";

    public new void Awake()
    {
        base.Awake();

        Guard.CheckIsNull(_overworldNameInputField, "_overworldNameInputField", gameObject);
    }

    public void SaveOverworld()
    {
        Logger.Log("Save overworld");
        if (string.IsNullOrWhiteSpace(_overworldName))
        {
            Logger.Warning(Logger.Datawriting, "In order to save the overworld, please fill in an overworld name");
            return;
        }

        if (OverworldManager.Instance.EditorOverworld == null)
        {
            Logger.Warning(Logger.Datawriting, "Please first generate an overworld before saving.");
            return;
        }

        if (_overworldName == "overworlds")
        {
            Logger.Warning(Logger.Datawriting, "A maze level cannot have the name 'levels', as this is already the name of the file that lists all the maze levels");
            return;
        }

        SaveOverworldData();
        AddOverworldToOverworldList();

        Logger.Log(Logger.Datawriting, "Overworld {0} Saved.", _overworldName);
    }

    private void SaveOverworldData()
    {
        OverworldData overworldData = new OverworldData(OverworldManager.Instance.EditorOverworld).WithName(_overworldName);
        JsonOverworldFileWriter fileWriter = new JsonOverworldFileWriter();
        fileWriter.SerialiseData(overworldData);
    }

    private void AddOverworldToOverworldList()
    {
        OverworldNamesData overworldNameData = new OverworldNamesData(_overworldName).AddOverworldName(_overworldName);

        JsonOverworldListFileWriter fileWriter = new JsonOverworldListFileWriter();
        fileWriter.SerialiseData(overworldNameData);
    }

    public void LoadOverworld()
    {
        Logger.Log("Load overworld");
        if (string.IsNullOrWhiteSpace(_overworldName))
        {
            Logger.Warning(Logger.Datawriting, "In order to save the overworld, please fill in an overworld name");
            return;
        }

        bool overworldNameExists = OverworldLoader.OverworldExists(_overworldName);

        if (overworldNameExists)
        {
            OverworldData pverworldData = OverworldLoader.LoadOverworldData(_overworldName);
            OverworldLoader.LoadOverworldForEditor(pverworldData);
        }
    }

    public void SetOverworldName(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            _overworldNameInputField.text = "";
            return;
        }

        _overworldName = input;
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
        OverworldManager.Instance.UnloadOverworld();

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

                //SerialisableTileAttribute edgeObstacle = TryAddEdgeObstacle(gridLocation);

                //if (edgeObstacle != null)
                //{
                //    tileAttributes.Add(edgeObstacle);
                //}

                //SerialisableTilePathBackground overworldTilePath = TryAddPathsForNewOverworld(gridLocation, tileAttributes);

                //if (overworldTilePath != null)
                //{
                //    tileBackgrounds.Add(overworldTilePath);
                //}

                SerialisableTileBaseBackground baseBackground = TryAddBaseBackgroundForNewOverworld(tileBackgrounds, tileAttributes);

                if (baseBackground != null)
                {
                    tileBackgrounds.Add(baseBackground);
                }

                SerialisableTile tile = new SerialisableTile(tileId, tileAttributes, tileBackgrounds, gridLocation.X, gridLocation.Y);
                tiles.Add(tile);
            }
        }

        OverworldData newOverworldData = new OverworldData();
        newOverworldData.Tiles = tiles;

        OverworldLoader.LoadOverworldForEditor(newOverworldData);
    }

    private SerialisableTileBaseBackground TryAddBaseBackgroundForNewOverworld(List<SerialisableTileBackground> tileBackgrounds, List<SerialisableTileAttribute> tileAttributes)
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

}
