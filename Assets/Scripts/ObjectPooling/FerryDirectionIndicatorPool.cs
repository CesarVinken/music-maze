using System.Collections.Generic;
using UnityEngine;

public class FerryDirectionIndicatorPool : MonoBehaviour
{
    public static FerryDirectionIndicatorPool Instance;

    public Queue<FerryDirectionIndicator> FerryDirectionIndicators = new Queue<FerryDirectionIndicator>();

    public FerryDirectionIndicator FerryDirectionIndicatorPrefab;

    public Sprite[] FerryDirectionIndicatorSprites;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(FerryDirectionIndicatorPrefab, "FerryDirectionIndicatorPrefab");
    }

    public FerryDirectionIndicator Get()
    {
        if (FerryDirectionIndicators.Count == 0)
        {
            AddFerryDirectionIndicator();
        }

        return FerryDirectionIndicators.Dequeue();
    }

    private void AddFerryDirectionIndicator()
    {
        FerryDirectionIndicator ferryDirectionIndicator = Instantiate(FerryDirectionIndicatorPrefab);
        FerryDirectionIndicators.Enqueue(ferryDirectionIndicator);
    }

    public void ReturnToPool(FerryDirectionIndicator ferryDirectionIndicator)
    {
        ferryDirectionIndicator.gameObject.SetActive(false);
        ferryDirectionIndicator.transform.SetParent(transform);
        ferryDirectionIndicator.Unset();
        //ferryDirectionIndicator.SetRendererAlpha(0);
        FerryDirectionIndicators.Enqueue(ferryDirectionIndicator);
    }
}
