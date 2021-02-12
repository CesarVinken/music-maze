using UnityEngine;

public class OverworldManager : MonoBehaviour
{
    public static OverworldManager Instance;

    public void Awake()
    {
        Instance = this;
    }
}
