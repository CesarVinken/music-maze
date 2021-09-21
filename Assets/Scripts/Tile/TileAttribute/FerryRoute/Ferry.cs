using Character;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class Ferry : MonoBehaviour
{
    public static List<Ferry> Ferries = new List<Ferry>();
        
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public MazePlayerCharacter ControllingPlayerCharacter { get; private set; }
    public FerryRoute FerryRoute { get; private set; }

    public FerryDirection FerryDirection;
    public MazeTile CurrentLocationTile = null;

    private Tile _dockingStartTile;
    private Tile _dockingEndTile;

    public void Initialise(FerryRoute ferryRoute)
    {
        FerryRoute = ferryRoute;

        GridLocation location = GridLocation.FindClosestGridTile(transform.position);
        SetNewCurrentLocation(location);
        Ferries.Add(this);

        _dockingStartTile = ferryRoute.GetFerryDocking(FerryDockingType.DockingStart).DockingTile;
        _dockingEndTile = ferryRoute.GetFerryDocking(FerryDockingType.DockingEnd)?.DockingTile;
    }

    public void Update()
    {
        if (EditorManager.InEditor) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            // if the ferry is not in a docking place, do not listen to the Return key
            if(CurrentLocationTile?.TileId != _dockingStartTile.TileId &&
                CurrentLocationTile?.TileId != _dockingEndTile.TileId)
            {
                return;
            }

            Dictionary <PlayerNumber, MazePlayerCharacter> playerCharacters = GameManager.Instance.CharacterManager.GetPlayers<MazePlayerCharacter>();

            foreach (KeyValuePair<PlayerNumber, MazePlayerCharacter> item in playerCharacters)
            {
                MazePlayerCharacter playerCharacter = item.Value;
                if (playerCharacter.CurrentGridLocation.X == CurrentLocationTile?.GridLocation.X &&
                    playerCharacter.CurrentGridLocation.Y == CurrentLocationTile?.GridLocation.Y)
                {
                    Logger.Log($"playerCharacter {playerCharacter.Name} is on the Ferry and wants to activate it");
                    playerCharacter.ToggleFerryControl(this);
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            // if the ferry is not in a docking place, do not listen to the Mouse click
            if (CurrentLocationTile?.TileId != _dockingStartTile.TileId &&
                CurrentLocationTile?.TileId != _dockingEndTile.TileId)
            {
                return;
            }

            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            
            if (hit.collider != null)
            {
                Logger.Log(hit.transform.name);
                MazePlayerCharacter playerCharacter = hit.transform.gameObject.GetComponent<MazePlayerCharacter>();
                
                if (playerCharacter == null)
                {
                    return;
                }

                if (playerCharacter.CurrentGridLocation.X == CurrentLocationTile?.GridLocation.X &&
                    playerCharacter.CurrentGridLocation.Y == CurrentLocationTile?.GridLocation.Y)
                {
                    Logger.Log($"playerCharacter {playerCharacter.Name} is on the Ferry and wants to activate it");
                    Sprite buttonSprite = EditorCanvasUI.Instance.TileAttributeIcons[9]; // Temporary

                    MainScreenCameraCanvas.Instance.CreateMapInteractionButton(
                            playerCharacter,
                            new Vector2(CurrentLocationTile.GridLocation.X, CurrentLocationTile.GridLocation.Y - 1),
                            MapInteractionAction.PerformControlFerryAction,
                            "",
                            buttonSprite);
                }
            }
        }

        if(ControllingPlayerCharacter != null)
        {
            transform.position = new Vector2(ControllingPlayerCharacter.transform.position.x - 0.5f, ControllingPlayerCharacter.transform.position.y - 0.5f);
            if(CurrentLocationTile.GridLocation.X != ControllingPlayerCharacter.CurrentGridLocation.X ||
                CurrentLocationTile.GridLocation.Y != ControllingPlayerCharacter.CurrentGridLocation.Y)
            {
                SetNewCurrentLocation(ControllingPlayerCharacter.CurrentGridLocation);
                Logger.Log($"Current location of FERRY is now {CurrentLocationTile.GridLocation.X}, {CurrentLocationTile.GridLocation.Y}");
            }
        }
    }

    public void SetDirection(FerryDirection ferryDirection)
    {
        FerryDirection = ferryDirection;

        switch (FerryDirection)
        {
            case FerryDirection.Horizontal:
                _spriteRenderer.sprite = MazeSpriteManager.Instance.FerryRouteSprites[4];
                break;
            case FerryDirection.Vertical:
                _spriteRenderer.sprite = MazeSpriteManager.Instance.FerryRouteSprites[5];
                break;
            default:
                Logger.Error($"Unknown ferry direction {ferryDirection}");
                break;
        }
    }

    public void SetNewCurrentLocation(GridLocation newCurrentLocation)
    {
        if (CurrentLocationTile != null)
        {
            CurrentLocationTile.SetWalkable(false);
        }

        CurrentLocationTile = GameManager.Instance.CurrentGameLevel.TilesByLocation[newCurrentLocation] as MazeTile;
        CurrentLocationTile.SetWalkable(true);
    }

    public void SetControllingPlayerCharacter(MazePlayerCharacter playerCharacter)
    {
        ControllingPlayerCharacter = playerCharacter;

        if(playerCharacter == null) // If there is no longer a controlling character, make the ferry route points inaccible again
        {
            Logger.Log($"None is controlling the ferry now");
            MakeFerryRouteAccessibleForPlayer(false);
        }
        else
        {
            Logger.Log($"The player controlling the ferry is now {ControllingPlayerCharacter?.Name}");
            MakeFerryRouteAccessibleForPlayer(true);
        }
    }

    public void MakeFerryRouteAccessibleForPlayer(bool makeAccessible)
    {
        List<FerryRoutePoint> ferryRoutePoints = FerryRoute.GetFerryRoutePoints();

        for (int i = 0; i < ferryRoutePoints.Count; i++)
        {
            Tile tile = ferryRoutePoints[i].Tile;
            //The current tile of the ferry should always be accessible
            if (tile.TileId.Equals(CurrentLocationTile.TileId))
            {
                ferryRoutePoints[i].Tile.SetWalkable(true);
                continue;
            }
            tile.SetWalkable(makeAccessible);
        }
    }
}
