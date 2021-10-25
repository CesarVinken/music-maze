using System.Collections;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public const int SmokeExplosion1AnimationId = 1;
    public const int EmmonCaughtAnimationId = 2;

    public SpriteRenderer SpriteRenderer { get => _spriteRenderer; private set => _spriteRenderer = value; }

    public void Awake()
    {
        Guard.CheckIsNull(_animator, "_animator", gameObject);
        Guard.CheckIsNull(SpriteRenderer, "SpriteRenderer", gameObject);
        SetSortingOrder(SpriteSortingOrderRegister.EffectController);
    }

    public void PlayEffectLoop(AnimationEffect animationEffect)
    {
        switch (animationEffect)
        {
            case AnimationEffect.ExclamationMark:
                _animator.SetInteger("AnimationId", 2);
                break;
            case AnimationEffect.StartledSpinner:
                _animator.SetInteger("AnimationId", 1);
                break;
            default:
                Logger.Error($"Nothing implemented for the animation effect {animationEffect}");
                break;
        }
    }

    public void PlayEffect(AnimationEffect animationEffect)
    {
        switch (animationEffect)
        {
            case AnimationEffect.EmmonCaught:
                _animator.SetInteger("AnimationId", 2);
                StartCoroutine(DestroyAfterCoroutine(3f));
                break;
            case AnimationEffect.ExitOpenExplosion:
                _animator.SetInteger("AnimationId", 4);
                StartCoroutine(DestroyAfterCoroutine(3f));
                break;
            case AnimationEffect.FaeCaught:
                _animator.SetInteger("AnimationId", 3);
                StartCoroutine(DestroyAfterCoroutine(3f));
                break;
            case AnimationEffect.SmokeExplosion:
                _animator.SetInteger("AnimationId", 1);
                StartCoroutine(DestroyAfterCoroutine(3f));
                break;
            default:
                Logger.Error($"Nothing implemented for the animation effect {animationEffect}");
                break;
        }
    }

    private IEnumerator DestroyAfterCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    public void SetSortingOrder(int sortingOrder)
    {
        _spriteRenderer.sortingOrder = sortingOrder;
    }
}
