using UnityEngine;
using UnityEngine.UI;

public class SwitchEditorButton : MonoBehaviour
{
    public static SwitchEditorButton Instance;

    [SerializeField] private Text _buttonLabel;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(_buttonLabel, "Button label");
    }

    public void Start()
    {
        SetButtonLabel();
    }

    public void SetButtonLabel()
    {
        if (GameManager.CurrentSceneType == SceneType.Maze)
        {
            _buttonLabel.text = "To Overworld Editor";
        }
        else
        {
            _buttonLabel.text = "To Maze Editor";
        }
    }

    public void SwitchEditor()
    {
        if(GameManager.CurrentSceneType == SceneType.Maze)
        {
            EditorCanvasUI.Instance.LoadOverworldEditor();
        }
        else
        {
            EditorCanvasUI.Instance.LoadMazeEditor();
        }
    }
}
