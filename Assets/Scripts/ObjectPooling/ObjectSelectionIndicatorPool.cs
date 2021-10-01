using System.Collections.Generic;
using UnityEngine;

public class ObjectSelectionIndicatorPool : MonoBehaviour
{
    public static ObjectSelectionIndicatorPool Instance;
    public Queue<ObjectSelectionIndicator> ObjectSelectionIndicators = new Queue<ObjectSelectionIndicator>();

    public ObjectSelectionIndicator ObjectSelectionIndicatorPrefab;

    public Sprite FerrySelectionIndicatorSprite;

    public virtual void Awake()
    {
        Guard.CheckIsNull(FerrySelectionIndicatorSprite, "FerrySelectionIndicatorSprite", gameObject);

        Instance = this;
    }

    public ObjectSelectionIndicator Get()
    {
        if (ObjectSelectionIndicators.Count == 0)
        {
            AddObjectSelectionIndicator();
        }

        return ObjectSelectionIndicators.Dequeue();
    }

    private void AddObjectSelectionIndicator()
    {
        ObjectSelectionIndicator objectSelectionIndicator = Instantiate(ObjectSelectionIndicatorPrefab);
        ObjectSelectionIndicators.Enqueue(objectSelectionIndicator);
    }

    public void ReturnToPool(ObjectSelectionIndicator objectSelectionIndicator)
    {
        objectSelectionIndicator.gameObject.SetActive(false);
        objectSelectionIndicator.transform.SetParent(transform);
        objectSelectionIndicator.Unset();

        ObjectSelectionIndicators.Enqueue(objectSelectionIndicator);
    }
}
