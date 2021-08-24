
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
            case EditorUIAction.SaveMaze:
                string mazeLevelName = EditorMazeModificationPanel.Instance.GetMazeLevelName();
                MazeLevelSaver mazeLevelSaver = new MazeLevelSaver();
                mazeLevelSaver.Save(mazeLevelName);

                Destroy(gameObject);
                break;
            case EditorUIAction.SaveOverworld:
                string overworldName = EditorOverworldModificationPanel.Instance.GetOverworldName();

                OverworldSaver overworldSaver = new OverworldSaver();
                overworldSaver.Save(overworldName); 
                Destroy(gameObject);
                break;
            case EditorUIAction.Close:
                Destroy(gameObject);
                break;
            default:
                Logger.Error($"The action {actionToExecute} was not yet implemented");
                break;
        }
    }
}
