using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BlackStatus
{
    Black,
    Clear,
    InTransition
}

public class BlackOutSquare : MonoBehaviour
{
    [SerializeField] private Image _blackImage;
    [SerializeField] private float _fadeAmount;
    [SerializeField] private float _fadeSpeed = 5;

    public BlackStatus BlackStatus;


    public IEnumerator ToBlack()
    {
        BlackStatus = BlackStatus.InTransition;

        while (_blackImage.color.a < 1)
        {
            _fadeAmount = _blackImage.color.a + (_fadeSpeed * Time.deltaTime);
            AdjustAlpha(_fadeAmount);

            yield return null;
        }
        BlackStatus = BlackStatus.Black;
    }

    public IEnumerator ToClear()
    {
        BlackStatus = BlackStatus.InTransition;

        while (_blackImage.color.a > 0)
        {
            _fadeAmount = _blackImage.color.a - (_fadeSpeed * Time.deltaTime);
            AdjustAlpha(_fadeAmount);

            yield return null;
        }

        BlackStatus = BlackStatus.Clear;
    }

    private void AdjustAlpha(float fadeAmount)
    {
        _blackImage.color = new Color(_blackImage.color.r, _blackImage.color.g, _blackImage.color.b, fadeAmount);
    }

    public void ResetToDefault()
    {
        BlackStatus = BlackStatus.Clear;
        AdjustAlpha(0);
    }
}
