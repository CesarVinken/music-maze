using System.Collections.Generic;
using UnityEngine;

public class TileSpriteContainerPool : MonoBehaviour
{
    public static TileSpriteContainerPool Instance;

    public Queue<TileSpriteContainer> TileSpriteContainers = new Queue<TileSpriteContainer>();

    public TileSpriteContainer TileSpriteContainerPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(TileSpriteContainerPrefab, "TileSpriteContainerPrefab");
    }

    public TileSpriteContainer Get()
    {
        if(TileSpriteContainers.Count == 0)
        {
            AddTileSpriteContainer();
        }

        return TileSpriteContainers.Dequeue();
    }

    private void AddTileSpriteContainer()
    {
        TileSpriteContainer tileSpriteContainer = Instantiate(TileSpriteContainerPrefab);
        TileSpriteContainers.Enqueue(tileSpriteContainer);
    }

    public void ReturnToPool(TileSpriteContainer tileSpriteContainer)
    {
        tileSpriteContainer.gameObject.SetActive(false);
        tileSpriteContainer.transform.SetParent(transform);
        tileSpriteContainer.SetRendererAlpha(0);
        TileSpriteContainers.Enqueue(tileSpriteContainer);
    }
}
