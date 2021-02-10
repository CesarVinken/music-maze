using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorCanvasUI : MonoBehaviour
{
    public static EditorCanvasUI Instance;

    public GameObject EditorModeStatusTextGO;
    public GameObject PlayableLevelsPanelGO;

    public Sprite DefaultIcon;
    public Sprite[] TileAttributeIcons;

    public EditorMazeModificationPanel GridGenerator;

    [Space(10)]
    [Header("Tile Modifier Actions")]
    public GameObject GenerateTileTransformationMapPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(EditorModeStatusTextGO, "EditorModeStatusTextGO", gameObject);
        Guard.CheckIsNull(PlayableLevelsPanelGO, "PlayableLevelsPanelGO", gameObject);

        Guard.CheckIsNull(GridGenerator, "GridGenerator", gameObject);

        Guard.CheckIsNull(GenerateTileTransformationMapPrefab, "GenerateTileTransformationMapPrefab");

        GameObject.DontDestroyOnLoad(gameObject);
    }

    public void InitialiseEditor()
    {
        EditorModeStatusTextGO.SetActive(true);
        gameObject.SetActive(true);

        EditorWorldContainer.Instance.ShowTileSelector();
    }

    public void CloseEditor()
    {
        EditorModeStatusTextGO.SetActive(false);
        gameObject.SetActive(false);
    }

    public void LoadOverworldEditor()
    {
        SceneManager.LoadScene("Overworld");
    }

    public void LoadMazeEditor()
    {
        SceneManager.LoadScene("Maze");
    }
}
