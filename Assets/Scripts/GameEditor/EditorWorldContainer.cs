using UnityEngine;

public class EditorWorldContainer : MonoBehaviour
{
    public static EditorWorldContainer Instance;

    public GameObject EditorTileSelectorGO;
    public EditorTileSelector EditorTileSelector;

    private void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(EditorTileSelector, "EditorTileSelector", gameObject);
        Guard.CheckIsNull(EditorTileSelectorGO, "EditorTileSelectorGO", gameObject);
    }

    public void InitialiseEditor()
    {
    }

    public void CloseEditor()
    {
        EditorTileSelectorGO.SetActive(false);
    }

    public void ShowTileSelector()
    {
        EditorTileSelector.CurrentSelectedLocation = new GridLocation(0, 0);
        EditorTileSelectorGO.SetActive(true);
    }
}
