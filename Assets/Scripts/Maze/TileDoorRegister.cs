using System.Collections.Generic;
using UnityEngine;

public class TileDoorRegister : MonoBehaviour
{
    // translate connectionScore to two ints, which are the spriteNumbers for the two parts of which every door consists. Either left+right part or up+down part.
    public static Dictionary<int, int[]> _closedDoorSpriteNumberRegister = new Dictionary<int, int[]>()
    {
        { 1, new [] { 1, 7} },
        { 2, new [] { 2, 7 } },
        { 3, new [] { 13, 20 } },
        { 4, new [] { 1, 8 } },
        { 5, new [] { 14, 19 } },
        { 6, new [] { -1 } },//
        { 7, new [] { 2, 8 } },
        { 8, new [] { -1 } },//
        { 9, new [] { -1 } },//
        { 10, new [] { 14, 20 } },
        { 11, new [] { -1 } },//
        { 12, new [] { -1 } },//
        { 13, new [] { -1 } },//
        { 14, new [] { -1 } },//
        { 15, new [] { -1 } },//
        { 16, new [] { -1 } },//
        { 17, new [] { 3, 7 } },
        { 18, new [] { 13, 21 } },
        { 19, new [] { 1, 9 } },
        { 20, new [] { 15, 19 } },
        { 21, new [] { -1 } },//
        { 22, new [] { 3, 9 } },
        { 23, new [] { -1 } },//
        { 24, new [] { 15, 21 } },
        { 25, new [] { -1 } },//
        { 26, new [] { -1 } },//
        { 27, new [] { 3, 8 } },
        { 28, new [] { 14, 21 } },
        { 29, new [] { 2, 9 } },
        { 30, new [] { 15, 20 } },
        { 31, new [] { -1 } },//
        { 32, new [] { -1 } },//
        { 33, new [] { -1 } },//
        { 34, new [] { -1 } } //
    };

}
