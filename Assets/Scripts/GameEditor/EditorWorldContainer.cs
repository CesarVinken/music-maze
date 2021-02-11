using UnityEngine;

public class EditorWorldContainer : MonoBehaviour
{
    public static EditorWorldContainer Instance;

    [SerializeField] private GameObject _editorTileSelectorGO;
    [SerializeField] private EditorTileSelector _editorTileSelector;

    private void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(_editorTileSelector, "EditorTileSelector", gameObject);
        Guard.CheckIsNull(_editorTileSelectorGO, "EditorTileSelectorGO", gameObject);

        GameObject.DontDestroyOnLoad(gameObject);
    }

    public void InitialiseEditor()
    {
        gameObject.SetActive(true);
    }

    public void CloseEditor()
    {
        gameObject.SetActive(false);
    }

    public void ShowTileSelector()
    {
        _editorTileSelector.CurrentSelectedLocation = new GridLocation(0, 0);
        _editorTileSelectorGO.SetActive(true);
    }

    public void HideTileSelector()
    {
        _editorTileSelectorGO.SetActive(false);
    }
}
