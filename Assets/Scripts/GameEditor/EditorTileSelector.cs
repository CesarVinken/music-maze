using System.Linq;
using UnityEngine;

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

            _lineRenderer.SetPosition(0, new Vector2(transform.position.x, transform.position.y));
            _lineRenderer.SetPosition(1, new Vector2(transform.position.x + 1, transform.position.y));
            _lineRenderer.SetPosition(2, new Vector2(transform.position.x + 1, transform.position.y + 1));
            _lineRenderer.SetPosition(3, new Vector2(transform.position.x, transform.position.y + 1));
            _lineRenderer.SetPosition(4, new Vector2(transform.position.x, transform.position.y));
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
        if (MazeLevelManager.Instance.Level == null) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SelectTileWithMouse();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            UpdateCurrentSelectedLocation(0, 1);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            UpdateCurrentSelectedLocation(-1, 0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            UpdateCurrentSelectedLocation(0, -1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            UpdateCurrentSelectedLocation(1, 0);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Logger.Log("Do something to tile {0}, {1}", CurrentSelectedLocation.X, CurrentSelectedLocation.Y);
            EditorMazeTileAttributeType attributeType = EditorManager.SelectedMazeTileAttributeType;
            PlaceMazeTileAttribute(attributeType, CurrentSelectedLocation);
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
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GridLocation selectedTileLocation = GridLocation.FindClosestGridTile(mousePosition);

        if (!IsValidGridLocationToSelect(selectedTileLocation)) return;

        CurrentSelectedLocation = selectedTileLocation;
    }

    private bool IsValidGridLocationToSelect(GridLocation selectedTileLocation)
    {
        if (selectedTileLocation.X < 0) return false;
        if (selectedTileLocation.X > MazeLevelManager.Instance.Level.LevelBounds.X) return false;

        if (selectedTileLocation.Y < 0) return false;
        if (selectedTileLocation.Y > MazeLevelManager.Instance.Level.LevelBounds.Y) return false;

        return true;
    }

    public void PlaceMazeTileAttribute(EditorMazeTileAttributeType attributeType, GridLocation gridLocation)
    {
        IEditorMazeTileAttribute attribute = EditorSelectedAttributeContainer.Instance.EditorMazeTileAttributes.FirstOrDefault(a => a.AttributeType == attributeType);

        if (attribute == null) Logger.Error($"Could not find the attribute type {attributeType}");

        MazeLevelManager.Instance.Level.TilesByLocation.TryGetValue(gridLocation, out Tile tile);
        attribute.PlaceAttribute(tile);
    }
}
