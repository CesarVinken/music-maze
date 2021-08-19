using DataSerialisation;
using UnityEngine;
using UnityEngine.UI;

public class PlayableMazeLevelNameToggle : MonoBehaviour
{
    private MazeLevelNameData _levelNameData;
    public Toggle Toggle;
    [SerializeField] private Text _levelNameToggleLabel;

    public void Awake()
    {
        Guard.CheckIsNull(_levelNameToggleLabel, "LevelNameToggleLabel", gameObject);    
    }

    public void Initialise(MazeLevelNameData levelNameData)
    {
        _levelNameData = levelNameData;

        gameObject.name = $"{_levelNameData.LevelName}Toggle";
        _levelNameToggleLabel.text = _levelNameData.LevelName;
        Toggle.isOn = levelNameData.IsPlayable;
    }

    public void ToggleLevelPlayability(bool isPlayable)
    {
        _levelNameData.IsPlayable = isPlayable;
        Logger.Log($"Toggled playability of the maze level {_levelNameData.LevelName} to {isPlayable}");
    }

    public void DeleteMazeLevel()
    {
        Logger.Log($"Delete {_levelNameData.LevelName}");

        string sanatisedLevelName = _levelNameData.LevelName.ToLower().Replace(" ", "-");

        bool levelExists = MazeLevelLoader.MazeLevelExists(sanatisedLevelName);

        if (!levelExists)
        {
            return;
        }

        JsonMazeLevelFileWriter.DeleteFile(sanatisedLevelName);

        MazeLevelNamesData levelNamesData = new MazeLevelNamesData(sanatisedLevelName);
        levelNamesData.DeleteLevelName(sanatisedLevelName);

        Destroy(gameObject);
    }
}
