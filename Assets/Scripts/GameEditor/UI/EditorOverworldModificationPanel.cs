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
            OverworldData overworldData = OverworldLoader.LoadOverworldData(_overworldName);
            OverworldLoader.LoadOverworldForEditor(overworldData);
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
                SerialisableTileMainMaterial mainMaterial = new SerialisableTileMainMaterial("GroundMainMaterial", new SerialisableLandMaterial());

                List<SerialisableTileAttribute> tileAttributes = new List<SerialisableTileAttribute>();
                List<SerialisableTileBackground> tileBackgrounds = new List<SerialisableTileBackground>();

                SerialisableTileBaseGround baseBackground = TryAddBaseBackgroundForNewOverworld(tileBackgrounds, tileAttributes);

                if (baseBackground != null)
                {
                    tileBackgrounds.Add(new SerialisableTileBackground(baseBackground.GetType().ToString(), baseBackground));
                }

                SerialisableTile tile = new SerialisableTile(tileId, mainMaterial, tileAttributes, tileBackgrounds, gridLocation.X, gridLocation.Y);
                tiles.Add(tile);
            }
        }

        OverworldData newOverworldData = new OverworldData();
        newOverworldData.Tiles = tiles;

        OverworldLoader.LoadOverworldForEditor(newOverworldData);
    }

    private SerialisableTileBaseGround TryAddBaseBackgroundForNewOverworld(List<SerialisableTileBackground> tileBackgrounds, List<SerialisableTileAttribute> tileAttributes)
    {
        for (int i = 0; i < tileBackgrounds.Count; i++)
        {
            Type type = Type.GetType(tileBackgrounds[i].BackgroundType);
            if (type.Equals(typeof(SerialisableTilePathBackground)))
            {
                SerialisableTilePathBackground serialisableTilePathBackground = (SerialisableTilePathBackground)JsonUtility.FromJson(tileBackgrounds[i].SerialisedData, type);
                if (serialisableTilePathBackground.TileConnectionScore == 16)
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

}
