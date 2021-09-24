using Character;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

public class Ferry : MonoBehaviour
{
    public static List<Ferry> Ferries = new List<Ferry>();
        
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public PlayerCharacter ControllingPlayerCharacter { get; private set; }
    public FerryRoute FerryRoute { get; private set; }

    public FerryDirection FerryDirection;
    public MazeTile CurrentLocationTile = null;

    private Tile _dockingStartTile;
    private Tile _dockingEndTile;

    private GameObject _controlFerryButtonGO = null;
    private string _controlFerryButtonId;
    private List<PlayerCharacter> _playersOnFerry = new List<PlayerCharacter>();

    public void Initialise(FerryRoute ferryRoute)
    {
        FerryRoute = ferryRoute;

        GridLocation location = GridLocation.FindClosestGridTile(transform.position);
        SetNewCurrentLocation(location);
        Ferries.Add(this);

        _dockingStartTile = ferryRoute.GetFerryDocking(FerryDockingType.DockingStart).DockingTile;
        _dockingEndTile = ferryRoute.GetFerryDocking(FerryDockingType.DockingEnd)?.DockingTile;

        PlayerCharacter.NewPlayerGridLocationEvent += OnPlayerOnNewGridLocation;
    }

    public void Update()
    {
        if (EditorManager.InEditor) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            HandleFerryControlByKeyboard();
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleControlFerryButton();         
        }
        else if(Input.GetMouseButtonDown(1))
        {
            TryDestroyControlFerryButton();
        }

