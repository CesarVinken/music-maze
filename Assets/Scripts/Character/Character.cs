using CharacterType;
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

    public GridLocation CurrentGridLocation { get; private set; }

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
            SetCurrentGridLocation(nextGridLocation);
            PathToTarget.RemoveAt(0);
            //Logger.Log($"New Grid location{currentGridLocation.X}, {currentGridLocation.Y}. Remaining path length is {PathToTarget.Count}");
            if(PathToTarget.Count == 0)
            {

                OnTargetReached(); 
            }

            return;
        }
    }

    // set character to current spawnpoint and reset pathfinder
    public void ResetCharacterPosition()
    {
        SetHasCalculatedTarget(false);
        _animationHandler.SetIdle();

        //Logger.Warning("Starting position for {0} is {1},{2}", gameObject.name, gameObject.GetComponent<PlayerCharacter>().StartingPosition.X, gameObject.GetComponent<PlayerCharacter>().StartingPosition.Y);
        GameManager.Instance.CharacterManager.PutCharacterOnGrid(gameObject, GridLocation.GridToVector(StartingPosition));
        SetCurrentGridLocation(StartingPosition);
    }

    public void SetCurrentGridLocation(GridLocation newGridLocation)
    {
        CurrentGridLocation = newGridLocation;
        //Logger.Log($"Current gridlocation is now : {CurrentGridLocation.X}, {CurrentGridLocation.Y}");
    }

    protected void SetBodyAlpha(float alphaValue)
    {
        SpriteRenderer spriteRenderer = CharacterBody.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Logger.Error("could not find a sprite renderer on the character body");
        }

        Color newColor = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alphaValue);
        spriteRenderer.color = newColor;
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

    public ICharacter GetCharacterType()
    {
        return _characterType;
    }
}
