using Mirror;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : NetworkBehaviour
{
    public CharacterBlueprint CharacterBlueprint;
    public GridLocation StartingPosition;

    public ObjectDirection CharacterDirection = ObjectDirection.Down;
    public CharacterAnimationHandler AnimationHandler;

    public float BaseSpeed = 8f;
    public float Speed;

    public bool IsOnTile = true; // If a character is not on a tile, it means the character is in locomotion.
    public bool IsFrozen = false;

    public AIDestinationSetter DestinationSetter;
    public CharacterPath CharacterPath;
    public Vector2 Target
    {
        get
        {
            return new Vector2(TargetObject.transform.position.x, TargetObject.transform.position.y);
        }
    }

    private GameObject _targetObject;
    public GameObject TargetObject { get => _targetObject; set => _targetObject = value; }


    public void Awake()
    {
        if (AnimationHandler == null)
            Logger.Error(Logger.Initialisation, "Could not find AnimationHandler component on CharacterLocomotion");

        if (DestinationSetter == null)
            Logger.Error(Logger.Initialisation, "Could not find AIDestinationSetter component on CharacterLocomotion");

        if (CharacterPath == null)
            Logger.Error(Logger.Initialisation, "Could not find CharacterPath component on CharacterLocomotion");

        Speed = BaseSpeed;
    }


    public void SetStartingPosition(GridLocation gridLocation)
    {
        StartingPosition = gridLocation;
    }

    public void ResetCharacterPosition()
    {
        SetLocomotionTarget(GridLocation.GridToVector(StartingPosition));
        CharacterManager.Instance.PutCharacterOnGrid(gameObject, GridLocation.GridToVector(StartingPosition));
        ReachLocomotionTarget();
    }

    public void Update()
    {
        //if (CharacterPath.reachedEndOfPath)
        //{
        //    CharacterPath.enabled = false;
        //}

        //if (!IsOnTile)
        //{
        //    MoveCharacter();
        //}
    }

    public void MoveCharacter()
    {
        transform.position = CharacterPath.transform.position;
        float directionRotation = CharacterPath.rotation.eulerAngles.z;

        if (directionRotation == 0)
        {
            AnimationHandler.SetDirection(ObjectDirection.Up);
        }
        else if (directionRotation == 90)
        {
            AnimationHandler.SetDirection(ObjectDirection.Left);

        }
        else if (directionRotation == 180)
        {
            AnimationHandler.SetDirection(ObjectDirection.Down);

        }
        else if (directionRotation == 270)
        {
            AnimationHandler.SetDirection(ObjectDirection.Right);

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
        if (MazeLevelManager.Instance.Level.UnwalkableTiles.FirstOrDefault(tile => tile.GridLocation.X == targetGridLocation.X && tile.GridLocation.Y == targetGridLocation.Y))
        {
            Logger.Log(Logger.Locomotion, "target is unwalkable");
            return false;
        }

        // TODO this check is maybe not very efficient
        if (!MazeLevelManager.Instance.Level.Tiles.FirstOrDefault(tile => tile.GridLocation.X == targetGridLocation.X && tile.GridLocation.Y == targetGridLocation.Y))
        {
            //Logger.Log("target is not a tile");
            return false;
        }

        return true;
    }

    public void SetLocomotionTarget(Vector3 newTarget)
    {
        IsOnTile = false;

        if (TargetObject == null)
        {
            if (CharacterBlueprint == null) return;

            GameObject targetGO = new GameObject();
            targetGO.name = "Target object " + CharacterBlueprint.CharacterType;
            targetGO.transform.SetParent(GameManager.Instance.PathfindingSystemGO.transform);
            TargetObject = targetGO;
        }

        float offsetToTileMiddle = GridLocation.OffsetToTileMiddle;
        TargetObject.transform.position = new Vector3(newTarget.x + offsetToTileMiddle, newTarget.y + offsetToTileMiddle);

        DestinationSetter.target = TargetObject.transform;
    }

    public virtual void ReachLocomotionTarget() { }

    public IEnumerator FreezeCharacter(Character character, float freezeTime)
    {
        character.IsFrozen = true;

        yield return new WaitForSeconds(freezeTime);

        character.IsFrozen = false;
    }
}
