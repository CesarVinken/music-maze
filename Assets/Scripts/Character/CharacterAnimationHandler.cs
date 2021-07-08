using UnityEngine;
using Photon.Pun;

public class CharacterAnimationHandler : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private bool _inLocomotion = false;
    public bool InLocomotion
    {
        get { return _inLocomotion; }
        set
        {
            Logger.Log(Logger.Locomotion, "Set InLocomotion for animation to {0}", value);
            _inLocomotion = value;
            _animator.SetBool("Locomotion", _inLocomotion);
        }
    }

    [SerializeField] private PhotonAnimatorView _photonAnimatorView;

    public void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }

        Guard.CheckIsNull(_animator, "Animator", gameObject);

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

    public void SetLocomotion(bool value)
    {   
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
}
