﻿using UnityEngine;
using Photon.Pun;
using System.Collections;
using Character.CharacterType;

namespace Character
{
    public class CharacterAnimationHandler : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Character _character;

        private bool _inLocomotion = false;
        public bool InLocomotion
        {
            get { return _inLocomotion; }
            set
            {
                _inLocomotion = value;
                _animator.SetBool("Locomotion", _inLocomotion); // TODO? Use hashtable instead of string name for animations (= more efficient?)
                if (_isControllingFerry)
                {
                    _animator.speed = _inLocomotion ? 1 : 0; // if we are controlling the ferry but stop moving, we show the paused ferry move animation
                }
            }
        }

        private bool _isControllingFerry = false;
        public bool IsControllingFerry
        {
            get { return _isControllingFerry; }
            set
            {
                _isControllingFerry = value;
                _animator.SetBool("ControllingFerry", _isControllingFerry); // TODO? Use hashtable instead of string name for animations (= more efficient?)
                if(!_isControllingFerry)
                {
                    _animator.speed = 1;
                }
            }
        }

        [SerializeField] private PhotonAnimatorView _photonAnimatorView;

        private float timeInIdle = 0f;
        private IEnumerator _idleRoutine = null;

        public void Awake()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }

            Guard.CheckIsNull(_animator, "Animator", gameObject);
            Guard.CheckIsNull(_character, "Character", gameObject);

            if (_animator == null)
                _photonAnimatorView = GetComponent<PhotonAnimatorView>();
        }

        public void Update()
        {
            // The client does not keep track of the bools on the script. So when the other player initiates an animatin, we can only get the status from the animator itself. We need this to make sure the ferry animation stops and continues correctly
            // We are calling GetBool 2 times every update. Maybe to inefficient
            if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && !_character.PhotonView.IsMine)
            {
                bool isControllingFerryOnAnimator = _animator.GetBool("ControllingFerry");
                if (isControllingFerryOnAnimator)
                {
                    if (_animator.GetBool("Locomotion"))
                    {
                        _animator.speed = 1;
                    }
                    else
                    {
                        _animator.speed = 0;
                    }
                }
            }
        }

        public void SetAnimationControllerForCharacterType(ICharacter characterType)
        {
            _animator.runtimeAnimatorController = characterType.GetAnimationController();
        }

        public void SetHorizontal(float speed)
        {
            _animator.SetFloat("Horizontal", speed);
        }

        public void SetVertical(float speed)
        {
            _animator.SetFloat("Vertical", speed);
        }

        public void SetIdle()
        {
            SetLocomotion(false);

            if(_idleRoutine != null)
            {
                StopCoroutine(_idleRoutine);
            }

            // TODO: Remove. Temporary as Fae does not have a IdleLong animation yet.
            if(!(_character.GetCharacterType() is Emmon))
            {
                return;
            }

            _idleRoutine = IdleRoutine();
            StartCoroutine(_idleRoutine);
        }

        public void SetLocomotion(bool value)
        {
            if (value)
            {
                if (_idleRoutine != null)
                {
                    StopCoroutine(_idleRoutine);
                }
            }

            InLocomotion = value;
        }

        public void SetDirectionOnFerry(Ferry ferry, Direction direction)
        {
            // If we are controlling the ferry, we do not want to turn in a different direction than the direction of the ferry
            if (!DirectionHelper.IsAlligningWithFerry(ferry.FerryRoute.FerryRouteDirection, direction))
            {
                return;
            }
            SetDirection(direction);
        }

        public void SetDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    SetHorizontal(0);
                    SetVertical(-1f);
                    break;
                case Direction.Left:
                    SetHorizontal(-1f);
                    SetVertical(0);
                    break;
                case Direction.Right:
                    SetHorizontal(1f);
                    SetVertical(0);
                    break;
                case Direction.Up:
                    SetHorizontal(0);
                    SetVertical(1f);
                    break;
                default:
                    Logger.Warning("Unhandled locomotion direction {0}", direction);
                    break;
            }
        }

        public void TriggerSpawning()
        {
            _animator.SetTrigger("Spawn");
        }

        // every x amount of seconds there is a chance to play an idle animation variation.
        public IEnumerator IdleRoutine()
        {
            bool isIdle = true;

            while (true)
            {
                yield return new WaitForSeconds(4);
                isIdle = _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
                //Logger.Log($"Do another check for idle variation. Are we idle? {isIdle}");

                if (isIdle)
                {
                    // 25% chance to play an idle variation
                    int randomOutOfFour = UnityEngine.Random.Range(1, 5);
                    if(randomOutOfFour == 1)
                    {
                        _animator.SetTrigger("Variation");
                    }
                }
                else
                {
                    yield return null;
                }
            }   
        }

        public void SetAnimationSpeed(float speed)
        {
            _animator.speed = speed;
        }
    }
}