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

        if (MazeLevelManager.Instance.EditorLevel == null) return;

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
                PlaceMazeTileModifierVariation();
            }
            else
            {
                PlaceMazeTileModifier();
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
        if (selectedTileLocation.X > MazeLevelManager.Instance.EditorLevel.LevelBounds.X) return false;

        if (selectedTileLocation.Y < 0) return false;
        if (selectedTileLocation.Y > MazeLevelManager.Instance.EditorLevel.LevelBounds.Y) return false;

        return true;
    }

    private void PlaceMazeTileModifier()
    {
        EditorMazeTileModifierType editorMazeTileModifierType = EditorManager.SelectedMazeTileModifierType;

        if(editorMazeTileModifierType == EditorMazeTileModifierType.Attribute)
        {
            IEditorMazeTileAttribute attribute = EditorSelectedModifierContainer.Instance.EditorMazeTileAttributes[EditorManager.SelectedMazeTileAttributeModifierIndex];
            PlaceMazeTileAttribute(CurrentSelectedLocation, attribute);
        }
        else if (editorMazeTileModifierType == EditorMazeTileModifierType.Background)
        {
            IEditorMazeTileBackground background = EditorSelectedModifierContainer.Instance.EditorMazeTileBackgrounds[EditorManager.SelectedMazeTileBackgroundModifierIndex];
            PlaceMazeTileBackground(CurrentSelectedLocation, background);
        }
        else if (editorMazeTileModifierType == EditorMazeTileModifierType.TransformationTriggerer)
        {
            IEditorMazeTileTransformationTriggerer transformationTriggerer = EditorSelectedModifierContainer.Instance.EditorMazeTileTransformationTriggerers[EditorManager.SelectedMazeTileTransformationTriggererIndex];
            PlaceTransformationTriggerer(CurrentSelectedLocation, transformationTriggerer);
        }
        else
        {
            Logger.Error("EditorMazeTileModifierType not yet implemented");
        }
    }

    private void PlaceMazeTileModifierVariation()
    {
        Logger.Log("place a variation");
        EditorMazeTileModifierType editorMazeTileModifierType = EditorManager.SelectedMazeTileModifierType;

        if (editorMazeTileModifierType == EditorMazeTileModifierType.Attribute)
        {
            IEditorMazeTileAttribute attribute = EditorSelectedModifierContainer.Instance.EditorMazeTileAttributes[EditorManager.SelectedMazeTileAttributeModifierIndex];
            PlaceMazeTileAttributeVariation(CurrentSelectedLocation, attribute);
        }
        else if (editorMazeTileModifierType == EditorMazeTileModifierType.Background)
        {
            IEditorMazeTileBackground background = EditorSelectedModifierContainer.Instance.EditorMazeTileBackgrounds[EditorManager.SelectedMazeTileBackgroundModifierIndex];
            PlaceMazeTileBackgroundVariation(CurrentSelectedLocation, background);
        }
    }

    private void PlaceMazeTileAttribute(GridLocation gridLocation, IEditorMazeTileAttribute attribute)
    {
        if (attribute == null) Logger.Error($"Could not find the attribute type {attribute.GetType()}");

        MazeLevelManager.Instance.EditorLevel.TilesByLocation.TryGetValue(gridLocation, out EditorTile tile);
        attribute.PlaceAttribute(tile);
    }

    private void PlaceMazeTileBackground(GridLocation gridLocation, IEditorMazeTileBackground background)
    {
        if (background == null) Logger.Error($"Could not find the background type {background.GetType()}");

        MazeLevelManager.Instance.EditorLevel.TilesByLocation.TryGetValue(gridLocation, out EditorTile tile);
        background.PlaceBackground(tile);
    }

    private void PlaceMazeTileAttributeVariation(GridLocation gridLocation, IEditorMazeTileAttribute attribute)
    {
        if (attribute == null) Logger.Error($"Could not find the attribute type {attribute.GetType()}");

        MazeLevelManager.Instance.EditorLevel.TilesByLocation.TryGetValue(gridLocation, out EditorTile tile);
        attribute.PlaceAttributeVariation(tile);
    }

    private void PlaceMazeTileBackgroundVariation(GridLocation gridLocation, IEditorMazeTileBackground background)
    {
        if (background == null) Logger.Error($"Could not find the background type {background.GetType()}");

        MazeLevelManager.Instance.EditorLevel.TilesByLocation.TryGetValue(gridLocation, out EditorTile tile);
        background.PlaceBackgroundVariation(tile);
    }

    private void PlaceTransformationTriggerer(GridLocation gridLocation, IEditorMazeTileTransformationTriggerer transformationTriggerer)
    {
        if (transformationTriggerer == null) Logger.Error($"Could not find the transformationTriggerer type {transformationTriggerer.GetType()}");

        MazeLevelManager.Instance.EditorLevel.TilesByLocation.TryGetValue(gridLocation, out EditorTile tile);
        transformationTriggerer.HandleTransformationTriggerPlacement(tile);
    }
}
