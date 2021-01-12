using UnityEngine;

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

    public void Awake()
    {
        if (Animator == null)
            Animator = GetComponent<Animator>();
    }

    public void SetAnimationControllerForCharacterType(CharacterType characterType)
    {

        switch (characterType)
        {
            case CharacterType.Bard1:
                Animator.runtimeAnimatorController = CharacterManager.Instance.Bard1Controller;
                break;
            case CharacterType.Bard2:
                Animator.runtimeAnimatorController = CharacterManager.Instance.Bard2Controller;
                break;
            default:
                Logger.Error($"The CharacterType {characterType} is not yet implemented");
                break;
        }
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
