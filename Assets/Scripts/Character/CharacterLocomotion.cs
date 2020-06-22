using Pathfinding;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    public ObjectDirection CharacterDirection = ObjectDirection.Down;
    public CharacterBlueprint Character;
    public CharacterAnimationHandler AnimationHandler;
    public float BaseSpeed = 8f;
    public float Speed;
    public bool IsOnTile = true; // If a character is not on a tile, it means the character is in locomotion.
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
            Logger.Error(Logger.Initialisation, "Could not find AnimationHandler component on character");

        if (DestinationSetter == null)
            Logger.Error(Logger.Initialisation, "Could not find AIDestinationSetter component on CharacterLocomotion");

        if (CharacterPath == null)
            Logger.Error(Logger.Initialisation, "Could not find CharacterPath component on CharacterLocomotion");

        Speed = BaseSpeed;
    }

    public void Update()
    {
        if (CharacterPath.reachedEndOfPath)
        {
            CharacterPath.enabled = false;
        }

        if (!IsOnTile)
        {
            MoveCharacter();
        }
    }

    public void MoveCharacter()
    {
        transform.position = CharacterPath.position;
    }

    protected void TryStartCharacterMovement(ObjectDirection direction)
    {
        // check if character is in tile position, if so, start movement in direction.
        if (!IsOnTile)
        {
            // if already in locomotion, it means that we are between tiles and we are moving. Return.
            return;
        }

        CharacterPath.transform.position = transform.position;
        CharacterPath.transform.rotation = Quaternion.identity;
        CharacterPath.enabled = true;

        GridLocation currentGridLocation = GridLocation.VectorToGrid(transform.position);
        GridLocation targetGridLocation;

        //Order character to go to another tile

        switch (direction)
        {
            case ObjectDirection.Down:
                AnimationHandler.SetHorizontal(0);
                AnimationHandler.SetVertical(-1f);

                targetGridLocation = new GridLocation(currentGridLocation.X, currentGridLocation.Y - 1);

                if (!ValidateTarget(targetGridLocation)) return;

                SetLocomotionTarget(GridLocation.GridToVector(targetGridLocation));
                break;
            case ObjectDirection.Left:
                AnimationHandler.SetHorizontal(-1f);
                AnimationHandler.SetVertical(0);

                targetGridLocation = new GridLocation(currentGridLocation.X - 1, currentGridLocation.Y);
                if (!ValidateTarget(targetGridLocation)) return;

                SetLocomotionTarget(GridLocation.GridToVector(targetGridLocation));
                break;
            case ObjectDirection.Right:
                AnimationHandler.SetHorizontal(1f);
                AnimationHandler.SetVertical(0);
                targetGridLocation = new GridLocation(currentGridLocation.X + 1, currentGridLocation.Y);
                if (!ValidateTarget(targetGridLocation)) return;

                SetLocomotionTarget(GridLocation.GridToVector(targetGridLocation));
                break;
            case ObjectDirection.Up:
                AnimationHandler.SetHorizontal(0);
                AnimationHandler.SetVertical(1f);
                Logger.Log("currently {0}, {1}", currentGridLocation.X, currentGridLocation.Y);
                targetGridLocation = new GridLocation(currentGridLocation.X, currentGridLocation.Y + 1);
                Logger.Log("future {0}, {1}", targetGridLocation.X, targetGridLocation.Y);

                if (!ValidateTarget(targetGridLocation)) return;

                SetLocomotionTarget(GridLocation.GridToVector(targetGridLocation));
                break;
            default:
                Logger.Warning("Unhandled locomotion direction {0}", direction);
                break;
        }

        if (!AnimationHandler.InLocomotion)
            AnimationHandler.SetLocomotion(true);
    }

    private bool ValidateTarget(GridLocation targetGridLocation)
    {
        if (MazeLevelManager.Instance.Level.UnwalkableTiles.FirstOrDefault(tile => tile.GridLocation.X == targetGridLocation.X && tile.GridLocation.Y == targetGridLocation.Y))
        {
            Logger.Log(Logger.Locomotion, "target is unwalkable");
            return false;
        }

        // TODO this check is maybe not very efficient
        if (!MazeLevelManager.Instance.Level.Tiles.FirstOrDefault(tile => tile.GridLocation.X == targetGridLocation.X && tile.GridLocation.Y == targetGridLocation.Y))
        {
            Logger.Log("target is not a tile");
            return false;
        }

        return true;
    }

    public void SetLocomotionTarget(Vector3 newTarget)
    {
        IsOnTile = false;

        if (TargetObject == null)
        {
            GameObject targetGO = new GameObject();
            //targetGO.name = "Target object " + Character.FullName();
            targetGO.transform.SetParent(GameManager.Instance.AstarGO.transform);
            TargetObject = targetGO;
        }

        float offsetToTileMiddle = GridLocation.OffsetToTileMiddle;
        TargetObject.transform.position = new Vector3(newTarget.x + offsetToTileMiddle, newTarget.y + offsetToTileMiddle);

        DestinationSetter.target = TargetObject.transform;
    }

    public void ReachLocomotionTarget()
    {
        IsOnTile = true;
        Vector3 roundedVectorPosition = new Vector3((float)Math.Round(transform.position.x - GridLocation.OffsetToTileMiddle), (float)Math.Round(transform.position.y - GridLocation.OffsetToTileMiddle));
        transform.position = new Vector3(roundedVectorPosition.x + GridLocation.OffsetToTileMiddle, roundedVectorPosition.y + GridLocation.OffsetToTileMiddle, 0);

        AnimationHandler.SetLocomotion(false);
    }
}
