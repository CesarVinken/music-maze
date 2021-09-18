using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorTileSelector : MonoBehaviour
{
    public static EditorTileSelector Instance;

    private Tile _currentSelectedTile;
    private List<Tile> _colouredOverlayTiles = new List<Tile>();

    public Tile CurrentlySelectedTile
    {
        get { return _currentSelectedTile; }
        set
        {
            _currentSelectedTile = value;
            transform.position = new Vector2(_currentSelectedTile.GridLocation.X, _currentSelectedTile.GridLocation.Y);

            _lineRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y, -1));
            _lineRenderer.SetPosition(1, new Vector3(transform.position.x + 1, transform.position.y, -1));
            _lineRenderer.SetPosition(2, new Vector3(transform.position.x + 1, transform.position.y + 1, -1));
            _lineRenderer.SetPosition(3, new Vector3(transform.position.x, transform.position.y + 1, -1));
            _lineRenderer.SetPosition(4, new Vector3(transform.position.x, transform.position.y, -1));
        }
    }

    public EditorMazeTile OverlayImageTile;

    [SerializeField] private LineRenderer _lineRenderer;

    public void Awake()
    {
        Guard.CheckIsNull(_lineRenderer, "_lineRenderer", gameObject);

        Instance = this;
    }

    public void Start()
    {
        CurrentlySelectedTile = GameManager.Instance.CurrentEditorLevel.TilesByLocation[new GridLocation(0, 0)];
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
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            SelectTileWithMouse();
            TrySelectForTileOverlay();
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
            Logger.Log("Do something to tile {0}, {1}", CurrentlySelectedTile.GridLocation.X, CurrentlySelectedTile.GridLocation.Y);

            SelectModificationPanel();

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                PlaceTileModifierVariation();
            }
            else
            {
                PlaceTileModifier();
            }
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            PerformDeletionAction();
        }
    }

    public void UpdateCurrentSelectedLocation(int xChange, int yChange)
    {
        int tempXPosition = _currentSelectedTile.GridLocation.X + xChange;
        int tempYPosition = _currentSelectedTile.GridLocation.Y + yChange;

        if (GameManager.Instance.CurrentEditorLevel.TilesByLocation.TryGetValue(new GridLocation(tempXPosition, tempYPosition), out Tile selectedTile))
        {
            if (!IsValidGridLocationToSelect(selectedTile.GridLocation)) return;
        }
        else
        {
            return;
        }

        CurrentlySelectedTile = selectedTile;

        UpdateEditorUIForSelectedLocation();
    }

    private void UpdateEditorUIForSelectedLocation()
    {
        if(EditorManager.SelectedTileModifier != null && EditorManager.SelectedTileModifier is EditorEnemySpawnpointTileAttribute)
        {
            TileAreaToEnemySpawnpointAssigner.Instance?.CheckForEnemySpawnpointOnTile();
        }
        else if(EditorManager.SelectedTileModifier != null && EditorManager.SelectedTileModifier is EditorFerryRouteTileAttribute)
        {
            FerryRouteDrawingModeAccessor.Instance?.CheckForFerryRouteOnTile();
        }
    }

    private void SelectTileWithMouse()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // clicked on the UI
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(GameManager.Instance.CurrentEditorLevel.TilesByLocation.TryGetValue(GridLocation.FindClosestGridTile(mousePosition), out Tile selectedTile))
        {
            if (!IsValidGridLocationToSelect(selectedTile.GridLocation)) return;
        }
        else
        {
            return;
        }

        CurrentlySelectedTile = selectedTile;
        UpdateEditorUIForSelectedLocation();
    }

    public void TrySelectForTileOverlay()
    {
        EditorTileModifierCategory editorTileModifierType = EditorManager.SelectedTileModifierCategory;

        if (editorTileModifierType == EditorTileModifierCategory.TransformationTriggerer)
        {
            EditorSelectedTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer;
            List<IEditorTileModifier> transformationTriggerers = selectedTileModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.TransformationTriggerer];
            EditorMazeTileBeautificationTriggerer transformationTriggerer = transformationTriggerers[EditorManager.SelectedTileTransformationTriggererIndex] as EditorMazeTileBeautificationTriggerer;

            if (transformationTriggerer == null)
            {
                return;
            }

            EditorMazeTile selectedTile = CurrentlySelectedTile as EditorMazeTile;
            if (selectedTile == null) return;

            transformationTriggerer.SetSelectedTile(selectedTile);
        }
        else if (editorTileModifierType == EditorTileModifierCategory.Attribute)
        {
            EditorSelectedTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer;
            List<IEditorTileModifier> attributes = selectedTileModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.Attribute];
            EditorTileAttributeModifier selectedAttribute = attributes[EditorManager.SelectedTileAttributeModifierIndex] as EditorTileAttributeModifier;

            if (selectedAttribute is EditorFerryRouteTileAttribute)
            {
                EditorFerryRouteTileAttribute ferryRouteAttribute = selectedAttribute as EditorFerryRouteTileAttribute;

                if (ferryRouteAttribute == null)
                {
                    Logger.Log("return for ferryRoute attribute");
                    return;
                }

                // Only draw overlay if we are in drawing mode
                if (!FerryRouteDrawingModeAccessor.InDrawingMode)
                {
                    Logger.Log("return for not in drawing mode");
                    return;
                }

                FerryRoute selectedFerryRoute = FerryRouteDrawingModeAccessor.SelectedFerryRoute;

                if (selectedFerryRoute == null)
                {
                    Logger.Log("return for no ferryRoute selected");
                    return;
                }

                EditorMazeTile editorMazeTile = _currentSelectedTile as EditorMazeTile;
                if(editorMazeTile.TileOverlayMode != TileOverlayMode.Green) {
                    List<FerryRoutePoint> currentFerryRoutePoints = selectedFerryRoute.GetFerryRoutePoints();
                    FerryRoutePoint alreadyExistingPoint = currentFerryRoutePoints.FirstOrDefault(p => p.Tile.TileId.Equals(_currentSelectedTile.TileId));
                    if (alreadyExistingPoint != null)
                    {
                        if (currentFerryRoutePoints.Count < 2 && alreadyExistingPoint.Tile.TileId.Equals(currentFerryRoutePoints[0].Tile.TileId))
                        {
                            selectedFerryRoute.TryTurnDocking(FerryDockingType.DockingStart);
                        }
                        else if (alreadyExistingPoint.Tile.TileId.Equals(currentFerryRoutePoints[currentFerryRoutePoints.Count - 1].Tile.TileId))
                        {
                            selectedFerryRoute.TryTurnDocking(FerryDockingType.DockingEnd);
                        }
                        else
                        {
                            selectedFerryRoute.RemoveFerryRoutePoints(alreadyExistingPoint);
                        }

                    }
                    return;
                }

                selectedFerryRoute.AddFerryRoutePointInEditor(_currentSelectedTile);
                ResetColouredTiles();
                FerryRouteDrawingModeAccessor.Instance.ColourAddableTiles();
            }
        }
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
        EditorTileModifierCategory editorTileModifierType = EditorManager.SelectedTileModifierCategory;
        EditorSelectedTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer;

        GameManager.Instance.CurrentEditorLevel.TilesByLocation.TryGetValue(CurrentlySelectedTile.GridLocation, out Tile tile);

        if (editorTileModifierType == EditorTileModifierCategory.Attribute)
        {
            List<IEditorTileModifier> attributes = selectedTileModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.Attribute];
            EditorTileAttributeModifier attribute = attributes[EditorManager.SelectedTileAttributeModifierIndex] as EditorTileAttributeModifier;
            PlaceTileAttribute(tile, attribute);
        }
        else if (editorTileModifierType == EditorTileModifierCategory.Background)
        {
            List<IEditorTileModifier> backgrounds = selectedTileModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.Background];
            EditorTileBackgroundModifier background = backgrounds[EditorManager.SelectedTileBackgroundModifierIndex] as EditorTileBackgroundModifier;
            PlaceTileBackground(tile, background);
        }
        else if (editorTileModifierType == EditorTileModifierCategory.TransformationTriggerer)
        {
            List<IEditorTileModifier> transformationTriggerers = selectedTileModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.TransformationTriggerer];
            EditorTileTransformationModifier transformationTriggerer = transformationTriggerers[EditorManager.SelectedTileTransformationTriggererIndex] as EditorTileTransformationModifier;
            PlaceTransformationTriggerer(tile, transformationTriggerer);
        }
        else if (editorTileModifierType == EditorTileModifierCategory.Area)
        {
            List<IEditorTileModifier> areaModifiers = selectedTileModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.Area];
            EditorTileAreaModifier tileAreaModifier = areaModifiers[EditorManager.SelectedTileAreaModifierIndex] as EditorTileAreaModifier;
            AddToSelectedArea(tile, tileAreaModifier);
        }
        else
        {
            Logger.Error($"EditorMazeTileModifierType {editorTileModifierType} not yet implemented");
        }

        GameManager.Instance.CurrentEditorLevel.UnsavedChanges = true;
    }

    private void SelectModificationPanel()
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
    }

    private void PlaceTileModifierVariation()
    {

        EditorTileModifierCategory editorMazeTileModifierCategory = EditorManager.SelectedTileModifierCategory;
        GameManager.Instance.CurrentEditorLevel.TilesByLocation.TryGetValue(CurrentlySelectedTile.GridLocation, out Tile tile);
        EditorSelectedTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer;

        if (editorMazeTileModifierCategory == EditorTileModifierCategory.Attribute)
        {
            List<IEditorTileModifier> attributes = selectedTileModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.Attribute];
            EditorTileAttributeModifier attribute = attributes[EditorManager.SelectedTileAttributeModifierIndex] as EditorTileAttributeModifier;
            PlaceTileAttributeVariation(tile, attribute);
        }
        else if (editorMazeTileModifierCategory == EditorTileModifierCategory.Background)
        {
            List<IEditorTileModifier> backgrounds = selectedTileModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.Background];
            EditorTileBackgroundModifier background = backgrounds[EditorManager.SelectedTileBackgroundModifierIndex] as EditorTileBackgroundModifier;
            PlaceMazeTileBackgroundVariation(tile, background);
        }

        GameManager.Instance.CurrentEditorLevel.UnsavedChanges = true;
    }

    private void PlaceTileAttribute(Tile tile, EditorTileAttributeModifier attribute)
    {
        if (attribute == null) Logger.Error($"Could not find the attribute type {attribute.GetType()}");

        attribute.PlaceAttribute(tile);
    }

    private void PlaceTileBackground(Tile tile, EditorTileBackgroundModifier background)
    {
        if (background == null) Logger.Error($"Could not find the background type {background.GetType()}");

        background.PlaceBackground(tile);
    }

    private void PlaceTileAttributeVariation(Tile tile, EditorTileAttributeModifier attribute)
    {
        if (attribute == null) Logger.Error($"Could not find the attribute type {attribute.GetType()}");

        attribute.PlaceAttributeVariation(tile);
    }

    private void PlaceMazeTileBackgroundVariation(Tile tile, EditorTileBackgroundModifier background)
    {
        if (background == null) Logger.Error($"Could not find the background type {background.GetType()}");

        background.PlaceBackgroundVariation<Tile>(tile);
    }

    private void PlaceTransformationTriggerer(Tile tile, EditorTileTransformationModifier transformationTriggerer)
    {
        if (transformationTriggerer == null) Logger.Error($"Could not find the transformationTriggerer type {transformationTriggerer.GetType()}");

        transformationTriggerer.HandleBeautificationTriggerPlacement(tile);
    }

    private void AddToSelectedArea(Tile tile, EditorTileAreaModifier areaModifier)
    {
        if (areaModifier == null) Logger.Error($"Could not find the areaModifier type {areaModifier.GetType()}");

        areaModifier.SetSelectedTile(tile);
    }

    private void PerformDeletionAction()
    {
        EditorTileModifierCategory editorTileModifierType = EditorManager.SelectedTileModifierCategory;
        EditorSelectedTileModifierContainer selectedTileModifierContainer = EditorCanvasUI.Instance.SelectedTileModifierContainer;

        if (editorTileModifierType == EditorTileModifierCategory.TransformationTriggerer)
        {
            List<IEditorTileModifier> transformationTriggerers = selectedTileModifierContainer.CurrentlyAvailableTileModifiers[EditorTileModifierCategory.TransformationTriggerer];
            EditorTileTransformationModifier transformationTriggerer = transformationTriggerers[EditorManager.SelectedTileTransformationTriggererIndex] as EditorTileTransformationModifier;

            if (transformationTriggerer == null) Logger.Error($"Could not find the transformationTriggerer type {transformationTriggerer.GetType()}");
            transformationTriggerer.RemoveAllTriggerersFromTile();
        }
    }

    public void ResetColouredTiles()
    {
        for (int i = 0; i < _colouredOverlayTiles.Count; i++)
        {
            IEditorTile editorMazeTile = _colouredOverlayTiles[i] as IEditorTile;
            editorMazeTile.SetTileOverlayImage(TileOverlayMode.Empty);
        }

        _colouredOverlayTiles.Clear();
    }

    public void AddColouredTile(Tile tile)
    {
        _colouredOverlayTiles.Add(tile);
    }
}
