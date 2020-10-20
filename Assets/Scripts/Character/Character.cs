using Pathfinding;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterBlueprint CharacterBlueprint;
    public GridLocation StartingPosition;

    public ObjectDirection CharacterDirection = ObjectDirection.Down;

    public float BaseSpeed = 8f;
    public float Speed;

    protected bool IsFrozen = false;
    protected bool HasCalculatedTarget = false;

    [SerializeField] protected CharacterAnimationHandler _animationHandler;
    [SerializeField] protected CharacterPath _characterPath;
    [SerializeField] protected Seeker _seeker;
    public PhotonView PhotonView;

    public GameObject CharacterBody;

    public void Awake()
    {
        if (_animationHandler == null)
            Logger.Error(Logger.Initialisation, "Could not find AnimationHandler component on Character");

        if (_characterPath == null)
            Logger.Error(Logger.Initialisation, "Could not find CharacterPath component on Character");

        if (_seeker == null)
            Logger.Error(Logger.Initialisation, "Could not find Seeker component on Character");

        Speed = BaseSpeed;
    }

    public void SetStartingPosition(Character character, GridLocation gridLocation)
    {
        character.StartingPosition = gridLocation;
    }

    // set character to current spawnpoint and reset pathfinder
    public void ResetCharacterPosition()
    {
        _characterPath.SetPath(null);
        SetHasCalculatedTarget(false);
        _animationHandler.SetLocomotion(false);

        Logger.Warning("Starting position for {0} is {1},{2}", gameObject.name, gameObject.GetComponent<PlayerCharacter>().StartingPosition.X, gameObject.GetComponent<PlayerCharacter>().StartingPosition.Y);
        CharacterManager.Instance.PutCharacterOnGrid(gameObject, GridLocation.GridToVector(StartingPosition));
        _characterPath.transform.position = transform.position;
    }

    protected Vector3 SetNewLocomotionTarget(Vector2 gridVectorTarget)
    {
        float offsetToTileMiddle = GridLocation.OffsetToTileMiddle;
        return new Vector3(gridVectorTarget.x + offsetToTileMiddle, gridVectorTarget.y + offsetToTileMiddle);
    }

    public void MoveCharacter()
    {
        transform.position = _characterPath.transform.position;
        float directionRotation = _characterPath.rotation.eulerAngles.z;

        if (directionRotation == 0)
        {
            _animationHandler.SetDirection(ObjectDirection.Up);
        }
        else if (directionRotation == 90)
        {
            _animationHandler.SetDirection(ObjectDirection.Left);
        }
        else if (directionRotation == 180)
        {
            _animationHandler.SetDirection(ObjectDirection.Down);
        }
        else if (directionRotation == 270)
        {
            _animationHandler.SetDirection(ObjectDirection.Right);
        }
        else
        {
            Logger.Warning("Unexpected movement direction {0}", directionRotation);
        }
    }

    public Tile GetRandomTileTarget()
    {
        List<Tile> walkableTiles = MazeLevelManager.Instance.Level.Tiles.Where(tile => tile.Walkable).ToList(); // to do keep central list in Tilemanager
        //TODO remove current tile from walkable tiles
        //TODO pick random
        int random = UnityEngine.Random.Range(0, walkableTiles.Count);
        //Logger.Warning("Picked random:: {0}", random);
        //Logger.Log("We want a random tile. We found: {0}, {1}", walkableTiles[random].GridLocation.X, walkableTiles[random].GridLocation.Y);
        return walkableTiles[random];
    }

    public bool ValidateTarget(GridLocation targetGridLocation)
    {
        if (MazeLevelManager.Instance.Level.TilesByLocation.ContainsKey(targetGridLocation))
        {
            Tile tile = MazeLevelManager.Instance.Level.TilesByLocation[targetGridLocation];

            if(tile.Walkable)
                return true;
        }
        return false;
    }

    public virtual void ReachLocomotionTarget() { }

    public IEnumerator RespawnCharacter(Character character, float freezeTime)
    {
        character.IsFrozen = true;
        CharacterBody.SetActive(false); // TODO make character animation for appearing and disappearing of character, rather than turning the GO off and on
        ResetCharacterPosition();

        yield return new WaitForSeconds(freezeTime / 2);
        CharacterBody.SetActive(true);

        yield return new WaitForSeconds(freezeTime /2 );

        character.IsFrozen = false;
    }

    public void SetHasCalculatedTarget(bool hasCalculatedTarget)
    {
        HasCalculatedTarget = hasCalculatedTarget;
    }

    public virtual void ReachTarget()
    {
        SetHasCalculatedTarget(false);
        _animationHandler.SetLocomotion(false);
    }
}
