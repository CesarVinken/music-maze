using UnityEngine;

public class TilesContainer : MonoBehaviour
{
    public static TilesContainer Instance;

    public void Awake()
    {
        Instance = this;
    }
}
