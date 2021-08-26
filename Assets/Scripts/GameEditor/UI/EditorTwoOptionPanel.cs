
using DataSerialisation;
using UnityEngine;
using UnityEngine.UI;

public class EditorTwoOptionPanel : MonoBehaviour
{
    [SerializeField] private Button _optionAButton;
    [SerializeField] private Button _optionBButton;

    [SerializeField] private Text _messageText;
    [SerializeField] private Text _optionAButtonText;
    [SerializeField] private Text _optionBButtonText;
    public void Initialize(string description, EditorUIAction optionAAction, string optionAButtonLabel, EditorUIAction optionBAction, string optionBButtonLabel)
    {
        _messageText.text = description;
        _optionAButtonText.text = optionAButtonLabel;
        _optionBButtonText.text = optionBButtonLabel;

        _optionAButton.onClick.AddListener(() =>
        {
            ExecuteAction(optionAAction);
        });

        _optionBButton.onClick.AddListener(() =>
        {
            ExecuteAction(optionBAction);
        });
    }


    public void ShowMessage(string message)
    {
        _messageText.text = message;
        gameObject.SetActive(true);
    }

    public void ExecuteAction(EditorUIAction actionToExecute)
    {
        switch (actionToExecute)
        {
            case EditorUIAction.LoadMaze:
                ExecuteLoadMaze();
                break;
            case EditorUIAction.LoadOverworld:
                ExecuteLoadOverworld();
                break;
            case EditorUIAction.SaveMaze:
                ExecuteSaveMaze();
                break;
            case EditorUIAction.SaveOverworld:
                ExecuteSaveOverworld();
                break;
            case EditorUIAction.Close:
                break;
            default:
                Logger.Error($"The action {actionToExecute} was not yet implemented");
                break;
        }
    }

    private void ExecuteLoadMaze()
    {
        MazeLevelGameplayManager.Instance.UnloadLevel();

        string mazeLevelName = EditorMazeModificationPanel.Instance.GetMazeLevelName();
        MazeLevelData mazeLevelData = MazeLevelLoader.LoadMazeLevelData(mazeLevelName);
        MazeLevelLoader.LoadMazeLevelForEditor(mazeLevelData);

        EditorSelectedMazeTileModifierContainer selectedMazeTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer as EditorSelectedMazeTileModifierContainer;
        selectedMazeTileModifierContainer?.SetInitialModifierValues();

        EditorMazeTileModificationPanel.Instance?.Reset();
        EditorMazeTileModificationPanel.Instance?.DestroyModifierActions();

        Destroy(gameObject);
    }

    private void ExecuteLoadOverworld()
    {
        OverworldGameplayManager.Instance.UnloadOverworld();

        string overworldName = EditorOverworldModificationPanel.Instance.GetOverworldName();
        OverworldData overworldData = OverworldLoader.LoadOverworldData(overworldName);
        OverworldLoader.LoadOverworldForEditor(overworldData);

        EditorSelectedOverworldTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer as EditorSelectedOverworldTileModifierContainer;
        selectedTileModifierContainer?.SetInitialModifierValues();

        EditorOverworldTileModificationPanel.Instance?.Reset();
        EditorOverworldTileModificationPanel.Instance?.DestroyModifierActions();

        Destroy(gameObject);
    }

    private void ExecuteSaveMaze()
    {
        string mazeLevelNameToSave = EditorMazeModificationPanel.Instance.GetMazeLevelName();
        MazeLevelSaver mazeLevelSaver = new MazeLevelSaver();
        mazeLevelSaver.Save(mazeLevelNameToSave);

        Destroy(gameObject);
    }

    private void ExecuteSaveOverworld()
    {
        string overworldName = EditorOverworldModificationPanel.Instance.GetOverworldName();

        OverworldSaver overworldSaver = new OverworldSaver();
        overworldSaver.Save(overworldName);
        Destroy(gameObject);
    }
}
