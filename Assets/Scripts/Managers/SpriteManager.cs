using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance;

    public Sprite[] DefaultDoor;
    public Sprite[] DefaultWall;

    public void Awake()
    {
        Instance = this;
    }
}
