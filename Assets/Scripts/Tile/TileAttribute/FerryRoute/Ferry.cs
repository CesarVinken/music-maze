using Character;
using System;
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
    public bool IsMoving { get; private set; }

    public FerryDirection FerryDirection;
    public MazeTile CurrentLocationTile = null;

    private Tile _dockingStartTile;
    private Tile _dockingEndTile;

    private GameObject _controlFerryButtonGO = null;
    private string _controlFerryButtonId;
    private List<PlayerCharacter> _playersOnFerry = new List<PlayerCharacter>();
    private List<FerryDirectionIndicator> _ferryDirectionIndicators = new List<FerryDirectionIndicator>();
    private List<ObjectSelectionIndicator> _ferrySelectionIndicators = new List<ObjectSelectionIndicator>();

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

        if(GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
        {
            if (Input.GetKeyDown(GameManager.Instance.KeyboardConfiguration.Player1Action))
            {
                PlayerNumber ourPlayerCharacterNumber = GameManager.Instance.CharacterManager.GetOurPlayerCharacter();
                HandleFerryControlByKeyboard(ourPlayerCharacterNumber);
            }
        }
        else
        {
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
        if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && !playerCharacter.PhotonView.IsMine)
        {
            return;
        }

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
                    FerryRoutePoint ferryRoutePointForPlayerLocation = FerryRoute.GetFerryRoutePointForPlayerLocation(newPlayerGridLocation, CurrentFerryRoutePoint);
                    if(ferryRoutePointForPlayerLocation == null) // if the player's location is not aligning any ferry route point, it means the player is off the route
                    {
                        RemovePlayerOnFerry(playerCharacter);
                        ToggleFerrySelectionIndicator(false, playerCharacter);

                        if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
                        {
                            PlayerFerryBoardingEvent playerFerryBoardingEvent = new PlayerFerryBoardingEvent();
                            playerFerryBoardingEvent.SendPlayerFerryBoardingEvent(FerryRoute.Id, playerCharacter, false);
                        }
                    }
                    else if(ControllingPlayerCharacter == playerCharacter)
                    {
                        Logger.Log($"Ferry was moved. Set ferry location to {ferryRoutePointForPlayerLocation.Tile.GridLocation.X}, {ferryRoutePointForPlayerLocation.Tile.GridLocation.Y}");
                        SetNewCurrentLocation(ferryRoutePointForPlayerLocation);
                    }
                }
            }
        }

        if (PlayerIsOnFerryLocation(playerCharacter)) 
        {
            if (!_playersOnFerry.Contains(playerCharacter))
            {
                AddPlayerOnFerry(playerCharacter);

                if(ControllingPlayerCharacter == null)
                {
                    ToggleFerrySelectionIndicator(true, playerCharacter);
                }

                if(GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
                {
                    PlayerFerryBoardingEvent playerFerryBoardingEvent = new PlayerFerryBoardingEvent();
                    playerFerryBoardingEvent.SendPlayerFerryBoardingEvent(FerryRoute.Id, playerCharacter, true);
                }
            }
        }

        if (_controlFerryButtonGO == null) return;

        return;
    }  

    private void ToggleFerrySelectionIndicator(bool showIndidator, PlayerCharacter playerCharacter)
    {
        //Logger.Log($"Player {playerCharacter.Name} Toggled FerrySelectionIndicator to {showIndidator}. Before the action count is     {_ferrySelectionIndicators.Count}");
        if (!showIndidator)
        {
            if(GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
            {
                if (ControllingPlayerCharacter != null)
                {
                    //Logger.Warning($"The scenario of Turning off the indicator and there IS a controlling playerCharacter");
                    for (int i = 0; i < _ferrySelectionIndicators.Count; i++)
                    {
                        ObjectSelectionIndicatorPool.Instance.ReturnToPool(_ferrySelectionIndicators[i]);
                    }
                    _ferrySelectionIndicators.Clear();
                }
                else
                {
                    //Logger.Warning($"The scenario of Turning off the indicator and there is NOT a ferry controlling playerCharacter");

                    for (int i = _ferrySelectionIndicators.Count - 1; i >= 0; i--)
                    {
                        if (_ferrySelectionIndicators[i].PlayerCharacter == playerCharacter)
                        {
                            ObjectSelectionIndicatorPool.Instance.ReturnToPool(_ferrySelectionIndicators[i]);
                            _ferrySelectionIndicators.Remove(_ferrySelectionIndicators[i]);
                        }
                    }
                    for (int j = 0; j < _playersOnFerry.Count; j++)
                    {
                        if (playerCharacter != _playersOnFerry[j])
                        {
                            ObjectSelectionIndicator ferrySelectionIndicator = _ferrySelectionIndicators.FirstOrDefault(f => f.PlayerCharacter == _playersOnFerry[j]);
                            if (!ferrySelectionIndicator)
                            {
                                //Logger.Log($"Turn one on for {playerCharacter.Name}, because there wasn't one yet");
                                ToggleFerrySelectionIndicator(true, _playersOnFerry[j]);
                            }
                        }
                    }
                }
            }
            else
            {
                //Logger.Log($"We want to remove something. Is there a controller? {ControllingPlayerCharacter != null}. The triggerer was {playerCharacter.Name}");
                if (ControllingPlayerCharacter)
                {
                    for (int i = 0; i < _ferrySelectionIndicators.Count; i++)
                    {
                        ObjectSelectionIndicatorPool.Instance.ReturnToPool(_ferrySelectionIndicators[i]);
                    }
                    _ferrySelectionIndicators.Clear();
                }
                else
                {
                    ObjectSelectionIndicator ferrySelectionIndicator = _ferrySelectionIndicators.FirstOrDefault(f => f.PlayerCharacter == playerCharacter);
                    if (ferrySelectionIndicator)
                    {
                        _ferrySelectionIndicators.Remove(ferrySelectionIndicator);
                        ObjectSelectionIndicatorPool.Instance.ReturnToPool(ferrySelectionIndicator);
                    }
                }

            }
        }
        else
        {
            if(GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
            {
                if (!playerCharacter.PhotonView.IsMine) return;
            }

            ObjectSelectionIndicator ferrySelectionIndicator = ObjectSelectionIndicatorPool.Instance.Get();
            Direction direction = Direction.Right;

            ferrySelectionIndicator.Initialise(playerCharacter, direction, transform, ObjectSelectionIndicatorType.Ferry);
            ferrySelectionIndicator.transform.SetParent(MainScreenCameraCanvas.Instance.transform);
            ferrySelectionIndicator.gameObject.SetActive(true);
            _ferrySelectionIndicators.Add(ferrySelectionIndicator);
        }
        //Logger.Log($"Count is now  {_ferrySelectionIndicators.Count}");
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
        Logger.Warning($"Update current ferry route point to {nextFerryRoutePoint.Tile.GridLocation.X} {nextFerryRoutePoint.Tile.GridLocation.Y}");
        CurrentFerryRoutePoint = nextFerryRoutePoint;

        if (CurrentLocationTile != null)
        {
            CurrentLocationTile.SetWalkable(false);
        }

        CurrentLocationTile = GameManager.Instance.CurrentGameLevel.TilesByLocation[nextFerryRoutePoint.Tile.GridLocation] as MazeTile;
        CurrentLocationTile.SetWalkable(true);

        if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
        {
            for (int i = 0; i < _playersOnFerry.Count; i++)
            {
                if (_playersOnFerry[i] != ControllingPlayerCharacter)
                {
                    PlayerCharacter otherPlayer = _playersOnFerry[i];
                    //Logger.Log($"The OTHER player on the ferry is {otherPlayer.Name}. We update its Current Position to {CurrentLocationTile.GridLocation.X}, {CurrentLocationTile.GridLocation.Y}");
                    otherPlayer.SetCurrentGridLocation(CurrentLocationTile.GridLocation);
                }
            }
        }

        UpdateDirectionIndicators();
    }

    public void UnsetControllingPlayerCharacter(PlayerCharacter playerCharacter, bool playerIsStatic = false)
    {
        //Logger.Log($"Unsetting player status SetHasCalculatedTarget? {playerCharacter.HasCalculatedTarget}.  {playerCharacter?.IsMoving}");
        PlayerCharacter oldControllingPlayer = ControllingPlayerCharacter;
        ControllingPlayerCharacter = null;
        if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && oldControllingPlayer != null && !oldControllingPlayer.PhotonView.IsMine)
        {
            // TODO: Find better method to have the ferry at the correct location when the client player moves off and leaves the ferry hanging somewhere halfway a tile.
            // For now: force move the ferry to the tile middle
            GridLocation endGridLocation = GridLocation.FindClosestGridTile(new Vector2(oldControllingPlayer.transform.position.x, oldControllingPlayer.transform.position.y));
            transform.position = new Vector2(endGridLocation.X, endGridLocation.Y);
        }
        Logger.Log($"No one is controlling the ferry now. Unsetter: {playerCharacter.Name}. Number of players on ferry: {_playersOnFerry.Count}, {playerIsStatic}");
        MakeFerryRouteAccessibleForPlayer(false);

        if (playerIsStatic) // if the player is static, then the unsetting happened because of a clicking/keyboard action, and not because a player character walked off
        {
            for (int i = 0; i < _playersOnFerry.Count; i++)
            {
                ToggleFerrySelectionIndicator(true, _playersOnFerry[i]);
            }
        }
        else if(GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && !playerCharacter.PhotonView.IsMine) // happens if we got this order through an external event
        {
            for (int i = 0; i < _playersOnFerry.Count; i++)
            {
                if (!_playersOnFerry[i].PhotonView.IsMine) continue;

                ToggleFerrySelectionIndicator(true, _playersOnFerry[i]);
            }
        }

        IsMoving = false;

        UpdateDirectionIndicators();
    }

    public void SetControllingPlayerCharacter(PlayerCharacter playerCharacter)
    {
        ControllingPlayerCharacter = playerCharacter;

        //if(playerCharacter == null) // If there is no longer a controlling character, make the ferry route points inaccessible again
        //{
        //    if(GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && oldControllingPlayer != null && !oldControllingPlayer.PhotonView.IsMine)
        //    {
        //        // TODO: Find better method to have the ferry at the correct location when the client player moves off and leaves the ferry hanging somewhere halfway a tile.
        //        // For now: force move the ferry to the tile middle
        //        GridLocation endGridLocation = GridLocation.FindClosestGridTile(new Vector2(oldControllingPlayer.transform.position.x, oldControllingPlayer.transform.position.y));
        //        transform.position =  new Vector2(endGridLocation.X, endGridLocation.Y);
        //    }
        //    Logger.Log($"No one is controlling the ferry now");
        //    MakeFerryRouteAccessibleForPlayer(false);
        //    Logger.Log($"Player status SetHasCalculatedTarget? {oldControllingPlayer?.IsCalculatingPath}");
        //    ToggleFerrySelectionIndicator(true, playerCharacter);
        //    IsMoving = false;
        //}
        //else
        //{
            Logger.Log($"The player controlling the ferry is now {ControllingPlayerCharacter?.Name}");
            MakeFerryRouteAccessibleForPlayer(true);
            ToggleFerrySelectionIndicator(false, playerCharacter);
        //}

        UpdateDirectionIndicators();
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
            triggeringPlayer.ToggleFerryControl(this, true);
        }
    }

    private void HandleMoveFerryWithPlayer()
    {
        if (ControllingPlayerCharacter != null)
        {
            if (ControllingPlayerCharacter.IsMoving)
            {
                IsMoving = true;
            }
            else
            {
                IsMoving = false;
            }

            transform.position = new Vector2(ControllingPlayerCharacter.transform.position.x - 0.5f, ControllingPlayerCharacter.transform.position.y - 0.5f);
            //Logger.Log(transform.position);   

            // Other players should move with the ferry, if the ferry moves
            if(_playersOnFerry.Count > 1)
            {
                for (int i = 0; i < _playersOnFerry.Count; i++)
                {
                    //Logger.Log($"{_playersOnFerry[i].Name}: {_playersOnFerry[i].CurrentGridLocation.X}, {_playersOnFerry[i].CurrentGridLocation.Y}");
                    if (_playersOnFerry[i] == ControllingPlayerCharacter) continue;

                    PlayerCharacter otherPlayer = _playersOnFerry[i];
                    if (otherPlayer.IsMoving) continue;

                    otherPlayer.transform.position = new Vector2(ControllingPlayerCharacter.transform.position.x, ControllingPlayerCharacter.transform.position.y);

                }
            }

            //// if the ferry has a controller, keep checking the current grid location of the ferry. This is needed in order to update the walkability across the network
            if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && !ControllingPlayerCharacter.PhotonView.IsMine
                //|| _playersOnFerry.Count > 1
                )
            {
                float usedposX = (float)Math.Ceiling(transform.position.x);
                float posXCloseness = (float)Math.Ceiling(transform.position.x) - transform.position.x;
                if (posXCloseness > 0.1f)
                {
                    usedposX = transform.position.x;
                }
                float usedposY = (float)Math.Ceiling(transform.position.y);
                float posYCloseness = (float)Math.Ceiling(transform.position.y) - transform.position.y;
                if (posYCloseness > 0.1f)
                {
                    usedposY = transform.position.y;
                }

                GridLocation currentGridLocation = GridLocation.FindClosestGridTile(new Vector2(usedposX, usedposY));
                if (currentGridLocation.X == CurrentLocationTile.GridLocation.X && currentGridLocation.Y == CurrentLocationTile.GridLocation.Y) return;
                //Logger.Log($"Search for ferry route point on grid location {currentGridLocation.X}, {currentGridLocation.Y} because the roundedPos is {usedposX} {usedposY}. (old) current location of ferry is {CurrentLocationTile.GridLocation.X}, {CurrentLocationTile.GridLocation.Y}");
                FerryRoutePoint ferryRoutePointForPlayerLocation = FerryRoute.GetFerryRoutePointForPlayerLocation(currentGridLocation, CurrentFerryRoutePoint);

                if (ferryRoutePointForPlayerLocation == null)
                {
                    //Logger.Log($"Could not find a ferryRoutePointForPlayerLocation at {currentGridLocation.X}, {currentGridLocation.Y}");
                    return;
                }
                //Logger.Warning($"Found a ferryRoutePointForPlayerLocation at {currentGridLocation.X}, {currentGridLocation.Y}");

                SetNewCurrentLocation(ferryRoutePointForPlayerLocation);
            }
        }
    }

    private void HandleControlFerryButton()
    {
        // if the ferry is not in a docking place, do not listen to the Mouse click
        if (CurrentLocationTile?.TileId != _dockingStartTile?.TileId &&
            CurrentLocationTile?.TileId != _dockingEndTile?.TileId)
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
        PlayerCharacter playerCharacter = null;
        List<PlayerCharacter> playerCharacters = GetPlayerCharacters();
        for (int i = 0; i < playerCharacters.Count; i++)
        {
            if(playerCharacters[i].CurrentGridLocation.X == CurrentLocationTile.GridLocation.X &&
                playerCharacters[i].CurrentGridLocation.Y == CurrentLocationTile.GridLocation.Y)
            {
                if(GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer &&
                    !playerCharacters[i].PhotonView.IsMine)
                {
                    continue;
                }
                playerCharacter = playerCharacters[i];
            }
        }
    
        if (playerCharacter == null) return;

        if (playerCharacter.CurrentGridLocation.X == CurrentLocationTile?.GridLocation.X &&
            playerCharacter.CurrentGridLocation.Y == CurrentLocationTile?.GridLocation.Y)
        {
            if (_controlFerryButtonGO != null) return;
            if (playerCharacter.IsMoving) return;
            if (ControllingPlayerCharacter && GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && !ControllingPlayerCharacter.PhotonView.IsMine) return;

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

    private void UpdateDirectionIndicators()
    {
        if(ControllingPlayerCharacter == null)
        {
            for (int i = 0; i < _ferryDirectionIndicators.Count; i++)
            {
                FerryDirectionIndicatorPool.Instance.ReturnToPool(_ferryDirectionIndicators[i]);
            }
            _ferryDirectionIndicators.Clear();
            return;
        }

        if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && !ControllingPlayerCharacter.PhotonView.IsMine) return;

        // check what directions are needed
        FerryRoutePoint nextFerryRoutePoint = FerryRoute.GetNextFerryRoutePoint(CurrentFerryRoutePoint);
        FerryRoutePoint previousFerryRoutePoint = FerryRoute.GetPreviousFerryRoutePoint(CurrentFerryRoutePoint);

        List<Direction> neededDirections = new List<Direction>();

        if (nextFerryRoutePoint != null)
        {
            neededDirections.Add(DirectionHelper.GetDirectionFromGridLocation(CurrentFerryRoutePoint.Tile.GridLocation, nextFerryRoutePoint.Tile.GridLocation));
        }
        if (previousFerryRoutePoint != null)
        {
            neededDirections.Add(DirectionHelper.GetDirectionFromGridLocation(CurrentFerryRoutePoint.Tile.GridLocation, previousFerryRoutePoint.Tile.GridLocation));
        }

        // remove directions that we do not want
        for (int i = _ferryDirectionIndicators.Count - 1; i >= 0; i--)
        {
            // remove any curreny ferry direction indicators that are not Needed Directions
            if (!neededDirections.Contains(_ferryDirectionIndicators[i].Direction))
            {
                FerryDirectionIndicatorPool.Instance.ReturnToPool(_ferryDirectionIndicators[i]);
                _ferryDirectionIndicators.Remove(_ferryDirectionIndicators[i]);
            }
        }

        // Add any Needed Directions to the ferry direction indicators that we do not yet have
        for (int j = 0; j < neededDirections.Count; j++)
        {
            if(!_ferryDirectionIndicators.Any(indicator => indicator.Direction == neededDirections[j]))
            {
                FerryDirectionIndicator ferryDirectionIndicator = FerryDirectionIndicatorPool.Instance.Get();
                ferryDirectionIndicator.Initialise(ControllingPlayerCharacter, neededDirections[j], CurrentFerryRoutePoint.Tile, this);
                ferryDirectionIndicator.transform.SetParent(MainScreenCameraCanvas.Instance.transform);
                ferryDirectionIndicator.gameObject.SetActive(true);

                _ferryDirectionIndicators.Add(ferryDirectionIndicator);
            }
        }
    }

    public void AddPlayerOnFerry(PlayerCharacter playerCharacter)
    {
        _playersOnFerry.Add(playerCharacter);
        Logger.Warning($"Added player {playerCharacter.Name} to ferry list");
    }

    public void RemovePlayerOnFerry(PlayerCharacter playerCharacter)
    {
        if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
        {
            if (playerCharacter.PhotonView.IsMine)
            {
                TryDestroyControlFerryButton();
            }
            for (int i = 0; i < _playersOnFerry.Count; i++)
            {
                if (_playersOnFerry[i] == playerCharacter)
                {
                    ToggleFerrySelectionIndicator(false, playerCharacter);
                }
                else
                {
                    Logger.Log($"Activate selection indicator for player {_playersOnFerry[i].Name}");
                    ToggleFerrySelectionIndicator(true, _playersOnFerry[i]);
                }
            }
        }

        if(GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
        {
            ToggleFerrySelectionIndicator(false, playerCharacter);
        }
        //Logger.Warning($"PlayerLocation {playerCharacter.Name} is {playerCharacter.CurrentGridLocation.X}, {playerCharacter.CurrentGridLocation.Y}. ferry location is {CurrentLocationTile.GridLocation.X}, {CurrentLocationTile.GridLocation.Y} ");

        _playersOnFerry.Remove(playerCharacter);
        Logger.Warning($"Removed player {playerCharacter.Name} from ferry");
    }
}
