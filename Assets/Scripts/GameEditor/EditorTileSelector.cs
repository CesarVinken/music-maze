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
        if (EditorManager.EditorLevel == null) return;

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
        }
    }

    public void UpdateCurrentSelectedLocation(int xChange, int yChange)
    {
        int tempXPosition = _currentSelectedLocation.X + xChange;
        int tempYPosition = _currentSelectedLocation.Y + yChange;

        if (tempXPosition < 0) return;
        if (tempXPosition > EditorManager.EditorLevel.LevelBounds.X) return;

        if (tempYPosition < 0) return;
        if (tempYPosition > EditorManager.EditorLevel.LevelBounds.Y) return;

        CurrentSelectedLocation = new GridLocation(tempXPosition, tempYPosition);
    }
}
