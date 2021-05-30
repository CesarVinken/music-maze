using Pathfinding;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterBlueprint CharacterBlueprint;
    public GridLocation StartingPosition;
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

    [SerializeField] private Transform _characterPathTransform;
    [SerializeField] protected CharacterAnimationHandler _animationHandler;
    [SerializeField] protected CharacterPath _characterPath;   // change back to protected
    [SerializeField] protected Seeker _seeker;

    [Space(10)]
    [Header("Networking")]

    public PhotonView PhotonView;

    public virtual void Awake()
    {
        Guard.CheckIsNull(_bodyRenderer, "_bodyRenderer", gameObject);
        Guard.CheckIsNull(_animationHandler, "_animationHandler", gameObject);
        Guard.CheckIsNull(_characterPath, "_characterPath", gameObject);
        Guard.CheckIsNull(_seeker, "_seeker", gameObject);

        _speed = _baseSpeed;
        _characterPathTransform = _characterPath.transform;
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

    public void SetStartingPosition(Character character, GridLocation gridLocation)
    {
        character.StartingPosition = gridLocation;
    }

    protected Vector3 SetNewLocomotionTarget(Vector2 gridVectorTarget)
    {
        float offsetToTileMiddle = GridLocation.OffsetToTileMiddle;
        return new Vector3(gridVectorTarget.x + offsetToTileMiddle, gridVectorTarget.y + offsetToTileMiddle);
    }

    public void MoveCharacter()
    {
        transform.position = _characterPathTransform.position;
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

    public virtual bool ValidateTarget(GridLocation targetGridLocation)
    {
        return false;
    }

    // set character to current spawnpoint and reset pathfinder
    public void ResetCharacterPosition()
    {
        _characterPath.SetPath(null);
        SetHasCalculatedTarget(false);
        _animationHandler.SetLocomotion(false);

        Logger.Warning("Starting position for {0} is {1},{2}", gameObject.name, gameObject.GetComponent<PlayerCharacter>().StartingPosition.X, gameObject.GetComponent<PlayerCharacter>().StartingPosition.Y);
        GameManager.Instance.CharacterManager.PutCharacterOnGrid(gameObject, GridLocation.GridToVector(StartingPosition));
        _characterPathTransform.position = transform.position;
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

        //Screem back to clear
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
}
