
using UnityEngine;

public static class TileConnectionRegister
{
    public static TileConnectionScoreInfo CalculateTileConnectionScore<T>(TileModifierConnectionInfo<T> right, TileModifierConnectionInfo<T> down, TileModifierConnectionInfo<T> left, TileModifierConnectionInfo<T> up) where T : MonoBehaviour, ITileConnectable
    {
        int rightConnectionScore = right.TileModifier ? right.TileModifier.ConnectionScore : 16;
        int downConnectionScore = down.TileModifier ? down.TileModifier.ConnectionScore : 16;
        int leftConnectionScore = left.TileModifier ? left.TileModifier.ConnectionScore : 16;
        int upConnectionScore = up.TileModifier ? up.TileModifier.ConnectionScore : 16;

        if (right.HasConnection)
        {
            // narrow
            if (rightConnectionScore == 1 || rightConnectionScore == 2 || rightConnectionScore == 3 || rightConnectionScore == 4 || rightConnectionScore == 5 || rightConnectionScore == 6 || rightConnectionScore == 7 || rightConnectionScore == 8 || rightConnectionScore == 9 || rightConnectionScore == 10 || rightConnectionScore == 11 || rightConnectionScore == 12 || rightConnectionScore == 14 || rightConnectionScore == 15 || rightConnectionScore == 17 || rightConnectionScore == 18 || rightConnectionScore == 20 || rightConnectionScore == 27)
            {
                if (down.HasConnection)
                {
                    // narrow
                    if (downConnectionScore == 1 || downConnectionScore == 2 || downConnectionScore == 3 || downConnectionScore == 4 || downConnectionScore == 5 || downConnectionScore == 6 || downConnectionScore == 7 || downConnectionScore == 8 || downConnectionScore == 9 || downConnectionScore == 10 || downConnectionScore == 11 || downConnectionScore == 13 || downConnectionScore == 14 || downConnectionScore == 15 || downConnectionScore == 17 || downConnectionScore == 19 || downConnectionScore == 20 || downConnectionScore == 28)
                    {
                        if (left.HasConnection)
                        {
                            // narrow
                            if (leftConnectionScore == 1 || leftConnectionScore == 2 || leftConnectionScore == 3 || leftConnectionScore == 4 || leftConnectionScore == 5 || leftConnectionScore == 6 || leftConnectionScore == 7 || leftConnectionScore == 8 || leftConnectionScore == 9 || leftConnectionScore == 10 || leftConnectionScore == 11 || leftConnectionScore == 12 || leftConnectionScore == 13 || leftConnectionScore == 14 || leftConnectionScore == 18 || leftConnectionScore == 19 || leftConnectionScore == 20 || leftConnectionScore == 29)
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
                            if (upConnectionScore == 1 || upConnectionScore == 2 || upConnectionScore == 3 || upConnectionScore == 4 || upConnectionScore == 5 || upConnectionScore == 6 || upConnectionScore == 7 || upConnectionScore == 8 || upConnectionScore == 9 || upConnectionScore == 10 || upConnectionScore == 11 || upConnectionScore == 12 || upConnectionScore == 13 || upConnectionScore == 15 || upConnectionScore == 17 || upConnectionScore == 18 || upConnectionScore == 19 || upConnectionScore == 20 || upConnectionScore == 30)
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
                    if (leftConnectionScore == 1 || leftConnectionScore == 2 || leftConnectionScore == 3 || leftConnectionScore == 4 || leftConnectionScore == 5 || leftConnectionScore == 6 || leftConnectionScore == 7 || leftConnectionScore == 8 || leftConnectionScore == 9 || leftConnectionScore == 10 || leftConnectionScore == 11 || leftConnectionScore == 12 || leftConnectionScore == 13 || leftConnectionScore == 14 || leftConnectionScore == 18 || leftConnectionScore == 19 || leftConnectionScore == 20 || leftConnectionScore == 29)
                    {
                        if (up.HasConnection)
                        {
                            // narrow
                            if (upConnectionScore == 1 || upConnectionScore == 2 || upConnectionScore == 3 || upConnectionScore == 4 || upConnectionScore == 5 || upConnectionScore == 6 || upConnectionScore == 7 || upConnectionScore == 8 || upConnectionScore == 9 || upConnectionScore == 10 || upConnectionScore == 11 || upConnectionScore == 12 || upConnectionScore == 13 || upConnectionScore == 15 || upConnectionScore == 17 || upConnectionScore == 18 || upConnectionScore == 19 || upConnectionScore == 20 || upConnectionScore == 30)
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
                    if (upConnectionScore == 1 || upConnectionScore == 2 || upConnectionScore == 3 || upConnectionScore == 4 || upConnectionScore == 5 || upConnectionScore == 6 || upConnectionScore == 7 || upConnectionScore == 8 || upConnectionScore == 9 || upConnectionScore == 10 || upConnectionScore == 11 || upConnectionScore == 12 || upConnectionScore == 13 || upConnectionScore == 15 || upConnectionScore == 17 || upConnectionScore == 18 || upConnectionScore == 19 || upConnectionScore == 20 || upConnectionScore == 30)
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
                if (leftConnectionScore == 1 || leftConnectionScore == 2 || leftConnectionScore == 3 || leftConnectionScore == 4 || leftConnectionScore == 5 || leftConnectionScore == 6 || leftConnectionScore == 7 || leftConnectionScore == 8 || leftConnectionScore == 9 || leftConnectionScore == 10 || leftConnectionScore == 11 || leftConnectionScore == 12 || leftConnectionScore == 13 || leftConnectionScore == 14 || leftConnectionScore == 18 || leftConnectionScore == 19 || leftConnectionScore == 20 || leftConnectionScore == 29)
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
            if (downConnectionScore == 1 || downConnectionScore == 2 || downConnectionScore == 3 || downConnectionScore == 4 || downConnectionScore == 5 || downConnectionScore == 6 || downConnectionScore == 7 || downConnectionScore == 8 || downConnectionScore == 9 || downConnectionScore == 10 || downConnectionScore == 11 || downConnectionScore == 13 || downConnectionScore == 14 || downConnectionScore == 15 || downConnectionScore == 17 || downConnectionScore == 18 || downConnectionScore == 19 || downConnectionScore == 20 || downConnectionScore == 28)
            {
                if (left.HasConnection)
                {
                    // left = narrow
                    if (leftConnectionScore == 1 || leftConnectionScore == 2 || leftConnectionScore == 3 || leftConnectionScore == 4 || leftConnectionScore == 5 || leftConnectionScore == 6 || leftConnectionScore == 7 || leftConnectionScore == 8 || leftConnectionScore == 9 || leftConnectionScore == 10 || leftConnectionScore == 11 || leftConnectionScore == 12 || leftConnectionScore == 13 || leftConnectionScore == 14 || leftConnectionScore == 18 || leftConnectionScore == 19 || leftConnectionScore == 20 || leftConnectionScore == 29)
                    {
                        if (up.HasConnection)
                        {
                            // narrow
                            if (upConnectionScore == 1 || upConnectionScore == 2 || upConnectionScore == 3 || upConnectionScore == 4 || upConnectionScore == 5 || upConnectionScore == 6 || upConnectionScore == 7 || upConnectionScore == 8 || upConnectionScore == 9 || upConnectionScore == 10 || upConnectionScore == 11 || upConnectionScore == 12 || upConnectionScore == 13 || upConnectionScore == 15 || upConnectionScore == 17 || upConnectionScore == 18 || upConnectionScore == 19 || upConnectionScore == 20 || upConnectionScore == 30)
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
                        return new TileConnectionScoreInfo(34);
                    }
                    return new TileConnectionScoreInfo(25);
                }
                if (up.HasConnection)
                {
                    // narrow
                    if (upConnectionScore == 1 || upConnectionScore == 2 || upConnectionScore == 3 || upConnectionScore == 4 || upConnectionScore == 5 || upConnectionScore == 6 || upConnectionScore == 7 || upConnectionScore == 8 || upConnectionScore == 9 || upConnectionScore == 10 || upConnectionScore == 11 || upConnectionScore == 12 || upConnectionScore == 13 || upConnectionScore == 15 || upConnectionScore == 17 || upConnectionScore == 18 || upConnectionScore == 19 || upConnectionScore == 20 || upConnectionScore == 21 || upConnectionScore == 25 || upConnectionScore == 30)
                    {
                        return new TileConnectionScoreInfo(10);
                    }
                    if (upConnectionScore == 24 || upConnectionScore == 28)
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
                if (upConnectionScore == 1 || upConnectionScore == 2 || upConnectionScore == 3 || upConnectionScore == 4 || upConnectionScore == 5 || upConnectionScore == 6 || upConnectionScore == 7 || upConnectionScore == 8 || upConnectionScore == 9 || upConnectionScore == 10 || upConnectionScore == 11 || upConnectionScore == 12 || upConnectionScore == 13 || upConnectionScore == 15 || upConnectionScore == 17 || upConnectionScore == 18 || upConnectionScore == 19 || upConnectionScore == 20 || upConnectionScore == 30)
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
            if (leftConnectionScore == 1 || leftConnectionScore == 2 || leftConnectionScore == 3 || leftConnectionScore == 4 || leftConnectionScore == 5 || leftConnectionScore == 6 || leftConnectionScore == 7 || leftConnectionScore == 8 || leftConnectionScore == 9 || leftConnectionScore == 10 || leftConnectionScore == 11 || leftConnectionScore == 12 || leftConnectionScore == 13 || leftConnectionScore == 14 || leftConnectionScore == 18 || leftConnectionScore == 19 || leftConnectionScore == 20 || leftConnectionScore == 29)
            {
                if (up.HasConnection)
                {
                    // narrow
                    if (upConnectionScore == 1 || upConnectionScore == 2 || upConnectionScore == 3 || upConnectionScore == 4 || upConnectionScore == 5 || upConnectionScore == 6 || upConnectionScore == 7 || upConnectionScore == 8 || upConnectionScore == 9 || upConnectionScore == 10 || upConnectionScore == 11 || upConnectionScore == 12 || upConnectionScore == 13 || upConnectionScore == 15 || upConnectionScore == 17 || upConnectionScore == 18 || upConnectionScore == 19 || upConnectionScore == 20 || upConnectionScore == 30)
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
            if (upConnectionScore == 1 || upConnectionScore == 2 || upConnectionScore == 3 || upConnectionScore == 4 || upConnectionScore == 5 || upConnectionScore == 6 || upConnectionScore == 7 || upConnectionScore == 8 || upConnectionScore == 9 || upConnectionScore == 10 || upConnectionScore == 11 || upConnectionScore == 12 || upConnectionScore == 13 || upConnectionScore == 15 || upConnectionScore == 17 || upConnectionScore == 18 || upConnectionScore == 19 || upConnectionScore == 20 || upConnectionScore == 30)
            {
                return new TileConnectionScoreInfo(5);
            }
            return new TileConnectionScoreInfo(20);
        }
        return new TileConnectionScoreInfo(1);
    }
}
