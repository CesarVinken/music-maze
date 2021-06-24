using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class TileAreaToEnemySpawnpointAssigner : MonoBehaviour
{
    public static TileAreaToEnemySpawnpointAssigner Instance;

    [SerializeField] private Dropdown _tileAreaNamesDropdown;
    private Dictionary<OptionData, string> _tileAreaIdByDropdownOption = new Dictionary<OptionData, string>(); // <dropdownOption, id>

    [SerializeField] private Text _assignedAreasText;

    public void Awake()
    {
        //Guard.CheckIsNull(_assignedAreasContainer, "AssignedAreasContainer", gameObject);
        Guard.CheckIsNull(_tileAreaNamesDropdown, "_tileAreaNamesDropdown", gameObject);
        Guard.CheckIsNull(_assignedAreasText, "_assignedAreas", gameObject);

        Instance = this;
    }

    private void Start()
    {
        _tileAreaNamesDropdown.ClearOptions();

        List<OptionData> options = new List<OptionData>();


        OptionData defaultOption = new OptionData("");
        options.Add(defaultOption);

        foreach (KeyValuePair<string, TileArea> item in GameManager.Instance.CurrentEditorLevel.TileAreas)
        {
            string areaName = item.Value.Name;

            OptionData newOption = new OptionData(areaName);
            options.Add(newOption);

            _tileAreaIdByDropdownOption[newOption] = item.Key;
        }

        _tileAreaNamesDropdown.AddOptions(options);

        CheckForEnemySpawnpointOnTile();
    }

    // Every time we access a different tile, check if there is an enemy spawnpoint on it
    public void CheckForEnemySpawnpointOnTile()
    {
        MazeTile selectedTile = EditorTileSelector.Instance.CurrentlySelectedTile as MazeTile;
        EnemySpawnpoint enemySpawnpoint = selectedTile?.TryGetEnemySpawnpoint();

        if(enemySpawnpoint == null)
        {
            _tileAreaNamesDropdown.gameObject.SetActive(false);
            _assignedAreasText.gameObject.SetActive(false);
        }
        else
        {
            _tileAreaNamesDropdown.gameObject.SetActive(true);
            _assignedAreasText.gameObject.SetActive(true);

            RedrawAssignedAreasTest(enemySpawnpoint);
            RedrawDropdownOptions();
        }
    }

    private void RedrawAssignedAreasTest(EnemySpawnpoint enemySpawnpoint)
    {
        _assignedAreasText.text = GenerateAssignedAreasText(enemySpawnpoint);
    }

    private void RedrawDropdownOptions()
    {
        MazeTile selectedTile = EditorTileSelector.Instance.CurrentlySelectedTile as MazeTile;
        EnemySpawnpoint enemySpawnpoint = selectedTile?.TryGetEnemySpawnpoint();

        for (int i = 1; i < _tileAreaNamesDropdown.options.Count; i++)
        {
            string areaId = _tileAreaIdByDropdownOption[_tileAreaNamesDropdown.options[i]];
            TileArea tileArea = GameManager.Instance.CurrentEditorLevel.TileAreas[areaId];
            string originalAreaName = tileArea.Name;
            if (enemySpawnpoint.TileAreas.Contains(tileArea))
            {
                _tileAreaNamesDropdown.options[i].text = $"(X) {originalAreaName}";
            }
            else
            {
                _tileAreaNamesDropdown.options[i].text = originalAreaName;
            }
        }
    }

    public static void AssignTileAreaToEnemySpawnpoint()
    {
        Instance.AddTileAreaToSpawnpoint();
    }

    private void AddTileAreaToSpawnpoint()
    {
        if (_tileAreaNamesDropdown.value == 0)
        {
            return;
        }

        MazeTile selectedTile = EditorTileSelector.Instance.CurrentlySelectedTile as MazeTile;
        EnemySpawnpoint enemySpawnpoint = selectedTile?.TryGetEnemySpawnpoint();

        if(enemySpawnpoint == null)
        {
            Logger.Log("could not find enemySpawnpoint on the selected tile");
            return;
        }

        string currentlySelectedTileAreaId = GetIdCurrentSelectedTileArea();

        TileArea tileAreaInList = enemySpawnpoint.TileAreas.FirstOrDefault(tileArea => tileArea.Id == currentlySelectedTileAreaId);
        TileArea tileArea = GameManager.Instance.CurrentEditorLevel.TileAreas[currentlySelectedTileAreaId];
        if (tileAreaInList == null)
        {
            Logger.Log("did not find the area in the list, so we add it");

            enemySpawnpoint.AddTileArea(tileArea);
        }
        else
        {
            Logger.Log($"The currently selected tile area is {tileAreaInList.Name}");
            enemySpawnpoint.RemoveTileArea(tileArea);
        }
        
        _assignedAreasText.text = GenerateAssignedAreasText(enemySpawnpoint);

        RedrawDropdownOptions();

        _tileAreaNamesDropdown.value = 0;
    }

    private string GenerateAssignedAreasText(EnemySpawnpoint enemySpawnpoint)
    {
        string assignedAreaString = "Assigned areas:\n";
        for (int j = 0; j < enemySpawnpoint.TileAreas.Count; j++)
        {
            assignedAreaString += $"{enemySpawnpoint.TileAreas[j].Name}\n";
        }
        return assignedAreaString;
    }

    private string GetIdCurrentSelectedTileArea()
    {
        return _tileAreaIdByDropdownOption[_tileAreaNamesDropdown.options[_tileAreaNamesDropdown.value]];
    }
}
