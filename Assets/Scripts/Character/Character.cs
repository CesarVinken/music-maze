using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterBlueprint CharacterBlueprint;
    public GridLocation StartingPosition;
    public CharacterSpawnpoint Spawnpoint;

    public GridLocation CurrentGridLocation;
    public TargetLocation TargetGridLocation;

    public GameObject CharacterBody;
    protected ICharacter _characterType;

    [Space(10)]
    [Header("Rendering")]

    [SerializeField] protected int _sortingOrderBase = 500;
    protected float _sortingOrderCalculationOffset = .5f;
    protected float _sortingOrderTimer = 0;
    protected float _sortingOrderTimerLimit = .2f;

    [SerializeField] protected Renderer _bodyRenderer;

    [Space(10)]
    [Header("Locomotion")]

    [SerializeField] private float _baseSpeed = 8f;
    [SerializeField] private float _speed;
    [SerializeField] protected bool IsFrozen = false;
    [SerializeField] protected bool HasCalculatedTarget = false;
    [SerializeField] protected bool IsMoving = false;

    [Space(10)]
    [Header("Pathfinding")]

    public bool IsCalculatingPath = false;

    [SerializeField] protected CharacterAnimationHandler _animationHandler;
    protected Pathfinding _pathfinding;
    public List<PathNode> PathToTarget = new List<PathNode>();

    [Space(10)]
    [Header("Networking")]

    public PhotonView PhotonView;

    public virtual void Awake()
    {
        Guard.CheckIsNull(_bodyRenderer, "_bodyRenderer", gameObject);
        Guard.CheckIsNull(_animationHandler, "_animationHandler", gameObject);

        _speed = _baseSpeed;
        PathToTarget = new List<PathNode>();
    }

    public void LateUpdate()
    {
        _sortingOrderTimer -= Time.deltaTime;
        if(_sortingOrderTimer <= 0f)
        {
            _sortingOrderTimer = _sortingOrderTimerLimit;
            _bodyRenderer.sortingOrder = (int)(Math.Round(_sortingOrderBase - transform.position.y - _sortingOrderCalculationOffset) * 10);
        }
    }

    protected void SetCharacterType(ICharacter characterType)
    {
        _characterType = characterType;
        _animationHandler.SetAnimationControllerForCharacterType(_characterType);
    }

    public virtual void SetStartingPoint(Character character, GridLocation startingPosition, CharacterSpawnpoint spawnpoint)
    {
        character.Spawnpoint = spawnpoint;
        character.StartingPosition = startingPosition;
    }

    public void MoveCharacter()
    {
        PathNode nextNode = PathToTarget[0];
        GridLocation nextGridLocation = nextNode.Tile.GridLocation;
        GridLocation currentGridLocation = CurrentGridLocation;

        Vector3 moveDir;
        ObjectDirection direction = ObjectDirection.Right;
        if (nextGridLocation.X > currentGridLocation.X)
        {
            direction = ObjectDirection.Right;
            _animationHandler.SetDirection(direction);
        }
        else if (nextGridLocation.X < currentGridLocation.X)
        {
            direction = ObjectDirection.Left;
            _animationHandler.SetDirection(direction);
        }
        else if (nextGridLocation.Y > currentGridLocation.Y)
        {
            direction = ObjectDirection.Up;
            _animationHandler.SetDirection(direction);
        }
        else if (nextGridLocation.Y < currentGridLocation.Y)
        {
            direction = ObjectDirection.Down;
            _animationHandler.SetDirection(direction);
        }

        Vector2 targetVector2Pos = GridLocation.GridToVector(nextGridLocation);

        moveDir = (new Vector3(targetVector2Pos.x + GridLocation.OffsetToTileMiddle, targetVector2Pos.y + GridLocation.OffsetToTileMiddle, transform.position.z) - transform.position).normalized;
        float speed = 2.5f;
       
        transform.position = transform.position + moveDir * speed * Time.deltaTime;

        // Character reaches a tile grid location (its middle)
        if((direction == ObjectDirection.Right && transform.position.x > targetVector2Pos.x + GridLocation.OffsetToTileMiddle) ||
            (direction == ObjectDirection.Left && transform.position.x < targetVector2Pos.x + GridLocation.OffsetToTileMiddle) ||
            (direction == ObjectDirection.Down && transform.position.y < targetVector2Pos.y + GridLocation.OffsetToTileMiddle) ||
            direction == ObjectDirection.Up && transform.position.y > targetVector2Pos.y + GridLocation.OffsetToTileMiddle) 
        {
            CurrentGridLocation = nextGridLocation;
            PathToTarget.RemoveAt(0);
            //Logger.Log($"New Grid location{currentGridLocation.X}, {currentGridLocation.Y}. Remaining path length is {PathToTarget.Count}");
            if(PathToTarget.Count == 0)
            {
                OnTargetReached(); 
            }

            return;
        }
    }

    public virtual bool ValidateTarget(TargetLocation targetGridLocation)
    {
        return false;
    }

    // set character to current spawnpoint and reset pathfinder
    public void ResetCharacterPosition()
    {
        SetHasCalculatedTarget(false);
        _animationHandler.SetLocomotion(false);

        //Logger.Warning("Starting position for {0} is {1},{2}", gameObject.name, gameObject.GetComponent<PlayerCharacter>().StartingPosition.X, gameObject.GetComponent<PlayerCharacter>().StartingPosition.Y);
        GameManager.Instance.CharacterManager.PutCharacterOnGrid(gameObject, GridLocation.GridToVector(StartingPosition));
        CurrentGridLocation = StartingPosition;
    }

    public IEnumerator RespawnPlayerCharacter(PlayerCharacter character, float freezeTime)
    {
        int blackScreenNo = 0;
        if(GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer &&
        character.PlayerNumber == PlayerNumber.Player2)
        {
            blackScreenNo = 1;
        }
        BlackOutSquare blackOutSquare = MainScreenOverlayCanvas.Instance.BlackOutSquares[blackScreenNo];

        if(blackOutSquare == null)
        {
            Logger.Error($"Could not find blackout square for player {blackScreenNo + 1}");
        }

        character.FreezeCharacter();
        CharacterBody.SetActive(false); // TODO make character animation for appearing and disappearing of character, rather than turning the GO off and on

        // Screen to black
        IEnumerator toBlackCoroutine = blackOutSquare.ToBlack();

        StartCoroutine(toBlackCoroutine);
        while (blackOutSquare.BlackStatus == BlackStatus.InTransition)
        {
            yield return null;
        }
        ResetCharacterPosition();
        CharacterBody.SetActive(true);

        //Screen back to clear
        IEnumerator toClearCoroutine = blackOutSquare.ToClear();

        StartCoroutine(toClearCoroutine);
        while (blackOutSquare.BlackStatus == BlackStatus.InTransition)
        {
            yield return null;
        }
        character.UnfreezeCharacter();
    }

    public void SetHasCalculatedTarget(bool hasCalculatedTarget)
    {        
        if (hasCalculatedTarget)
        {
            IsMoving = true;
        }

        HasCalculatedTarget = hasCalculatedTarget;
    }

    public void FreezeCharacter()
    {
        IsFrozen = true;
    }

    public void UnfreezeCharacter()
    {
        IsFrozen = false;
    }

    public virtual void OnTargetReached()
    {
        PathToTarget.Clear();
        SetHasCalculatedTarget(false);
    }
}