        HandleMoveFerryWithPlayer();
    }

    // Listen for event that is triggered by player, each time they have a new CurrentPosition
    public void OnPlayerOnNewGridLocation(PlayerCharacter playerCharacter)
    {
        if(_playersOnFerry.Count > 0)
        {
            if (!PlayerIsOnFerry(playerCharacter))
            {
                // The player WAS on the ferry but leaves
                if (_playersOnFerry.Contains(playerCharacter))
                {
                    _playersOnFerry.Remove(playerCharacter);

                    // Destroy the map interaction button if it was existing
                    if (_controlFerryButtonGO)
                    {
                        MainScreenCameraCanvas.Instance.DestroyMapInteractionButton(_controlFerryButtonId);
                    }
                    Logger.Log($"Removed player {playerCharacter.Name} from ferry");
                }
            }
        }
        // The player is on the ferry, but not controlling
        if (PlayerIsOnFerry(playerCharacter) &&
            ControllingPlayerCharacter == null
            ) 
        {
            if (!_playersOnFerry.Contains(playerCharacter))
            {
                Logger.Log($" added player {playerCharacter.Name} to ferry list");
                _playersOnFerry.Add(playerCharacter);
            }
        }

        if (_controlFerryButtonGO == null) return;

        return;
    }

    private bool PlayerIsOnFerry(PlayerCharacter playerCharacter)
    {
        if (playerCharacter.CurrentGridLocation.X == CurrentLocationTile.GridLocation.X &&
            playerCharacter.CurrentGridLocation.Y == CurrentLocationTile.GridLocation.Y) return true;

        return false;
    }

    private List<PlayerCharacter> GetPlayerCharacters()
    {
        if(PersistentGameManager.CurrentSceneType == SceneType.Maze)
        {
            return GameManager.Instance.CharacterManager.GetPlayers<MazePlayerCharacter>().Select(p => p.Value as PlayerCharacter).ToList();
        }
        else
        {
            return GameManager.Instance.CharacterManager.GetPlayers<OverworldPlayerCharacter>().Select(p => p.Value as PlayerCharacter).ToList();
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

    public void SetControllingPlayerCharacter(PlayerCharacter playerCharacter)
    {
        //Logger.Log($"Is moving? {playerCharacter?.IsMoving}");

        ControllingPlayerCharacter = playerCharacter;

        if(playerCharacter == null) // If there is no longer a controlling character, make the ferry route points inaccible again
        {
            Logger.Log($"No one is controlling the ferry now");
            MakeFerryRouteAccessibleForPlayer(false);
        }
        else
        {
            Logger.Log($"The player controlling the ferry is now {ControllingPlayerCharacter?.Name}");
            MakeFerryRouteAccessibleForPlayer(true);
        }
    }

    public void TryDestroyControlFerryButton()
    {
        if (_controlFerryButtonGO)
        {
            MainScreenCameraCanvas.Instance.DestroyMapInteractionButton(_controlFerryButtonId);
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

    private void HandleFerryControlByKeyboard()
    {
        // if the ferry is not in a docking place, do not listen to the Return key
        if (CurrentLocationTile?.TileId != _dockingStartTile.TileId &&
            CurrentLocationTile?.TileId != _dockingEndTile.TileId)
        {
            return;
        }

        List<PlayerCharacter> playerCharacters = GetPlayerCharacters();

        for (int i = 0; i < playerCharacters.Count; i++)
        {
            PlayerCharacter playerCharacter = playerCharacters[i];
            if (playerCharacter.CurrentGridLocation.X == CurrentLocationTile?.GridLocation.X &&
                playerCharacter.CurrentGridLocation.Y == CurrentLocationTile?.GridLocation.Y)
            {
                playerCharacter.ToggleFerryControl(this);
            }
        }
    }

    private void HandleMoveFerryWithPlayer()
    {
        if (ControllingPlayerCharacter != null)
        {
            transform.position = new Vector2(ControllingPlayerCharacter.transform.position.x - 0.5f, ControllingPlayerCharacter.transform.position.y - 0.5f);
            if (CurrentLocationTile.GridLocation.X != ControllingPlayerCharacter.CurrentGridLocation.X ||
                CurrentLocationTile.GridLocation.Y != ControllingPlayerCharacter.CurrentGridLocation.Y)
            {
                //if(_controlFerryButtonGO != null)
                //{
                //    MainScreenCameraCanvas.Instance.DestroyMapInteractionButton(_controlFerryButtonId);
                //}

                SetNewCurrentLocation(ControllingPlayerCharacter.CurrentGridLocation);
            }
        }
    }

    private void HandleControlFerryButton()
    {
        // if the ferry is not in a docking place, do not listen to the Mouse click
        if (CurrentLocationTile?.TileId != _dockingStartTile.TileId &&
            CurrentLocationTile?.TileId != _dockingEndTile.TileId)
        {
            return;
        }

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GridLocation clickedGridLocation = GridLocation.FindClosestGridTile(worldPoint);

        // check if we clicked on the ferry tile
        if (clickedGridLocation.X != CurrentLocationTile.GridLocation.X || clickedGridLocation.Y != CurrentLocationTile.GridLocation.Y)
        {
            TryDestroyControlFerryButton();
            return;
        }

        // try to get the player on the ferry tile
        PlayerCharacter playerCharacter = GetPlayerCharacters().FirstOrDefault(
            p => p.CurrentGridLocation.X == CurrentLocationTile.GridLocation.X && p.CurrentGridLocation.Y == CurrentLocationTile.GridLocation.Y
            );

        // TODO: Make sure the clicked player is OUR player, not the other player

        if (playerCharacter == null) return;

        if (playerCharacter.CurrentGridLocation.X == CurrentLocationTile?.GridLocation.X &&
            playerCharacter.CurrentGridLocation.Y == CurrentLocationTile?.GridLocation.Y)
        {
            if (_controlFerryButtonGO != null) return;
            if (playerCharacter.IsMoving) return;

            Sprite buttonSprite = GetFerryButtonSprite();

            _controlFerryButtonGO = MainScreenCameraCanvas.Instance.CreateMapInteractionButton(
                    playerCharacter,
                    new Vector2(CurrentLocationTile.GridLocation.X, CurrentLocationTile.GridLocation.Y - 1),
                    MapInteractionAction.PerformControlFerryAction,
                    "",
                    buttonSprite);

            MapInteractionButton mapInteractionButton = _controlFerryButtonGO.GetComponent<MapInteractionButton>();
            if (mapInteractionButton == null) return;

            _controlFerryButtonId = mapInteractionButton.Id;
        }
    }

    private Sprite GetFerryButtonSprite()
    {
        if (ControllingPlayerCharacter)
        {
            return MainScreenCameraCanvas.Instance.FerryBoardingIcons[1];
        }

        return MainScreenCameraCanvas.Instance.FerryBoardingIcons[0];
    }
}
