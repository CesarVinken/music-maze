using UnityEngine;
using Photon.Pun;
using System.Collections;
using Character.CharacterType;
using Console;
using PlayerCamera;
using System.Collections.Generic;
using UI;
using System;

namespace Character
{
    public struct TargetLocation
    {
        public GridLocation TargetGridLocation;
        public Direction TargetDirection;

        public TargetLocation(GridLocation targetGridLocation, Direction targetDirection)
        {
            TargetGridLocation = targetGridLocation;
            TargetDirection = targetDirection;
        }
    }

    public class PlayerCharacter : Character
    {
        [Space(10)]

        public KeyboardInput KeyboardInput = KeyboardInput.None;
        public PlayerNumber PlayerNumber = PlayerNumber.Player1;
        public Tile LastTile;
        public string Name;
        protected TargetLocation TargetGridLocation;
        [SerializeField] protected BoxCollider2D _playerCollider;

        protected bool _isPressingPointerForSeconds = false;
        protected float _pointerPresserTimer = 1;
        protected const float _pointerPresserDelay = 0.25f;

        public static event Action<PlayerCharacter> NewPlayerGridLocationEvent;

        public Ferry ControllingFerry = null;
        public List<MapInteractionButton> MapInteractionButtonsForPlayer = new List<MapInteractionButton>();

        public override void Awake()
        {
            Guard.CheckIsNull(_playerCollider, "_playerCollider", gameObject);

            SetPlayerNumber();

            GameManager.Instance.CharacterManager.AddPlayer(PlayerNumber, this); // do here and not in manager.

            // TODO: character type should not depend on Player Number, but on which character the player chose when starting the game
            AssignCharacterType(); // relies on player number
            SetPlayerName(); // relies on player type
            SetPlayerKeyboardInput();

            base.Awake();

            _pointerPresserTimer = 0;
        }

        public virtual void Start()
        {
            _pathfinding = new Pathfinding(this);
        }

        public virtual void Update()
        {
            if (EditorManager.InEditor) return;

            if (IsFrozen) return;
            if (Console.Console.Instance && Console.Console.Instance.ConsoleState != ConsoleState.Closed)
                return;

            if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
                (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && PhotonView.IsMine)
                )
            {
                if (PersistentGameManager.CurrentPlatform == Platform.PC)
                    HandleKeyboardInput();

                if (Input.GetMouseButtonUp(0))
                {
                    _isPressingPointerForSeconds = false;
                    _pointerPresserTimer = 0;

                    if (!IsPressingMovementKey() && !HasCalculatedTarget)
                    {
                        _animationHandler.SetIdle();
                        IsMoving = false;
                    }
                }

                if (Input.GetMouseButton(0))
                {
                    if (!HasCalculatedTarget)
                    {
                        _animationHandler.SetIdle();
                        IsMoving = false;
                    }
                }

                if ((PersistentGameManager.CurrentPlatform == Platform.PC && Input.GetMouseButtonDown(0) && _pointerPresserTimer == 0) ||
                (PersistentGameManager.CurrentPlatform == Platform.Android && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began && _pointerPresserTimer == 0))
                {
                    if (CameraController.CurrentZoom != ZoomAction.PlayerZoom)
                    {
                        StartCoroutine(RunPointerPresserTimer());
                    }
                }

                if (_isPressingPointerForSeconds) //only check for the click/tap after we are pressing for x seconds
                {
                    if (!Input.GetMouseButton(0))
                    {
                        _isPressingPointerForSeconds = false;
                        _pointerPresserTimer = 0;

                        if (!IsPressingMovementKey())
                        {
                            _animationHandler.SetIdle();
                        }
                        IsMoving = false;
                    }
                    else if (CameraController.CurrentZoom != ZoomAction.PlayerZoom)
                    {
                        CheckPointerInput();
                    }
                }

                if (HasCalculatedTarget)
                {
                    MoveCharacter();
                }
            }
            else if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
            {
                if (PersistentGameManager.CurrentPlatform == Platform.PC)
                    HandleKeyboardInput();

                if (HasCalculatedTarget)
                {
                    MoveCharacter();
                }
            }
            else // Case: Clients in multiplayer network game
            {
                if (HasCalculatedTarget)
                {
                    MoveCharacter();
                }
            }

            if (PathToTarget.Count == 0 && IsMoving)
            {
                Logger.Log("player reached target");
                OnTargetReached();
            }
        }

        public void AssignCharacterType()
        {
            switch (PersistentGameManager.PlayerCharacterNames[PlayerNumber])
            {
                case "Emmon":
                    SetCharacterType(new Emmon());
                    break;
                case "Fae":
                    SetCharacterType(new Fae());
                    break;
                default:
                    break;
            }
        }

