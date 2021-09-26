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
    public FerryRoutePoint CurrentFerryRoutePoint { get; private set; }
    public List<PlayerCharacter> PlayersOnFerry { get => _playersOnFerry; set => _playersOnFerry = value; }

    public FerryDirection FerryDirection;
    public MazeTile CurrentLocationTile = null;

    private Tile _dockingStartTile;
    private Tile _dockingEndTile;

    private GameObject _controlFerryButtonGO = null;
    private string _controlFerryButtonId;
    private List<PlayerCharacter> _playersOnFerry = new List<PlayerCharacter>();

    public void Initialise(FerryRoute ferryRoute)
    {
        Logger.Log("Initialise Ferry");
        FerryRoute = ferryRoute;

        GridLocation location = GridLocation.FindClosestGridTile(transform.position);
        SetNewCurrentLocation(FerryRoute.GetFerryRoutePointByLocation(location));
        Ferries.Add(this);

        _dockingStartTile = ferryRoute.GetFerryDocking(FerryDockingType.DockingStart).DockingTile;
        _dockingEndTile = ferryRoute.GetFerryDocking(FerryDockingType.DockingEnd)?.DockingTile;

        PlayerCharacter.NewPlayerGridLocationEvent += OnPlayerOnNewGridLocation;
    }

    public void Update()
    {
        if (EditorManager.InEditor) return;

        if (Input.GetKeyDown(GameManager.Instance.KeyboardConfiguration.Player1Action))
        {
            HandleFerryControlByKeyboard(PlayerNumber.Player1);
        }
        else if(GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        {
            if (Input.GetKeyDown(GameManager.Instance.KeyboardConfiguration.Player2Action))
            {
                HandleFerryControlByKeyboard(PlayerNumber.Player2);
            }
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
        GridLocation newPlayerGridLocation = playerCharacter.CurrentGridLocation;

        // If at least one player is already on the ferry, make calculations to see if players should be removed, or if their CurrentLocation should be updated
        if (_playersOnFerry.Count > 0)
        {
            // if this player character is not already on the ferry,
            // It means that either:
            // A new player entered a ferry
            // Or
            // The controlling player took the ferry to a new location, and the ferry's location should be updated - IF the new tile is on the ferry route
            if (!PlayerIsOnFerryLocation(playerCharacter))
            {
                if(_playersOnFerry.Contains(playerCharacter))
                {
                    FerryRoutePoint ferryRoutePointForPlayerLocation = GetFerryRoutePointForPlayerLocation(newPlayerGridLocation);
                    if(ferryRoutePointForPlayerLocation == null) // if the player's location is not aligning any ferry route point, it means the player is off the route
                    {
                        _playersOnFerry.Remove(playerCharacter);
                        Logger.Log($"Removed player {playerCharacter.Name} from ferry");
                    }
                    else if(ControllingPlayerCharacter == playerCharacter)
                    {
                        SetNewCurrentLocation(ferryRoutePointForPlayerLocation);
                        for (int i = 0; i < _playersOnFerry.Count; i++)
                        {
                            if(_playersOnFerry[i] != playerCharacter)
                            {
                                PlayerCharacter otherPlayer = _playersOnFerry[i];
                                otherPlayer.SetCurrentGridLocation(playerCharacter.CurrentGridLocation);
                            }
                        }
                    }
                }
            }
        }

        if (PlayerIsOnFerryLocation(playerCharacter)) 
        {
            if (!_playersOnFerry.Contains(playerCharacter))
            {
                Logger.Warning($" added player {playerCharacter.Name} to ferry list");
                _playersOnFerry.Add(playerCharacter);
            }
        }

        if (_controlFerryButtonGO == null) return;

        return;
    }

    private FerryRoutePoint GetFerryRoutePointForPlayerLocation (GridLocation playerCharacterLocation)
    {
        FerryRoutePoint oldFerryRoutePoint = CurrentFerryRoutePoint;
        List<FerryRoutePoint> ferryRoutePoints = FerryRoute.GetFerryRoutePoints();
        int indexOldFerryRoutePoint = ferryRoutePoints.IndexOf(oldFerryRoutePoint);
        if (indexOldFerryRoutePoint == 0) // we're at the beginning
        {
            FerryRoutePoint nextPointOnRoute = ferryRoutePoints[indexOldFerryRoutePoint + 1];
            if (nextPointOnRoute.Tile.GridLocation.X == playerCharacterLocation.X &&
                nextPointOnRoute.Tile.GridLocation.Y == playerCharacterLocation.Y)
            {
                // We moved to the next point on the route
                return nextPointOnRoute;
            }
        }
        else if (indexOldFerryRoutePoint == ferryRoutePoints.Count - 1) // we're at the end
        {
            FerryRoutePoint previousPointOnRoute = ferryRoutePoints[indexOldFerryRoutePoint - 1];
            if (previousPointOnRoute.Tile.GridLocation.X == playerCharacterLocation.X &&
                previousPointOnRoute.Tile.GridLocation.Y == playerCharacterLocation.Y)
            {
                // We moved to the previous point on the route
                return previousPointOnRoute;
            }
        }
        else // we're in the middle
        {
            if (ferryRoutePoints[indexOldFerryRoutePoint + 1].Tile.GridLocation.X == playerCharacterLocation.X &&
                ferryRoutePoints[indexOldFerryRoutePoint + 1].Tile.GridLocation.Y == playerCharacterLocation.Y)
            {
                // We moved to the next point on the route
                return ferryRoutePoints[indexOldFerryRoutePoint + 1];
            }
            else if (ferryRoutePoints[indexOldFerryRoutePoint - 1].Tile.GridLocation.X == playerCharacterLocation.X &&
                ferryRoutePoints[indexOldFerryRoutePoint - 1].Tile.GridLocation.Y == playerCharacterLocation.Y)
            {
                // We moved to the previous point on the route
                return ferryRoutePoints[indexOldFerryRoutePoint - 1];
            }
        }
        return null;
    }

    private bool PlayerIsOnFerryLocation(PlayerCharacter playerCharacter)
    {
        if (playerCharacter.CurrentGridLocation.X == CurrentLocationTile.GridLocation.X &&
            playerCharacter.CurrentGridLocation.Y == CurrentLocationTile.GridLocation.Y) return true;
        return false;
    }

    private List<PlayerCharacter> GetPlayerCharacters()
    {
        if (PersistentGameManager.CurrentSceneType == SceneType.Maze)
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

    public void SetNewCurrentLocation(FerryRoutePoint nextFerryRoutePoint)
    {
        Logger.Log($"Update current ferry route point to {nextFerryRoutePoint.Tile.GridLocation.X} {nextFerryRoutePoint.Tile.GridLocation.Y}");
        CurrentFerryRoutePoint = nextFerryRoutePoint;

        if (CurrentLocationTile != null)
        {
            CurrentLocationTile.SetWalkable(false);
        }

        CurrentLocationTile = GameManager.Instance.CurrentGameLevel.TilesByLocation[nextFerryRoutePoint.Tile.GridLocation] as MazeTile;
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

    private void HandleFerryControlByKeyboard(PlayerNumber triggerPlayerNumber)
    {
        // if the ferry is not in a docking place, do not listen to the Return key
        if (CurrentLocationTile?.TileId != _dockingStartTile.TileId &&
            CurrentLocationTile?.TileId != _dockingEndTile.TileId)
        {
            return;
        }

        PlayerCharacter triggeringPlayer = CharacterHelper.GetUnbiasedPlayerCharacter(triggerPlayerNumber);

        // Return if the other player is already controlling the ferry
        if(ControllingPlayerCharacter != null && ControllingPlayerCharacter != triggeringPlayer)
        {
            return;
        }

        if (triggeringPlayer.CurrentGridLocation.X == CurrentLocationTile?.GridLocation.X &&
            triggeringPlayer.CurrentGridLocation.Y == CurrentLocationTile?.GridLocation.Y)
        {
            triggeringPlayer.ToggleFerryControl(this);
        }
    }

    private void HandleMoveFerryWithPlayer()
    {
        if (ControllingPlayerCharacter != null)
        {
            transform.position = new Vector2(ControllingPlayerCharacter.transform.position.x - 0.5f, ControllingPlayerCharacter.transform.position.y - 0.5f);
            
            // Other players should move with the ferry, if the ferry moves
            if(_playersOnFerry.Count > 1)
            {
                for (int i = 0; i < _playersOnFerry.Count; i++)
                {
                    if (_playersOnFerry[i] == ControllingPlayerCharacter) continue;

                    PlayerCharacter otherPlayer = _playersOnFerry[i];
                    if (otherPlayer.IsMoving) continue;

                    otherPlayer.transform.position = new Vector2(ControllingPlayerCharacter.transform.position.x, ControllingPlayerCharacter.transform.position.y);
                }
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
