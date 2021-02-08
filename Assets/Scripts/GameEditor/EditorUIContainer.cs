using UnityEngine;

public class EditorUIContainer : MonoBehaviour
{
    public static EditorUIContainer Instance;

    public GameObject EditorModeStatusTextGO;
    public GameObject EditorUIGO;
    public GameObject PlayableLevelsPanelGO;

    public Sprite DefaultIcon;
    public Sprite[] TileAttributeIcons;

    public EditorLevelModificationPanel GridGenerator;

    [Space(10)]
    [Header("Tile Modifier Actions")]
    public GameObject GenerateTileTransformationMapPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(EditorModeStatusTextGO, "EditorModeStatusTextGO", gameObject);
        Guard.CheckIsNull(EditorUIGO, "EditorUIGO", gameObject);
        Guard.CheckIsNull(PlayableLevelsPanelGO, "PlayableLevelsPanelGO", gameObject);

        Guard.CheckIsNull(GridGenerator, "GridGenerator", gameObject);

        Guard.CheckIsNull(GenerateTileTransformationMapPrefab, "GenerateTileTransformationMapPrefab");
    }

    public void InitialiseEditor()
    {
        EditorModeStatusTextGO.SetActive(true);
        EditorUIGO.SetActive(true);

        EditorWorldContainer.Instance.ShowTileSelector();
    }

    public void CloseEditor()
    {
        EditorModeStatusTextGO.SetActive(false);
        EditorUIGO.SetActive(false);
    }
}
