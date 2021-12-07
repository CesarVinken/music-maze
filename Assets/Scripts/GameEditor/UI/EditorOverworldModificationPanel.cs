using DataSerialisation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EditorOverworldModificationPanel : EditorGridModificationPanel
{
    public static EditorOverworldModificationPanel Instance;

    [SerializeField] private InputField _overworldNameInputField;

    private string _overworldName = "";

    public new void Awake()
    {
        Instance = this;

        base.Awake();

        Guard.CheckIsNull(_overworldNameInputField, "_overworldNameInputField", gameObject);
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

    public void SaveOverworld()
    {
        Logger.Log("Save overworld");
        if (string.IsNullOrWhiteSpace(_overworldName))
        {
            Logger.Warning(Logger.Datawriting, "In order to save the overworld, please fill in an overworld name");
            return;
        }

        if (OverworldGameplayManager.Instance.EditorOverworld == null)
        {
            Logger.Warning(Logger.Datawriting, "Please first generate an overworld before saving.");
            return;
        }

        if (_overworldName == "overworlds")
        {
            Logger.Warning(Logger.Datawriting, "An overworld cannot have the name 'overworlds', as this is already the name of the file that lists all the overworlds");
            return;
        }

        if (OverworldNamesData.OverworldNameExists(_overworldName))
        {
            // show warning panel for player. "Are you sure you want to save?"
            GameObject twoOptionsPanelGO = GameObject.Instantiate(EditorCanvasUI.Instance.EditorTwoOptionsPanelPrefab, EditorCanvasUI.Instance.transform);
            EditorTwoOptionPanel twoOptionsPanel = twoOptionsPanelGO.GetComponent<EditorTwoOptionPanel>();
            twoOptionsPanel.Initialize(
                $"Are you sure you want to overwrite the overworld {_overworldName}?",
                EditorUIAction.Close,
                "Cancel",
                EditorUIAction.SaveOverworld,
                "Save"
            );
        }
        else
        {
            OverworldSaver overworldSaver = new OverworldSaver();
            overworldSaver.Save(_overworldName);
        }
    }

    public void LoadOverworld(string overworldName = "")
    {
        if (string.IsNullOrWhiteSpace(overworldName))
        {
            overworldName = _overworldName;
        }

        Logger.Log("Load overworld (in editor)");
        if (string.IsNullOrWhiteSpace(_overworldName))
        {
            Logger.Warning(Logger.Datawriting, "In order to save the overworld, please fill in an overworld name");
            return;
        }

        bool overworldNameExists = OverworldLoader.OverworldExists(_overworldName);

        if (!overworldNameExists)
        {
            Logger.Warning($"Could not find the overworld {_overworldName}");
            return;
        }

        if(GameManager.Instance.CurrentEditorLevel.UnsavedChanges)
        {
            // Ask player to confirm, so no unsaved changes are lost
            GameObject twoOptionsPanelGO = GameObject.Instantiate(EditorCanvasUI.Instance.EditorTwoOptionsPanelPrefab, EditorCanvasUI.Instance.transform);
            EditorTwoOptionPanel twoOptionsPanel = twoOptionsPanelGO.GetComponent<EditorTwoOptionPanel>();
            twoOptionsPanel.Initialize(
                $"There are unsaved changes. Do you want to proceed?",
                EditorUIAction.Close,
                "Cancel",
                EditorUIAction.LoadOverworld,
                $"Load {_overworldName}"
            );
        }

        OverworldGameplayManager.Instance.UnloadOverworld();

        OverworldData overworldData = OverworldLoader.LoadOverworldData(_overworldName);
        OverworldLoader.LoadOverworldForEditor(overworldData);

        EditorSelectedOverworldTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer as EditorSelectedOverworldTileModifierContainer;
        selectedTileModifierContainer?.SetInitialModifierValues();

        EditorOverworldTileModificationPanel.Instance?.Reset();
        EditorOverworldTileModificationPanel.Instance?.DestroyModifierActions();
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
        OverworldGameplayManager.Instance.UnloadOverworld();

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
                List<SerialisableTileCornerFiller> tileCornerFillers = new List<SerialisableTileCornerFiller>();

                SerialisableTileBaseGround baseBackground = TryAddBaseBackgroundForNewOverworld(tileBackgrounds, tileAttributes);

                if (baseBackground != null)
                {
                    string sanatisedBackgroundType = baseBackground.GetType().ToString().Split('.')[1];
                    tileBackgrounds.Add(new SerialisableTileBackground(sanatisedBackgroundType, baseBackground));
                }

                SerialisableTile tile = new SerialisableTile(tileId, mainMaterial, tileAttributes, tileBackgrounds, tileCornerFillers, gridLocation.X, gridLocation.Y);
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
            Type type = SerialisableTileBackground.GetType(tileBackgrounds[i].BackgroundType);

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

    public string GetOverworldName()
    {
        return _overworldName;
    }
}
