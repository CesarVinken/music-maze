using System.Collections;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public SpriteRenderer SpriteRenderer { get => _spriteRenderer; private set => _spriteRenderer = value; }

    public void Awake()
    {
        Guard.CheckIsNull(_animator, "_animator", gameObject);
        Guard.CheckIsNull(SpriteRenderer, "SpriteRenderer", gameObject);
        _spriteRenderer.sortingOrder = SpriteSortingOrderRegister.EffectController;
    }

    public void PlayEffectLoop(AnimationEffect animationEffect)
    {
        switch (animationEffect)
        {
            case AnimationEffect.StartledSpinner:
                // TODO: Once we have more than 1 animation effect, create a system to pick the correct animation.
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
            case AnimationEffect.SmokeExplosion:
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
}