        public override void SetCurrentGridLocation(GridLocation newGridLocation)
        {
            CurrentGridLocation = newGridLocation;
            NewPlayerGridLocationEvent?.Invoke(this);
        }

        private AnimationEffect GetAnimationEffectForPlayerChaught()
        {
            if (_characterType is Emmon)
            {
                return AnimationEffect.EmmonCaught;
            }
            else if (_characterType is Fae)
            {
                return AnimationEffect.FaeCaught;
            }
            Logger.Error($"Could not find animation for characterType {_characterType}");
            return AnimationEffect.EmmonCaught;
        }

        protected IEnumerator RespawnPlayerCharacter(PlayerCharacter character)
        {
            // play caught animation
            AnimationEffect animationEffect = GetAnimationEffectForPlayerChaught();

            GameObject emmonCaughtPrefab = MazeLevelGameplayManager.Instance.GetEffectAnimationPrefab(animationEffect);
            GameObject emmonCaughtPGO = GameObject.Instantiate(emmonCaughtPrefab, SceneObjectManager.Instance.transform);
            Vector3 emmonCaughtSpawnPosition = character.transform.position;
            emmonCaughtPGO.transform.position = emmonCaughtSpawnPosition;

            EffectController emmonCaughtPGOEffectController = emmonCaughtPGO.GetComponent<EffectController>();

            emmonCaughtPGOEffectController.PlayEffect(animationEffect);

            character.FreezeCharacter();
            CharacterBody.SetActive(false);
            SetBodyAlpha(0); // make body transparent before respawning

            ResetCharacterPosition();

            float waitTime = 1.4f;
            yield return new WaitForSeconds(waitTime);

            _animationHandler.TriggerSpawning();
            CharacterBody.SetActive(true);

            float spawnAnimationLength = 0.5f;
            yield return new WaitForSeconds(spawnAnimationLength);

            character.UnfreezeCharacter();
        }

        private IEnumerator RunPointerPresserTimer()
        {
            while (_pointerPresserTimer < _pointerPresserDelay)
            {
                yield return null;
                _pointerPresserTimer += Time.deltaTime;
            }

            _isPressingPointerForSeconds = true;
            _pointerPresserTimer = 0;
        }

        private void CheckPointerInput()
        {
            if (HasCalculatedTarget) return;
            // Logger.Log($"Touch count {Input.touchCount}");

            if (PersistentGameManager.CurrentPlatform == Platform.Android && Input.touchCount != 1) return;

            Vector2 tempFingerPosition = GetPointerPosition();
            GridLocation closestGridLocation = GridLocation.FindClosestGridTile(tempFingerPosition);

            if (closestGridLocation.X == CurrentGridLocation.X && closestGridLocation.Y == CurrentGridLocation.Y) return;

            GridLocation newLocomotionTarget = CurrentGridLocation;
            Vector2 direction = tempFingerPosition - (Vector2)transform.position;
            float angle = Vector2.SignedAngle(Vector2.up, direction) * -1;

            new GridLocation(CurrentGridLocation.X, CurrentGridLocation.Y);
            Direction moveDirection = Direction.Right;

            if (angle <= -135)  // go down
            {
                newLocomotionTarget = new GridLocation(CurrentGridLocation.X, CurrentGridLocation.Y - 1);
                moveDirection = Direction.Down;
            }
            else if (angle <= -45) // go left
            {
                newLocomotionTarget = new GridLocation(CurrentGridLocation.X - 1, CurrentGridLocation.Y);
                moveDirection = Direction.Left;
            }
            else if (angle <= 45) // go up
            {
                newLocomotionTarget = new GridLocation(CurrentGridLocation.X, CurrentGridLocation.Y + 1);
                moveDirection = Direction.Up;
            }
            else if (angle <= 135) // go right
            {
                newLocomotionTarget = new GridLocation(CurrentGridLocation.X + 1, CurrentGridLocation.Y);
                moveDirection = Direction.Right;
            }
            else // go down
            {
                newLocomotionTarget = new GridLocation(CurrentGridLocation.X, CurrentGridLocation.Y - 1);
                moveDirection = Direction.Down;
            }

            SetPointerLocomotionTarget(GridLocation.GridToVector(newLocomotionTarget), moveDirection);
        }

