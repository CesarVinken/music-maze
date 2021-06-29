using System.Collections;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void Awake()
    {
        Guard.CheckIsNull(_animator, "_animator", gameObject);
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
