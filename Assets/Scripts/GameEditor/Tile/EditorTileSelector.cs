using UnityEngine;
using UnityEngine.EventSystems;

public class EditorTileSelector : MonoBehaviour
{
    private GridLocation _currentSelectedLocation;
    public GridLocation CurrentSelectedLocation
    {
        get { return _currentSelectedLocation; }
        set
        {
            _currentSelectedLocation = value;
            transform.position = new Vector2(_currentSelectedLocation.X, _currentSelectedLocation.Y);

            _lineRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y, -1));
            _lineRenderer.SetPosition(1, new Vector3(transform.position.x + 1, transform.position.y, -1));
            _lineRenderer.SetPosition(2, new Vector3(transform.position.x + 1, transform.position.y + 1, -1));
            _lineRenderer.SetPosition(3, new Vector3(transform.position.x, transform.position.y + 1, -1));
            _lineRenderer.SetPosition(4, new Vector3(transform.position.x, transform.position.y, -1));
        }
    }

    [SerializeField] private LineRenderer _lineRenderer;

    public void Awake()
    {
        Guard.CheckIsNull(_lineRenderer, "_lineRenderer", gameObject);
    }

    public void Start()
    {
        CurrentSelectedLocation = new GridLocation(0, 0);
    }

    void Update()
    {
        if (!EditorManager.InEditor) return;

        if (PlayableLevelsPanel.IsOpen) return;

        if (GameManager.Instance.CurrentEditorLevel == null) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SelectTileWithMouse();
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            UpdateCurrentSelectedLocation(0, 1);
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            UpdateCurrentSelectedLocation(-1, 0);
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            UpdateCurrentSelectedLocation(0, -1);
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            UpdateCurrentSelectedLocation(1, 0);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Logger.Log("Do something to tile {0}, {1}", CurrentSelectedLocation.X, CurrentSelectedLocation.Y);
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                PlaceTileModifierVariation();
            }
            else
            {
                PlaceTileModifier();
            }
        }
    }

    public void UpdateCurrentSelectedLocation(int xChange, int yChange)
    {
        int tempXPosition = _currentSelectedLocation.X + xChange;
        int tempYPosition = _currentSelectedLocation.Y + yChange;

        GridLocation selectedTileLocation = new GridLocation(tempXPosition, tempYPosition);

        if (!IsValidGridLocationToSelect(selectedTileLocation)) return;

        CurrentSelectedLocation = selectedTileLocation;
    }

    private void SelectTileWithMouse()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // clicked on the UI
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GridLocation selectedTileLocation = GridLocation.FindClosestGridTile(mousePosition);

        if (!IsValidGridLocationToSelect(selectedTileLocation)) return;

        CurrentSelectedLocation = selectedTileLocation;
    }

    private bool IsValidGridLocationToSelect(GridLocation selectedTileLocation)
    {   
        if (selectedTileLocation.X < 0) return false;
        if (selectedTileLocation.X > GameManager.Instance.CurrentEditorLevel.LevelBounds.X) return false;

        if (selectedTileLocation.Y < 0) return false;
        if (selectedTileLocation.Y > GameManager.Instance.CurrentEditorLevel.LevelBounds.Y) return false;

        return true;
    }

    private void PlaceTileModifier()
    {
        if (EditorCanvasUI.Instance.SelectedTileModifierContainer == null)
        {
            if(PersistentGameManager.CurrentSceneType == SceneType.Maze)
            {
                EditorModificationPanelContainer.Instance.SelectMazeTileModificationPanel();
            } else
            {
                EditorModificationPanelContainer.Instance.SelectOverworldTileModificationPanel();
            }
        }

        EditorTileModifierCategory editorTileModifierType = EditorManager.SelectedTileModifierCategory;
        EditorSelectedTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer;
        Logger.Log(editorTileModifierType);
        if (editorTileModifierType == EditorTileModifierCategory.Attribute)
        {
            EditorTileAttributeModifier attribute = selectedTileModifierContainer.EditorTileAttributes[EditorManager.SelectedTileAttributeModifierIndex];
            PlaceTileAttribute(CurrentSelectedLocation, attribute);
        }
        else if (editorTileModifierType == EditorTileModifierCategory.Background)
        {
            EditorTileBackgroundModifier background = selectedTileModifierContainer.EditorTileBackgrounds[EditorManager.SelectedTileBackgroundModifierIndex];
            PlaceTileBackground(CurrentSelectedLocation, background);
        }
        else if (editorTileModifierType == EditorTileModifierCategory.TransformationTriggerer)
        {
            EditorTileTransformationModifier transformationTriggerer = selectedTileModifierContainer.EditorTileTransformationTriggerers[EditorManager.SelectedTileTransformationTriggererIndex];
            PlaceTransformationTriggerer(CurrentSelectedLocation, transformationTriggerer);
        }
        else
        {
            Logger.Error("EditorMazeTileModifierType not yet implemented");
        }
    }

    private void PlaceTileModifierVariation()
    {
        if (EditorCanvasUI.Instance.SelectedTileModifierContainer == null)
        {
            if (PersistentGameManager.CurrentSceneType == SceneType.Maze)
            {
                EditorModificationPanelContainer.Instance.SelectMazeTileModificationPanel();
            }
            else
            {
                EditorModificationPanelContainer.Instance.SelectOverworldTileModificationPanel();
            }
        }

        EditorTileModifierCategory editorMazeTileModifierCategory = EditorManager.SelectedTileModifierCategory;

        if (editorMazeTileModifierCategory == EditorTileModifierCategory.Attribute)
        {
            EditorTileAttributeModifier attribute = EditorCanvasUI.Instance.SelectedTileModifierContainer.EditorTileAttributes[EditorManager.SelectedTileAttributeModifierIndex];
            PlaceTileAttributeVariation(CurrentSelectedLocation, attribute);
        }
        else if (editorMazeTileModifierCategory == EditorTileModifierCategory.Background)
        {
            EditorTileBackgroundModifier background = EditorCanvasUI.Instance.SelectedTileModifierContainer.EditorTileBackgrounds[EditorManager.SelectedTileBackgroundModifierIndex];
            PlaceMazeTileBackgroundVariation(CurrentSelectedLocation, background);
        }
    }

    private void PlaceTileAttribute(GridLocation gridLocation, EditorTileAttributeModifier attribute)
    {
        if (attribute == null) Logger.Error($"Could not find the attribute type {attribute.GetType()}");

        GameManager.Instance.CurrentEditorLevel.TilesByLocation.TryGetValue(gridLocation, out Tile tile);
        attribute.PlaceAttribute(tile);
    }

    private void PlaceTileBackground(GridLocation gridLocation, EditorTileBackgroundModifier background)
    {
        if (background == null) Logger.Error($"Could not find the background type {background.GetType()}");

        GameManager.Instance.CurrentEditorLevel.TilesByLocation.TryGetValue(gridLocation, out Tile tile);
        background.PlaceBackground(tile);
    }

    private void PlaceTileAttributeVariation(GridLocation gridLocation, EditorTileAttributeModifier attribute)
    {
        if (attribute == null) Logger.Error($"Could not find the attribute type {attribute.GetType()}");

        GameManager.Instance.CurrentEditorLevel.TilesByLocation.TryGetValue(gridLocation, out Tile tile);
        attribute.PlaceAttributeVariation(tile);
    }

    private void PlaceMazeTileBackgroundVariation(GridLocation gridLocation, EditorTileBackgroundModifier background)
    {
        if (background == null) Logger.Error($"Could not find the background type {background.GetType()}");

        GameManager.Instance.CurrentEditorLevel.TilesByLocation.TryGetValue(gridLocation, out Tile tile);
        background.PlaceBackgroundVariation<Tile>(tile);
    }

    private void PlaceTransformationTriggerer(GridLocation gridLocation, EditorTileTransformationModifier transformationTriggerer)
    {
        if (transformationTriggerer == null) Logger.Error($"Could not find the transformationTriggerer type {transformationTriggerer.GetType()}");

        GameManager.Instance.CurrentEditorLevel.TilesByLocation.TryGetValue(gridLocation, out Tile tile);
        transformationTriggerer.HandleBeautificationTriggerPlacement(tile);
    }
}