        private Vector2 GetPointerPosition()
        {
            if (PersistentGameManager.CurrentPlatform == Platform.Android)
            {
                return Camera.main.ScreenToWorldPoint(Input.touches[0].position);
            }
            else
            {
                return Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        private void SetPointerLocomotionTarget(Vector2 target, Direction moveDirection)
        {
            //Logger.Warning($"SetPointerLocomotionTarget to target {target.x}, {target.y} in the direction {moveDirection}");
            GridLocation targetGridLocation = GridLocation.FindClosestGridTile(target);

            if (!ValidateTarget(new TargetLocation(targetGridLocation, moveDirection))) return;

            Vector2 gridVectorTarget = GridLocation.GridToVector(targetGridLocation);

            if (CurrentGridLocation.X == targetGridLocation.X && CurrentGridLocation.Y == targetGridLocation.Y) return;

            if (!_animationHandler.InLocomotion)
                _animationHandler.SetLocomotion(true);

            IsCalculatingPath = true;

            PathToTarget = _pathfinding.FindNodePath(CurrentGridLocation, targetGridLocation);

            PathToTarget.RemoveAt(0);

            IsCalculatingPath = false;
            SetHasCalculatedTarget(true);
        }

        private void HandleKeyboardInput()
        {
            if (KeyboardInput == KeyboardInput.Player1)
            {
                if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Up))
                {
                    TryStartCharacterMovement(Direction.Up);
                }
                else if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Right))
                {
                    TryStartCharacterMovement(Direction.Right);
                }
                else if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Down))
                {
                    TryStartCharacterMovement(Direction.Down);
                }
                else if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Left))
                {
                    TryStartCharacterMovement(Direction.Left);
                }
            }
            else if (KeyboardInput == KeyboardInput.Player2)
            {
                if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Up))
                {
                    TryStartCharacterMovement(Direction.Up);
                }
                else if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Right))
                {
                    TryStartCharacterMovement(Direction.Right);
                }
                else if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Down))
                {
                    TryStartCharacterMovement(Direction.Down);
                }
                else if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Left))
                {
                    TryStartCharacterMovement(Direction.Left);
                }
            }
            else
            {

            }
        }

        public void TryStartCharacterMovement(Direction direction)
        {
            // check if character is in tile position, if so, start movement in direction.
            if (HasCalculatedTarget)
            {
                // if already in locomotion, it means that we are between tiles and we are moving. Return.
                return;
            }

            if (IsCalculatingPath) return;

            GridLocation currentGridLocation = GridLocation.VectorToGrid(transform.position);

            if(ControllingFerry != null)
            {
                _animationHandler.SetDirectionOnFerry(ControllingFerry, direction);
            }
            else
            {
                _animationHandler.SetDirection(direction);
            }

            switch (direction)
            {
                case Direction.Down:
                    TargetGridLocation = new TargetLocation(new GridLocation(currentGridLocation.X, currentGridLocation.Y - 1), direction);
                    break;
                case Direction.Left:
                    TargetGridLocation = new TargetLocation(new GridLocation(currentGridLocation.X - 1, currentGridLocation.Y), direction);
                    break;
                case Direction.Right:
                    TargetGridLocation = new TargetLocation(new GridLocation(currentGridLocation.X + 1, currentGridLocation.Y), direction);
                    break;
                case Direction.Up:
                    TargetGridLocation = new TargetLocation(new GridLocation(currentGridLocation.X, currentGridLocation.Y + 1), direction);
                    break;
                default:
                    Logger.Warning("Unhandled locomotion direction {0}", direction);
                    return;
            }

            if (!ValidateTarget(TargetGridLocation))
            {
                // This prevents the character from displaying locomotion animation when walking into an unwalkable tile
                _animationHandler.SetIdle();
                return;
            }

            IsCalculatingPath = true;
            //Logger.Log($"TryStartCharacterMovement. {CurrentGridLocation.X},{CurrentGridLocation.Y} to {TargetGridLocation.TargetGridLocation.X}, {TargetGridLocation.TargetGridLocation.Y}");
            PathToTarget = _pathfinding.FindNodePath(CurrentGridLocation, TargetGridLocation.TargetGridLocation);
            PathToTarget.RemoveAt(0);

            IsCalculatingPath = false;
            SetHasCalculatedTarget(true);

            if (!_animationHandler.InLocomotion)
            {
                _animationHandler.SetLocomotion(true);
            }
        }

        private bool IsPressingMovementKey()
        {
            if (KeyboardInput == KeyboardInput.Player1)
            {
                if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Up)) return true;
                if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Right)) return true;
                if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Down)) return true;
                if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player1Left)) return true;
            }
            if (KeyboardInput == KeyboardInput.Player2)
            {
                if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Up)) return true;
                if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Right)) return true;
                if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Down)) return true;
                if (Input.GetKey(GameManager.Instance.KeyboardConfiguration.Player2Left)) return true;
            }

            return false;
        }

        public override void OnTargetReached()
        {
            //Logger.Warning("Target reached.");
            SetHasCalculatedTarget(false);

            if (!IsPressingMovementKey() && !_isPressingPointerForSeconds)
            {
                _animationHandler.SetIdle();
            }
            IsMoving = false;
        }

        // The player number needs to be set from the player character, and not from the manager.
        // The reason is that in a network game, the character that is spawned by the other client will not be spawned here through the manager. However, it will go through the player character's Awake/Start function.
        protected void SetPlayerNumber()
        {
            ICharacterManager characterManager = GameManager.Instance.CharacterManager;

            if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (PhotonView.IsMine)
                        PlayerNumber = PlayerNumber.Player1;
                    else
                        PlayerNumber = PlayerNumber.Player2;
                }
                else
                {
                    if (PhotonView.IsMine)
                        PlayerNumber = PlayerNumber.Player2;
                    else
                        PlayerNumber = PlayerNumber.Player1;
                }
            }
            else if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
            {
                PlayerNumber = PlayerNumber.Player1;
            }
            else
            {
                if (GameManager.Instance.CharacterManager.GetPlayerCharacter<PlayerCharacter>(PlayerNumber.Player1) == null)
                {
                    PlayerNumber = PlayerNumber.Player1;
                }
                else
                {
                    PlayerNumber = PlayerNumber.Player2;
                }
            }
            Logger.Log(Logger.Initialisation, $"We set the player number to {PlayerNumber}");
        }

        // Should be here and not in manager.
        private void SetPlayerName()
        {
            ICharacterManager characterManager = GameManager.Instance.CharacterManager;

            if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
            {
                Name = PhotonView.Owner == null ? "Player 1" : PhotonView.Owner?.NickName;
            }
            else if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
            {
                Name = _characterType.ToString().Split('.')[2];
            }
            else // split screen
            {
                int playerCount = characterManager.GetPlayerCount();
                Logger.Log($"playerCount is {playerCount}");
                if (PlayerNumber == PlayerNumber.Player1)
                {
                    Name = "Player 1";
                }
                else if (PlayerNumber == PlayerNumber.Player2)
                {
                    Name = "Player 2";
                }
                else
                {
                    Logger.Error($"Unexpected number of players: {playerCount}");
                }
            }

            gameObject.name = Name;
            Logger.Warning($"Set name of {PlayerNumber} character to {Name}");
        }

        public virtual bool ValidateTarget(TargetLocation targetLocation)
        {
            return true;
        }

        public void ToggleFerryControl(Ferry ferry)
        {
            ferry.TryDestroyControlFerryButton();

            ControllingFerry = ControllingFerry ? null : ferry;
            
            if (ControllingFerry)
            {
                if (IsMoving) return; // we should never be able to set a controlling player when we are already moving. For example would happen if a player clicks immediately after leaving the ferry.

                ferry.SetControllingPlayerCharacter(this);
            }
            else
            {
                ferry.SetControllingPlayerCharacter(null);
            }
            _animationHandler.IsControllingFerry = ControllingFerry;

            // Send update to other players to update ferry & player status
            if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
            {
                PlayerControlsFerryEvent playerControlsFerryEvent = new PlayerControlsFerryEvent();
                playerControlsFerryEvent.SendPlayerControlsFerryEvent(CurrentGridLocation, this, ControllingFerry);
            }
        }

        // The non-triggering client receives event. Does not need any checks, just update the player character status that was determined by the other player
        public void ToggleFerryControlOnOthers(Ferry ferry, bool isControlling)
        {
            ferry.TryDestroyControlFerryButton();
            ControllingFerry = ferry ? ferry : null;

            ferry.SetControllingPlayerCharacter(this);

            _animationHandler.IsControllingFerry = isControlling;
            Logger.Log($"{Name} is now controlling the ferry. Set animation for isControlling to {isControlling == true}");
        }

        private void SetPlayerKeyboardInput()
        {
            if (PersistentGameManager.CurrentPlatform == Platform.PC)
            {
                if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
                {
                    KeyboardInput = KeyboardInput.Player1;
                }
                else if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
                {
                    KeyboardInput = KeyboardInput.Player1;
                }
                else if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
                {

                    if (PlayerNumber == PlayerNumber.Player1)
                    {
                        KeyboardInput = KeyboardInput.Player1;
                    }
                    else if (PlayerNumber == PlayerNumber.Player2)
                    {
                        KeyboardInput = KeyboardInput.Player2;
                    }
                }
            }
        }
    }
}
