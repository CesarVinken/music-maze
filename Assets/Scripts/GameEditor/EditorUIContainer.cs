using UnityEngine;

public class EditorUIContainer : MonoBehaviour
{
    public static EditorUIContainer Instance;

    public GameObject EditorModeStatusTextGO;
    public GameObject EditorUIGO;
    public GameObject PlayableLevelsPanelGO;

    public Sprite DefaultIcon;
    public Sprite[] TileAttributeIcons;

    public EditorGridGenerator GridGenerator;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(EditorModeStatusTextGO, "EditorModeStatusTextGO", gameObject);
        Guard.CheckIsNull(EditorUIGO, "EditorUIGO", gameObject);
        Guard.CheckIsNull(PlayableLevelsPanelGO, "PlayableLevelsPanelGO", gameObject);

        Guard.CheckIsNull(GridGenerator, "GridGenerator", gameObject);
    }

    public void InitialiseEditor()
    {
        EditorModeStatusTextGO.SetActive(true);
        EditorUIGO.SetActive(true);

        // TODO: transform current level into an editor level so that the player can continue editing the current level

        EditorWorldContainer.Instance.ShowTileSelector();
    }

    public void CloseEditor()
    {
        EditorModeStatusTextGO.SetActive(false);
        EditorUIGO.SetActive(false);
    }
}
