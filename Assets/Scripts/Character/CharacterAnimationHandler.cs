using UnityEngine;
using Photon.Pun;
using System.Collections;
using CharacterType;

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
            Logger.Log(Logger.Locomotion, "Set InLocomotion for animation to {0}", value);
            _inLocomotion = value;
            _animator.SetBool("Locomotion", _inLocomotion); // TODO? Use hashtable instead of string name for animations (= more efficient?)
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

    public void SetAnimationControllerForCharacterType(ICharacter characterType)
    {
        _animator.runtimeAnimatorController = characterType.GetAnimationController();

        //if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiPlayer)
        //{
        //    // valid for both player and enemy animators
        //    _photonAnimatorView.SetParameterSynchronized("Locomotion", PhotonAnimatorView.ParameterType.Bool, PhotonAnimatorView.SynchronizeType.Discrete);
        //    _photonAnimatorView.SetParameterSynchronized("Horizontal", PhotonAnimatorView.ParameterType.Float, PhotonAnimatorView.SynchronizeType.Discrete);
        //    _photonAnimatorView.SetParameterSynchronized("Vertical", PhotonAnimatorView.ParameterType.Float, PhotonAnimatorView.SynchronizeType.Discrete);
        //}
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

    public void SetDirection(ObjectDirection direction)
    {
        switch (direction)
        {
            case ObjectDirection.Down:
                SetHorizontal(0);
                SetVertical(-1f);
                break;
            case ObjectDirection.Left:
                SetHorizontal(-1f);
                SetVertical(0);
                break;
            case ObjectDirection.Right:
                SetHorizontal(1f);
                SetVertical(0);
                break;
            case ObjectDirection.Up:
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
}
