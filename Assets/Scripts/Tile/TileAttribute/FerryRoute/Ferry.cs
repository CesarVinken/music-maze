using Character;
using System.Collections.Generic;
using UnityEngine;

public class Ferry : MonoBehaviour
{
    public static List<Ferry> Ferries = new List<Ferry>();
        
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public MazePlayerCharacter ControllingPlayerCharacter { get; private set; }
    private FerryRoute _ferryRoute;

    public FerryDirection FerryDirection;
    public MazeTile CurrentLocationTile = null;

    public void Initialise(FerryRoute ferryRoute)
    {
        _ferryRoute = ferryRoute;

        GridLocation location = GridLocation.FindClosestGridTile(transform.position);
        SetNewCurrentLocation(location);
        Ferries.Add(this);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
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
        List<FerryRoutePoint> ferryRoutePoints = _ferryRoute.GetFerryRoutePoints();

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
