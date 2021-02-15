
using UnityEngine;

public static class TileConnectionRegister
{
    public static TileConnectionScoreInfo CalculateTileConnectionScore<T>(TileModifierConnectionInfo<T> right, TileModifierConnectionInfo<T> down, TileModifierConnectionInfo<T> left, TileModifierConnectionInfo<T> up) where T : MonoBehaviour, ITileConnectable
    {
        if (right.HasConnection)
        {
            // narrow
            if (right.TileModifier.ConnectionScore == 1 || right.TileModifier.ConnectionScore == 2 || right.TileModifier.ConnectionScore == 3 || right.TileModifier.ConnectionScore == 4 || right.TileModifier.ConnectionScore == 5 || right.TileModifier.ConnectionScore == 6 || right.TileModifier.ConnectionScore == 7 || right.TileModifier.ConnectionScore == 8 || right.TileModifier.ConnectionScore == 9 || right.TileModifier.ConnectionScore == 10 || right.TileModifier.ConnectionScore == 11 || right.TileModifier.ConnectionScore == 12 || right.TileModifier.ConnectionScore == 14 || right.TileModifier.ConnectionScore == 15 || right.TileModifier.ConnectionScore == 17 || right.TileModifier.ConnectionScore == 18 || right.TileModifier.ConnectionScore == 20 || right.TileModifier.ConnectionScore == 27)
            {
                if (down.HasConnection)
                {
                    // narrow
                    if (down.TileModifier.ConnectionScore == 1 || down.TileModifier.ConnectionScore == 2 || down.TileModifier.ConnectionScore == 3 || down.TileModifier.ConnectionScore == 4 || down.TileModifier.ConnectionScore == 5 || down.TileModifier.ConnectionScore == 6 || down.TileModifier.ConnectionScore == 7 || down.TileModifier.ConnectionScore == 8 || down.TileModifier.ConnectionScore == 9 || down.TileModifier.ConnectionScore == 10 || down.TileModifier.ConnectionScore == 11 || down.TileModifier.ConnectionScore == 13 || down.TileModifier.ConnectionScore == 14 || down.TileModifier.ConnectionScore == 15 || down.TileModifier.ConnectionScore == 17 || down.TileModifier.ConnectionScore == 19 || down.TileModifier.ConnectionScore == 20 || down.TileModifier.ConnectionScore == 28)
                    {
                        if (left.HasConnection)
                        {
                            // narrow
                            if (left.TileModifier.ConnectionScore == 1 || left.TileModifier.ConnectionScore == 2 || left.TileModifier.ConnectionScore == 3 || left.TileModifier.ConnectionScore == 4 || left.TileModifier.ConnectionScore == 5 || left.TileModifier.ConnectionScore == 6 || left.TileModifier.ConnectionScore == 7 || left.TileModifier.ConnectionScore == 8 || left.TileModifier.ConnectionScore == 9 || left.TileModifier.ConnectionScore == 10 || left.TileModifier.ConnectionScore == 11 || left.TileModifier.ConnectionScore == 12 || left.TileModifier.ConnectionScore == 13 || left.TileModifier.ConnectionScore == 14 || left.TileModifier.ConnectionScore == 18 || left.TileModifier.ConnectionScore == 19 || left.TileModifier.ConnectionScore == 20 || left.TileModifier.ConnectionScore == 29)
                            {
                                if (up.HasConnection)
                                {
                                    return new TileConnectionScoreInfo(16);
                                }
                                return new TileConnectionScoreInfo(12);
                            }
                            // wide
                            if (up.HasConnection)
                            {
                                return new TileConnectionScoreInfo(16);
                            }
                            return new TileConnectionScoreInfo(31);
                        }
                        if (up.HasConnection)
                        {
                            // narrow
                            if (up.TileModifier.ConnectionScore == 1 || up.TileModifier.ConnectionScore == 2 || up.TileModifier.ConnectionScore == 3 || up.TileModifier.ConnectionScore == 4 || up.TileModifier.ConnectionScore == 5 || up.TileModifier.ConnectionScore == 6 || up.TileModifier.ConnectionScore == 7 || up.TileModifier.ConnectionScore == 8 || up.TileModifier.ConnectionScore == 9 || up.TileModifier.ConnectionScore == 10 || up.TileModifier.ConnectionScore == 11 || up.TileModifier.ConnectionScore == 12 || up.TileModifier.ConnectionScore == 13 || up.TileModifier.ConnectionScore == 15 || up.TileModifier.ConnectionScore == 17 || up.TileModifier.ConnectionScore == 18 || up.TileModifier.ConnectionScore == 19 || up.TileModifier.ConnectionScore == 20 || up.TileModifier.ConnectionScore == 30)
                            {
                                return new TileConnectionScoreInfo(13);
                            }
                            // wide
                            return new TileConnectionScoreInfo(32);
                        }
                        return new TileConnectionScoreInfo(6);
                    }
                    else // wide
                    {
                        if (left.HasConnection)
                        {
                            if (up.HasConnection)
                            {
                                return new TileConnectionScoreInfo(16);
                            }
                            return new TileConnectionScoreInfo(31);
                        }
                        if (up.HasConnection)
                        {
                            return new TileConnectionScoreInfo(32);
                        }
                        return new TileConnectionScoreInfo(21);
                    }
                }
                if (left.HasConnection)
                {
                    // narrow
                    if (left.TileModifier.ConnectionScore == 1 || left.TileModifier.ConnectionScore == 2 || left.TileModifier.ConnectionScore == 3 || left.TileModifier.ConnectionScore == 4 || left.TileModifier.ConnectionScore == 5 || left.TileModifier.ConnectionScore == 6 || left.TileModifier.ConnectionScore == 7 || left.TileModifier.ConnectionScore == 8 || left.TileModifier.ConnectionScore == 9 || left.TileModifier.ConnectionScore == 10 || left.TileModifier.ConnectionScore == 11 || left.TileModifier.ConnectionScore == 12 || left.TileModifier.ConnectionScore == 13 || left.TileModifier.ConnectionScore == 14 || left.TileModifier.ConnectionScore == 18 || left.TileModifier.ConnectionScore == 19 || left.TileModifier.ConnectionScore == 20 || left.TileModifier.ConnectionScore == 29)
                    {
                        if (up.HasConnection)
                        {
                            // narrow
                            if (up.TileModifier.ConnectionScore == 1 || up.TileModifier.ConnectionScore == 2 || up.TileModifier.ConnectionScore == 3 || up.TileModifier.ConnectionScore == 4 || up.TileModifier.ConnectionScore == 5 || up.TileModifier.ConnectionScore == 6 || up.TileModifier.ConnectionScore == 7 || up.TileModifier.ConnectionScore == 8 || up.TileModifier.ConnectionScore == 9 || up.TileModifier.ConnectionScore == 10 || up.TileModifier.ConnectionScore == 11 || up.TileModifier.ConnectionScore == 12 || up.TileModifier.ConnectionScore == 13 || up.TileModifier.ConnectionScore == 15 || up.TileModifier.ConnectionScore == 17 || up.TileModifier.ConnectionScore == 18 || up.TileModifier.ConnectionScore == 19 || up.TileModifier.ConnectionScore == 20 || up.TileModifier.ConnectionScore == 30)
                            {
                                return new TileConnectionScoreInfo(14);
                            }
                            // wide
                            return new TileConnectionScoreInfo(33);
                        }
                        return new TileConnectionScoreInfo(7);
                    }
                    // wide left
                    if (up.HasConnection)
                    {
                        return new TileConnectionScoreInfo(33);
                    }
                    return new TileConnectionScoreInfo(29);
                }
                if (up.HasConnection)
                {
                    // narrow
                    if (up.TileModifier.ConnectionScore == 1 || up.TileModifier.ConnectionScore == 2 || up.TileModifier.ConnectionScore == 3 || up.TileModifier.ConnectionScore == 4 || up.TileModifier.ConnectionScore == 5 || up.TileModifier.ConnectionScore == 6 || up.TileModifier.ConnectionScore == 7 || up.TileModifier.ConnectionScore == 8 || up.TileModifier.ConnectionScore == 9 || up.TileModifier.ConnectionScore == 10 || up.TileModifier.ConnectionScore == 11 || up.TileModifier.ConnectionScore == 12 || up.TileModifier.ConnectionScore == 13 || up.TileModifier.ConnectionScore == 15 || up.TileModifier.ConnectionScore == 17 || up.TileModifier.ConnectionScore == 18 || up.TileModifier.ConnectionScore == 19 || up.TileModifier.ConnectionScore == 20 || up.TileModifier.ConnectionScore == 30)
                    {
                        return new TileConnectionScoreInfo(8);
                    }
                    // wide
                    return new TileConnectionScoreInfo(23);
                }
                return new TileConnectionScoreInfo(2);
            }
            // wide right
            if (down.HasConnection)
            {
                if (left.HasConnection)
                {
                    if (up.HasConnection)
                    {
                        return new TileConnectionScoreInfo(16);
                    }
                    return new TileConnectionScoreInfo(31);
                }
                if (up.HasConnection)
                {
                    return new TileConnectionScoreInfo(32);
                }
                return new TileConnectionScoreInfo(21);
            }
            if (left.HasConnection)
            {
                if (up.HasConnection)
                {
                    return new TileConnectionScoreInfo(33);
                }
                // narrow
                if (left.TileModifier.ConnectionScore == 1 || left.TileModifier.ConnectionScore == 2 || left.TileModifier.ConnectionScore == 3 || left.TileModifier.ConnectionScore == 4 || left.TileModifier.ConnectionScore == 5 || left.TileModifier.ConnectionScore == 6 || left.TileModifier.ConnectionScore == 7 || left.TileModifier.ConnectionScore == 8 || left.TileModifier.ConnectionScore == 9 || left.TileModifier.ConnectionScore == 10 || left.TileModifier.ConnectionScore == 11 || left.TileModifier.ConnectionScore == 12 || left.TileModifier.ConnectionScore == 13 || left.TileModifier.ConnectionScore == 14 || left.TileModifier.ConnectionScore == 18 || left.TileModifier.ConnectionScore == 19 || left.TileModifier.ConnectionScore == 20 || left.TileModifier.ConnectionScore == 29)
                {
                    return new TileConnectionScoreInfo(27);
                }
                return new TileConnectionScoreInfo(22);
            }
            if (up.HasConnection)
            {
                return new TileConnectionScoreInfo(23);
            }
            return new TileConnectionScoreInfo(17);
        }
        if (down.HasConnection)
        {
            // narrow
            if (down.TileModifier.ConnectionScore == 1 || down.TileModifier.ConnectionScore == 2 || down.TileModifier.ConnectionScore == 3 || down.TileModifier.ConnectionScore == 4 || down.TileModifier.ConnectionScore == 5 || down.TileModifier.ConnectionScore == 6 || down.TileModifier.ConnectionScore == 7 || down.TileModifier.ConnectionScore == 8 || down.TileModifier.ConnectionScore == 9 || down.TileModifier.ConnectionScore == 10 || down.TileModifier.ConnectionScore == 11 || down.TileModifier.ConnectionScore == 13 || down.TileModifier.ConnectionScore == 14 || down.TileModifier.ConnectionScore == 15 || down.TileModifier.ConnectionScore == 17 || down.TileModifier.ConnectionScore == 18 || down.TileModifier.ConnectionScore == 19 || down.TileModifier.ConnectionScore == 20 || down.TileModifier.ConnectionScore == 28)
            {
                if (left.HasConnection)
                {
                    // left = narrow
                    if (left.TileModifier.ConnectionScore == 1 || left.TileModifier.ConnectionScore == 2 || left.TileModifier.ConnectionScore == 3 || left.TileModifier.ConnectionScore == 4 || left.TileModifier.ConnectionScore == 5 || left.TileModifier.ConnectionScore == 6 || left.TileModifier.ConnectionScore == 7 || left.TileModifier.ConnectionScore == 8 || left.TileModifier.ConnectionScore == 9 || left.TileModifier.ConnectionScore == 10 || left.TileModifier.ConnectionScore == 11 || left.TileModifier.ConnectionScore == 12 || left.TileModifier.ConnectionScore == 13 || left.TileModifier.ConnectionScore == 14 || left.TileModifier.ConnectionScore == 18 || left.TileModifier.ConnectionScore == 19 || left.TileModifier.ConnectionScore == 20 || left.TileModifier.ConnectionScore == 29)
                    {
                        if (up.HasConnection)
                        {
                            // narrow
                            if (up.TileModifier.ConnectionScore == 1 || up.TileModifier.ConnectionScore == 2 || up.TileModifier.ConnectionScore == 3 || up.TileModifier.ConnectionScore == 4 || up.TileModifier.ConnectionScore == 5 || up.TileModifier.ConnectionScore == 6 || up.TileModifier.ConnectionScore == 7 || up.TileModifier.ConnectionScore == 8 || up.TileModifier.ConnectionScore == 9 || up.TileModifier.ConnectionScore == 10 || up.TileModifier.ConnectionScore == 11 || up.TileModifier.ConnectionScore == 12 || up.TileModifier.ConnectionScore == 13 || up.TileModifier.ConnectionScore == 15 || up.TileModifier.ConnectionScore == 17 || up.TileModifier.ConnectionScore == 18 || up.TileModifier.ConnectionScore == 19 || up.TileModifier.ConnectionScore == 20 || up.TileModifier.ConnectionScore == 30)
                            {
                                return new TileConnectionScoreInfo(15);
                            }
                            return new TileConnectionScoreInfo(34);
                        }
                        return new TileConnectionScoreInfo(9);
                    }
                    // left = wide
                    if (up.HasConnection)
                    {
                        return new TileConnectionScoreInfo(15);
                    }
                    return new TileConnectionScoreInfo(25);
                }
                if (up.HasConnection)
                {
                    // narrow
                    if (up.TileModifier.ConnectionScore == 1 || up.TileModifier.ConnectionScore == 2 || up.TileModifier.ConnectionScore == 3 || up.TileModifier.ConnectionScore == 4 || up.TileModifier.ConnectionScore == 5 || up.TileModifier.ConnectionScore == 6 || up.TileModifier.ConnectionScore == 7 || up.TileModifier.ConnectionScore == 8 || up.TileModifier.ConnectionScore == 9 || up.TileModifier.ConnectionScore == 10 || up.TileModifier.ConnectionScore == 11 || up.TileModifier.ConnectionScore == 12 || up.TileModifier.ConnectionScore == 13 || up.TileModifier.ConnectionScore == 15 || up.TileModifier.ConnectionScore == 17 || up.TileModifier.ConnectionScore == 18 || up.TileModifier.ConnectionScore == 19 || up.TileModifier.ConnectionScore == 20 || up.TileModifier.ConnectionScore == 21 || up.TileModifier.ConnectionScore == 25 || up.TileModifier.ConnectionScore == 30)
                    {
                        return new TileConnectionScoreInfo(10);
                    }
                    if (up.TileModifier.ConnectionScore == 24 || up.TileModifier.ConnectionScore == 28)
                    {
                        return new TileConnectionScoreInfo(30);
                    }
                    // wide
                    return new TileConnectionScoreInfo(24);
                }
                return new TileConnectionScoreInfo(3);
            }
            // wide down
            if (left.HasConnection)
            {
                if (up.HasConnection)
                {
                    return new TileConnectionScoreInfo(34);
                }
                return new TileConnectionScoreInfo(25);
            }
            if (up.HasConnection)
            {
                // narrow
                if (up.TileModifier.ConnectionScore == 1 || up.TileModifier.ConnectionScore == 2 || up.TileModifier.ConnectionScore == 3 || up.TileModifier.ConnectionScore == 4 || up.TileModifier.ConnectionScore == 5 || up.TileModifier.ConnectionScore == 6 || up.TileModifier.ConnectionScore == 7 || up.TileModifier.ConnectionScore == 8 || up.TileModifier.ConnectionScore == 9 || up.TileModifier.ConnectionScore == 10 || up.TileModifier.ConnectionScore == 11 || up.TileModifier.ConnectionScore == 12 || up.TileModifier.ConnectionScore == 13 || up.TileModifier.ConnectionScore == 15 || up.TileModifier.ConnectionScore == 17 || up.TileModifier.ConnectionScore == 18 || up.TileModifier.ConnectionScore == 19 || up.TileModifier.ConnectionScore == 20 || up.TileModifier.ConnectionScore == 30)
                {
                    return new TileConnectionScoreInfo(28);
                }
                return new TileConnectionScoreInfo(24);
            }
            return new TileConnectionScoreInfo(18);
        }

        if (left.HasConnection)
        {
            // narrow
            if (left.TileModifier.ConnectionScore == 1 || left.TileModifier.ConnectionScore == 2 || left.TileModifier.ConnectionScore == 3 || left.TileModifier.ConnectionScore == 4 || left.TileModifier.ConnectionScore == 5 || left.TileModifier.ConnectionScore == 6 || left.TileModifier.ConnectionScore == 7 || left.TileModifier.ConnectionScore == 8 || left.TileModifier.ConnectionScore == 9 || left.TileModifier.ConnectionScore == 10 || left.TileModifier.ConnectionScore == 11 || left.TileModifier.ConnectionScore == 12 || left.TileModifier.ConnectionScore == 13 || left.TileModifier.ConnectionScore == 14 || left.TileModifier.ConnectionScore == 18 || left.TileModifier.ConnectionScore == 19 || left.TileModifier.ConnectionScore == 20 || left.TileModifier.ConnectionScore == 29)
            {
                if (up.HasConnection)
                {
                    // narrow
                    if (up.TileModifier.ConnectionScore == 1 || up.TileModifier.ConnectionScore == 2 || up.TileModifier.ConnectionScore == 3 || up.TileModifier.ConnectionScore == 4 || up.TileModifier.ConnectionScore == 5 || up.TileModifier.ConnectionScore == 6 || up.TileModifier.ConnectionScore == 7 || up.TileModifier.ConnectionScore == 8 || up.TileModifier.ConnectionScore == 9 || up.TileModifier.ConnectionScore == 10 || up.TileModifier.ConnectionScore == 11 || up.TileModifier.ConnectionScore == 12 || up.TileModifier.ConnectionScore == 13 || up.TileModifier.ConnectionScore == 15 || up.TileModifier.ConnectionScore == 17 || up.TileModifier.ConnectionScore == 18 || up.TileModifier.ConnectionScore == 19 || up.TileModifier.ConnectionScore == 20 || up.TileModifier.ConnectionScore == 30)
                    {
                        return new TileConnectionScoreInfo(11);
                    }
                    return new TileConnectionScoreInfo(26);
                }
                return new TileConnectionScoreInfo(4);
            }
            // wide
            if (up.HasConnection)
            {
                return new TileConnectionScoreInfo(26);
            }
            return new TileConnectionScoreInfo(19);
        }
        if (up.HasConnection)
        {
            // narrow
            if (up.TileModifier.ConnectionScore == 1 || up.TileModifier.ConnectionScore == 2 || up.TileModifier.ConnectionScore == 3 || up.TileModifier.ConnectionScore == 4 || up.TileModifier.ConnectionScore == 5 || up.TileModifier.ConnectionScore == 6 || up.TileModifier.ConnectionScore == 7 || up.TileModifier.ConnectionScore == 8 || up.TileModifier.ConnectionScore == 9 || up.TileModifier.ConnectionScore == 10 || up.TileModifier.ConnectionScore == 11 || up.TileModifier.ConnectionScore == 12 || up.TileModifier.ConnectionScore == 13 || up.TileModifier.ConnectionScore == 15 || up.TileModifier.ConnectionScore == 17 || up.TileModifier.ConnectionScore == 18 || up.TileModifier.ConnectionScore == 19 || up.TileModifier.ConnectionScore == 20 || up.TileModifier.ConnectionScore == 30)
            {
                return new TileConnectionScoreInfo(5);
            }
            return new TileConnectionScoreInfo(20);
        }
        return new TileConnectionScoreInfo(1);
    }
}
