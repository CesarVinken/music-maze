using UnityEngine;
using Photon.Pun;

public class CharacterAnimationHandler : MonoBehaviour
{
    public Animator Animator;

    private bool _inLocomotion = false;
    public bool InLocomotion
    {
        get { return _inLocomotion; }
        set
        {
            Logger.Log(Logger.Locomotion, "Set InLocomotion for animation to {0}", value);
            _inLocomotion = value;
            Animator.SetBool("Locomotion", _inLocomotion);
        }
    }

    [SerializeField] private PhotonAnimatorView _photonAnimatorView;

    public void Awake()
    {
        if (Animator == null)
        {
            Animator = GetComponent<Animator>();
        }

        Guard.CheckIsNull(Animator, "Animator", gameObject);

        if (Animator == null)
            _photonAnimatorView = GetComponent<PhotonAnimatorView>();
    }

    public void SetAnimationControllerForCharacterType(ICharacter characterType)
    {
        Animator.runtimeAnimatorController = characterType.GetAnimationController();

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
        Animator.SetFloat("Horizontal", speed);
    }

    public void SetVertical(float speed)
    {
        Animator.SetFloat("Vertical", speed);
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
}
