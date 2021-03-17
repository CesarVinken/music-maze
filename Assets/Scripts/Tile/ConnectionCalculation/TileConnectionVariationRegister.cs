using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileConnectionVariationRegister<T> where T : MonoBehaviour, ITileConnectable
{
    private ITileConnectable _thisMazeTileAttribute;
    private ITileConnectable _connectionRight;
    private ITileConnectable _connectionDown;
    private ITileConnectable _connectionLeft;
    private ITileConnectable _connectionUp;

    private Dictionary<int, int[]> _mazeTileDefaultSpriteNumberByConnectionScore = new Dictionary<int, int[]>
    {
        { 1, new [] { 1 } },
        { 2, new [] { 2 } },
        { 3, new [] { 3 } },
        { 4, new [] { 4 } },
        { 5, new [] { 5 } },
        { 6, new [] { 6 } },
        { 7, new [] { 7 } },
        { 8, new [] { 8 } },
        { 9, new [] { 9 } },
        { 10, new [] { 10 } },
        { 11, new [] { 11 } },
        { 12, new [] { 12 } },
        { 13, new [] { 13 } },
        { 14, new [] { 14 } },
        { 15, new [] { 15 } },
        { 16, new [] { 16 } },
        { 17, new [] { 17 } },
        { 18, new [] { 18 } },
        { 19, new [] { 19 } },
        { 20, new [] { 20 } },
        { 21, new [] { 21 } },
        { 22, new [] { 22 } },
        { 23, new [] { 23 } },
        { 24, new [] { 24 } },
        { 25, new [] { 25 } },
        { 26, new [] { 26 } },
        { 27, new [] { 27 } },
        { 28, new [] { 28 } },
        { 29, new [] { 29 } },
        { 30, new [] { 30 } },
        { 31, new [] { 31 } },
        { 32, new [] { 32 } },
        { 33, new [] { 33 } },
        { 34, new [] { 34 } },
    };

    private Dictionary<int, int[]> _mazeTileBushWallSpriteNumberByConnectionScore = new Dictionary<int, int[]>
    {
        { 1, new [] { 1 } },
        { 2, new [] { 2 } },
        { 3, new [] { 3 } },
        { 4, new [] { 4 } },
        { 5, new [] { 5 } },
        { 6, new [] { 6 } },
        { 7, new [] { 7, 37 } },
        { 8, new [] { 8 } },
        { 9, new [] { 9 } },
        { 10, new [] { 10 } },
        { 11, new [] { 11 } },
        { 12, new [] { 12 } },
        { 13, new [] { 13 } },
        { 14, new [] { 14 } },
        { 15, new [] { 15 } },
        { 16, new [] { 16 } },
        { 17, new [] { 17 } },
        { 18, new [] { 18 } },
        { 19, new [] { 19 } },
        { 20, new [] { 20 } },
        { 21, new [] { 21 } },
        { 22, new [] { 22 } },
        { 23, new [] { 23, 38 } },
        { 24, new [] { 24 } },
        { 25, new [] { 25 } },
        { 26, new [] { 26 } },
        { 27, new [] { 27, 39 } },
        { 28, new [] { 28 } },
        { 29, new [] { 29, 40 } },
        { 30, new [] { 30 } },
        { 31, new [] { 31 } },
        { 32, new [] { 32 } },
        { 33, new [] { 33 } },
        { 34, new [] { 34 } },
    };

    private Dictionary<int, int[]> _spriteNumberRegister = new Dictionary<int, int[]>();

    public TileConnectionVariationRegister(T _thisMazeTileAttribute, TileModifierConnectionInfo<T> connectionRight, TileModifierConnectionInfo<T> connectionDown, TileModifierConnectionInfo<T> connectionLeft, TileModifierConnectionInfo<T> connectionUp)
    {
        this._thisMazeTileAttribute = _thisMazeTileAttribute;
        _connectionRight = connectionRight.TileModifier;
        _connectionDown = connectionDown.TileModifier;
        _connectionLeft = connectionLeft.TileModifier;
        _connectionUp = connectionUp.TileModifier;

        if (_thisMazeTileAttribute.GetSubtypeAsString() == "Bush") // TODO Should not be hardcoded!
        {
            _spriteNumberRegister = _mazeTileBushWallSpriteNumberByConnectionScore;
        }
        else
        {
            _spriteNumberRegister = _mazeTileDefaultSpriteNumberByConnectionScore;
        }
    }

    private TileConnectionScoreInfo GenerateConnectionScoreInfo(int connectionScore)
    {
        int randomVariation = Random.Range(0, _spriteNumberRegister[connectionScore].Length);

        int spriteNumber = _spriteNumberRegister[connectionScore][randomVariation];

        return new TileConnectionScoreInfo(connectionScore, spriteNumber);
    }

    public List<T> MazeTileBackgroundConnection()
    {
        List<T> updatedConnections = new List<T>();

        int currentScoreThisTileBackground = _thisMazeTileAttribute.ConnectionScore;
        int currentScoreRight = _connectionRight != null ? _connectionRight.ConnectionScore : -1;
        int currentScoreDown = _connectionDown != null ? _connectionDown.ConnectionScore : -1;
        int currentScoreLeft = _connectionLeft != null ? _connectionLeft.ConnectionScore : -1;
        int currentScoreUp = _connectionUp != null ? _connectionUp.ConnectionScore : -1;

        if (currentScoreThisTileBackground == 2)
        {
            if (currentScoreRight == 4)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
            }
            else if (currentScoreRight == 7)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
            }
            else if (currentScoreRight == 22)
            {
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
            }
            else if (currentScoreRight == 25 || currentScoreRight == 26)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
            }
            else if (currentScoreRight == 27)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
            }
            else if (currentScoreRight == 29)
            {
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
            }
        }

        if (currentScoreThisTileBackground == 3)
        {
            if (currentScoreDown == 5)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
            }
            else if (currentScoreDown == 10)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
            }
            else if (currentScoreDown == 23 || currentScoreDown == 26)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
            }
            else if (currentScoreDown == 24)
            {
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
            }
            else if (currentScoreDown == 28)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
            }
            else if (currentScoreDown == 30)
            {
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
            }
        }

        if (currentScoreThisTileBackground == 4)
        {
            if (currentScoreLeft == 2)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
            }
            else if (currentScoreLeft == 7)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
            }
            else if (currentScoreLeft == 22)
            {
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
            }
            else if (currentScoreLeft == 21 || currentScoreLeft == 23)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
            }
            else if (currentScoreLeft == 27)
            {
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
            }
            else if (currentScoreLeft == 29)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
            }
        }

        if (currentScoreThisTileBackground == 5)
        {
            if (currentScoreUp == 3)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
            }
            else if (currentScoreUp == 10)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
            }
            else if (currentScoreUp == 21 || currentScoreUp == 25)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
            }
            else if (currentScoreUp == 24)
            {
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
            }
            else if (currentScoreUp == 28)
            {
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
            }
            else if (currentScoreUp == 30)
            {
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
            }
        }

        if (currentScoreThisTileBackground == 6)
        {
            if (currentScoreRight == 9 || currentScoreRight == 11 || currentScoreRight == 12 || currentScoreRight == 14 || currentScoreRight == 15 || currentScoreRight == 16 ||
                currentScoreDown == 8 || currentScoreDown == 11 || currentScoreDown == 13 || currentScoreDown == 14 || currentScoreDown == 15 || currentScoreDown == 16)
            {
                // do nothing
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(21));

                if (currentScoreRight == 4)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                }
                else if (currentScoreRight == 7)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreRight == 27)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }

                if (currentScoreDown == 5)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                }
                else if (currentScoreDown == 10)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreDown == 28)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }
            }
        }

        if (currentScoreThisTileBackground == 7)
        {
            if (currentScoreRight == 4)
            {
                if (currentScoreLeft == 2)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));

                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                }
                else if (currentScoreLeft == 7)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));

                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreLeft == 29)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));

                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }
                else if (currentScoreLeft == 16)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                }
            }
            else if (currentScoreRight == 7)
            {
                if (currentScoreLeft == 2)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));

                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                }
                else if (currentScoreLeft == 7)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));

                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreLeft == 22)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));

                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreLeft == 29)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));

                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }
            }
            else if (currentScoreRight == 16)
            {
                if (currentScoreLeft == 16)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }
                else if (currentScoreLeft == 2)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
            }
            else if (currentScoreRight == 19)
            {
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
            }
            else if (currentScoreRight == 22)
            {
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
            }
            else if (currentScoreRight == 27)
            {
                if (currentScoreLeft == 2)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                }
                else if (currentScoreLeft == 7)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreLeft == 29)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }
            }
            else if (currentScoreRight == 29)
            {
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
            }

            if (currentScoreLeft == 16)
            {
                if (currentScoreRight == 16)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }
                else
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
            }
            else if (currentScoreLeft == 17)
            {
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
            }
            else if (currentScoreLeft == 22)
            {
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
            }
            else if (currentScoreLeft == 27)
            {
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
            }
        }

        if (currentScoreThisTileBackground == 8)
        {
            if (currentScoreRight == 9 || currentScoreRight == 11 || currentScoreRight == 12 || currentScoreRight == 14 || currentScoreRight == 15 || currentScoreRight == 16 ||
                currentScoreUp == 6 || currentScoreUp == 9 || currentScoreUp == 12 || currentScoreUp == 13 || currentScoreUp == 15 || currentScoreUp == 16)
            {
                // do nothing
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(23));

                if (currentScoreRight == 4)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                }
                else if (currentScoreRight == 7)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreRight == 27)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }

                if (currentScoreUp == 3)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreUp == 30)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }
            }
        }

        if (currentScoreThisTileBackground == 9)
        {
            if (currentScoreDown == 8 || currentScoreDown == 11 || currentScoreDown == 13 || currentScoreDown == 14 || currentScoreDown == 15 || currentScoreDown == 16 ||
                currentScoreLeft == 6 || currentScoreLeft == 8 || currentScoreLeft == 12 || currentScoreLeft == 13 || currentScoreLeft == 14 || currentScoreLeft == 16)
            {
                // do nothing
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(25));

                if (currentScoreDown == 5)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                }
                else if (currentScoreDown == 10)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreDown == 28)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }

                if (currentScoreLeft == 2)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                }
                else if (currentScoreLeft == 7)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreLeft == 29)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }
            }
        }

        if (currentScoreThisTileBackground == 10)
        {
            if (currentScoreDown == 26)
            {
                if (currentScoreUp == 12 || currentScoreUp == 13 || currentScoreUp == 15)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
            }
            else if (currentScoreDown == 5)
            {
                if (currentScoreUp == 3)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));

                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                }
                else if (currentScoreUp == 10)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));

                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
            }
            else if (currentScoreDown == 10)
            {
                if (currentScoreUp == 3)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));

                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                }
                else if (currentScoreUp == 5)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));

                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreUp == 10)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));

                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreUp == 30)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));

                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }
            }
            else if (currentScoreDown == 16)
            {
                if (currentScoreUp == 16)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }
                else
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
            }
            else if (currentScoreDown == 20)
            {
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
            }
            else if (currentScoreDown == 24)
            {
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
            }
            else if (currentScoreDown == 28)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
            }
            else if (currentScoreDown == 30)
            {
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
            }
            else
            {
                if (currentScoreUp == 18)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }

            //if (currentScoreUp == 16)
            //{
            //    else
            //    {
            //        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
            //    }
            //}

        }

        if (currentScoreThisTileBackground == 11)
        {
            if (currentScoreUp == 6 || currentScoreUp == 9 || currentScoreUp == 12 || currentScoreUp == 13 || currentScoreUp == 15 || currentScoreUp == 16 ||
                currentScoreLeft == 6 || currentScoreLeft == 8 || currentScoreLeft == 12 || currentScoreLeft == 13 || currentScoreLeft == 14 || currentScoreLeft == 16)
            {
                // do nothing
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(26));
                if (currentScoreUp == 3)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreUp == 30)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }

                if (currentScoreLeft == 2)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                }
                else if (currentScoreLeft == 7)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreLeft == 29)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }
            }
        }

        if (currentScoreThisTileBackground == 12)
        {
            if (currentScoreRight == 7)
            {
                if (currentScoreDown == 10)
                {
                    if (currentScoreLeft == 7)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(31));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                    }
                    else if (currentScoreLeft == 2)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(31));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                    }
                }
                else if (currentScoreDown == 5)
                {
                    if (currentScoreLeft == 7)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(31));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                    }
                    else if (currentScoreLeft == 2)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(31));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                    }
                }
            }
            else if (currentScoreRight == 4)
            {
                if (currentScoreDown == 10)
                {
                    if (currentScoreLeft == 7)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(31));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                    }
                    else if (currentScoreLeft == 2)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(31));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                    }
                }
                else if (currentScoreDown == 5)
                {
                    if (currentScoreLeft == 7)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(31));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                    }
                    else if (currentScoreLeft == 2)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(31));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                    }
                }
            }
        }

        if (currentScoreThisTileBackground == 13)
        {
            if (currentScoreRight == 7)
            {
                if (currentScoreDown == 10)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
                else if (currentScoreDown == 5)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
            }
            else if (currentScoreRight == 4)
            {
                if (currentScoreDown == 10)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
                else if (currentScoreDown == 5)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
            }
        }

        if (currentScoreThisTileBackground == 14)
        {
            if (currentScoreRight == 7)
            {
                if (currentScoreLeft == 7)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(33));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(33));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
                else if (currentScoreLeft == 2)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(33));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(33));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
            }
            else if (currentScoreRight == 4)
            {
                if (currentScoreLeft == 7)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(33));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(33));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
                else if (currentScoreLeft == 2)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(33));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(33));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
            }
        }

        if (currentScoreThisTileBackground == 15)
        {
            if (currentScoreDown == 10)
            {
                if (currentScoreLeft == 7)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
                else if (currentScoreLeft == 2)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
            }
            if (currentScoreDown == 5)
            {
                if (currentScoreLeft == 7)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
                else if (currentScoreLeft == 2)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
            }
        }

        if (currentScoreThisTileBackground == 16)
        {
            if (currentScoreRight == 4)
            {
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
            }
            else if (currentScoreRight == 7)
            {
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
            }
            else if (currentScoreLeft == 27)
            {
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
            }

            if (currentScoreDown == 5)
            {
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
            }
            else if (currentScoreDown == 10)
            {
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
            }
            else if (currentScoreDown == 28)
            {
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
            }

            if (currentScoreLeft == 2)
            {
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
            }
            else if (currentScoreLeft == 7)
            {
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
            }
            else if (currentScoreLeft == 27)
            {
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
            }

            if (currentScoreUp == 3)
            {
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
            }
            else if (currentScoreUp == 10)
            {
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
            }
            else if (currentScoreUp == 30)
            {
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
            }
        }

        if (currentScoreThisTileBackground == 17)
        {
            if (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 32 || currentScoreRight == 33 || currentScoreRight == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));

                if (currentScoreRight == 19)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }
            }
        }

        if (currentScoreThisTileBackground == 18)
        {
            if (currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 25 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));

                if (currentScoreDown == 20)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        if (currentScoreThisTileBackground == 19)
        {
            if (currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }
            }
        }

        else if (currentScoreThisTileBackground == 20)
        {
            if (currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));

                if (currentScoreUp == 18)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileBackground == 21)
        {
            if (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(6));

                if (currentScoreRight == 19)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreDown == 20)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileBackground == 22)
        {
            if ((currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33) &&
                (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34))
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if ((currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33) && currentScoreRight != 25 && currentScoreRight != 26 && currentScoreRight != 31 && currentScoreRight != 33 && currentScoreRight != 34)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreRight == 19)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if ((currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34) && currentScoreLeft != 21 && currentScoreLeft != 23 && currentScoreLeft != 31 && currentScoreLeft != 32 && currentScoreLeft != 33)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }
            }
        }

        else if (currentScoreThisTileBackground == 23)
        {
            if (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(8));

                if (currentScoreRight == 19)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileBackground == 24)
        {
            if ((currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34) &&
                (currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34))
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));

                if (currentScoreDown == 20)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if ((currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34) && currentScoreUp != 21 && currentScoreUp != 25 && currentScoreUp != 31 && currentScoreUp != 32 && currentScoreUp != 34)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if ((currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34) && currentScoreDown != 23 && currentScoreDown != 26 && currentScoreDown != 32 && currentScoreDown != 33 && currentScoreDown != 34)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileBackground == 25)
        {
            if (currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 33 || currentScoreLeft == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(9));

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreDown == 20)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileBackground == 26)
        {
            if (currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 33 || currentScoreLeft == 34 ||
                currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(11));

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileBackground == 27)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34)
            {
                if (currentScoreLeft == 2)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                }
                else if (currentScoreLeft == 7)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));

                if (currentScoreRight == 19)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }
            }
        }

        else if (currentScoreThisTileBackground == 28)
        {
            if (currentScoreDown == 23 || currentScoreDown == 26 || currentScoreUp == 31 || currentScoreDown == 33 || currentScoreDown == 34)
            {
                if (currentScoreUp == 3)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));

                if (currentScoreDown == 20)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileBackground == 29)
        {
            if (currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {
                if (currentScoreRight == 4)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                }
                if (currentScoreRight == 7)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }

                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }
            }
        }

        else if (currentScoreThisTileBackground == 30)
        {
            if (currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreDown == 5)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                }
                else if (currentScoreDown == 10)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));

                if (currentScoreUp == 18)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileBackground == 31)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {
                if (currentScoreRight == 4)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                }
                else if (currentScoreRight == 7)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreRight == 27)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }

                if (currentScoreDown == 5)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                }
                else if (currentScoreDown == 10)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreDown == 28)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }

                if (currentScoreLeft == 2)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                }
                else if (currentScoreLeft == 7)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreLeft == 29)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(12));

                if (currentScoreRight == 19)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreDown == 20)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }
            }
        }

        else if (currentScoreThisTileBackground == 32)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreRight == 4)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                }
                else if (currentScoreRight == 7)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreRight == 27)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }

                if (currentScoreDown == 5)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                }
                else if (currentScoreDown == 10)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreDown == 28)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }

                if (currentScoreUp == 3)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreUp == 30)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(13));

                if (currentScoreRight == 19)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreDown == 20)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileBackground == 33)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreLeft == 21 || currentScoreLeft == 2 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33 ||
                currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreRight == 4)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                }
                else if (currentScoreRight == 7)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreRight == 27)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }

                if (currentScoreLeft == 2)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                }
                else if (currentScoreLeft == 7)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreLeft == 29)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }

                if (currentScoreUp == 3)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreUp == 30)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(14));

                if (currentScoreRight == 19)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileBackground == 34)
        {
            if (currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33 ||
                currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreDown == 5)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                }
                else if (currentScoreDown == 10)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreDown == 28)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }

                if (currentScoreLeft == 2)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                }
                else if (currentScoreLeft == 7)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreLeft == 29)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }

                if (currentScoreUp == 3)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreUp == 30)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(15));

                if (currentScoreDown == 20)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        if (currentScoreThisTileBackground != _thisMazeTileAttribute.ConnectionScore)
            updatedConnections.Add((T)_thisMazeTileAttribute);
        if (_connectionRight != null && currentScoreRight != _connectionRight.ConnectionScore)
            updatedConnections.Add((T)_connectionRight);
        if (_connectionDown != null && currentScoreDown != _connectionDown.ConnectionScore)
            updatedConnections.Add((T)_connectionDown);
        if (_connectionLeft != null && currentScoreLeft != _connectionLeft.ConnectionScore)
            updatedConnections.Add((T)_connectionLeft);
        if (_connectionUp != null && currentScoreUp != _connectionUp.ConnectionScore)
            updatedConnections.Add((T)_connectionUp);
        Logger.Log($"::::::::::updated {currentScoreThisTileBackground} to {_thisMazeTileAttribute.ConnectionScore}:::");
        return updatedConnections;
    }

    public List<T> TileAttribute()
    {
        List<T> updatedConnections = new List<T>();

        Dictionary<int, int[]> spriteNumberRegister = new Dictionary<int, int[]>();

        int currentScoreThisTileAttribute = _thisMazeTileAttribute.ConnectionScore;
        int currentScoreRight = _connectionRight != null ? _connectionRight.ConnectionScore : -1;
        int currentScoreDown = _connectionDown != null ? _connectionDown.ConnectionScore : -1;
        int currentScoreLeft = _connectionLeft != null ? _connectionLeft.ConnectionScore : -1;
        int currentScoreUp = _connectionUp != null ? _connectionUp.ConnectionScore : -1;

        if (currentScoreThisTileAttribute == 2)
        {
            if (currentScoreRight == 7)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
            }
            else if (currentScoreRight == 22)
            {
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
            }
            else if (currentScoreRight == 25 || currentScoreRight == 26)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
            }
            else if (currentScoreRight == 29)
            {
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
            }
        }

        if (currentScoreThisTileAttribute == 3)
        {
            if (currentScoreDown == 10)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
            }
            else if (currentScoreDown == 23 || currentScoreDown == 26)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
            }
            else if (currentScoreDown == 24)
            {
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
            }
            else if (currentScoreDown == 30)
            {
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
            }
        }

        if (currentScoreThisTileAttribute == 4)
        {
            if (currentScoreLeft == 7)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
            }
            else if (currentScoreLeft == 22)
            {
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
            }
            else if (currentScoreLeft == 21 || currentScoreLeft == 23)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
            }
            else if (currentScoreLeft == 27)
            {
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
            }
        }

        if (currentScoreThisTileAttribute == 5)
        {
            if (currentScoreUp == 10)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
            }
            else if (currentScoreUp == 21 || currentScoreUp == 25)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
            }
            else if (currentScoreUp == 24)
            {
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
            }
            else if (currentScoreUp == 28)
            {
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
            }
        }

        if (currentScoreThisTileAttribute == 6)
        {
            if (currentScoreRight == 9 || currentScoreRight == 11 || currentScoreRight == 12 || currentScoreRight == 14 || currentScoreRight == 15 ||
                currentScoreDown == 8 || currentScoreDown == 11 || currentScoreDown == 13 || currentScoreDown == 14 || currentScoreDown == 15)
            {
                // do nothing
            }
            //else if(currentScoreDown == 16)
            //{

            //}
            //else if(currentScoreDown == 16)
            //{

            //}
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(21));

                if (currentScoreRight == 4)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                }
                else if (currentScoreRight == 7)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreRight == 27)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }

                if (currentScoreDown == 5)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                }
                else if (currentScoreDown == 10)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreDown == 28)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }
            }
        }

        if (currentScoreThisTileAttribute == 7)
        {
            if (currentScoreRight == 4)
            {
                if (currentScoreLeft == 2)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));

                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                }
                else if (currentScoreLeft == 7)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));

                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
            }
            else if (currentScoreRight == 7)
            {
                if (currentScoreLeft == 2)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));

                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                }
                else if (currentScoreLeft == 7)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));

                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if(currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
            }
            else if (currentScoreRight == 16)
            {
                if (currentScoreLeft == 16)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }
                else
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
            }
            else if (currentScoreRight == 19)
            {
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
            }
            else if (currentScoreRight == 22)
            {
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
            }
            else if (currentScoreRight == 25 || currentScoreRight == 26)
            {
                if ( currentScoreLeft == 2 || currentScoreLeft == 7)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
            }
            else if (currentScoreRight == 29)
            {
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
            }
            else if (currentScoreRight == 31)
            {
                if(currentScoreLeft == 7 || currentScoreLeft == 2)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
            }

            if (currentScoreLeft == 16)
            {
                if (currentScoreRight == 16)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }
                else
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
            }
            else if (currentScoreLeft == 17)
            {
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
            }
            else if (currentScoreLeft == 22)
            {
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
            }
            else if (currentScoreLeft == 27)
            {
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
            }
        }

        if (currentScoreThisTileAttribute == 8)
        {
            if (currentScoreRight == 9 || currentScoreRight == 11 || currentScoreRight == 12 || currentScoreRight == 14 || currentScoreRight == 15 ||
                currentScoreUp == 6 || currentScoreUp == 9 || currentScoreUp == 12 || currentScoreUp == 13 || currentScoreUp == 15)
            {
                // do nothing
            }
            //else if(currentScoreRight == 16)
            //{

            //}
            //else if(currentScoreUp == 16)
            //{

            //}
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(23));

                if (currentScoreRight == 4)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                }
                else if (currentScoreRight == 7)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreRight == 27)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }

                if (currentScoreUp == 3)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreUp == 30)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }
            }
        }

        if (currentScoreThisTileAttribute == 9)
        {
            if (currentScoreDown == 8 || currentScoreDown == 11 || currentScoreDown == 13 || currentScoreDown == 14 || currentScoreDown == 15 || 
                currentScoreLeft == 6 || currentScoreLeft == 8 || currentScoreLeft == 12 || currentScoreLeft == 13 || currentScoreLeft == 14 )
            {
                // do nothing
            }
            //else if(currentScoreDown == 16)
            //{

            //}
            //else if(currentScoreLeft == 16)
            //{
                
            //}
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(25));

                if (currentScoreDown == 5)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                }
                else if (currentScoreDown == 10)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreDown == 28)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }

                if (currentScoreLeft == 2)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                }
                else if (currentScoreLeft == 7)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreLeft == 29)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }
            }
        }

        if (currentScoreThisTileAttribute == 10)
        {
            if (currentScoreDown == 26)
            {
                if (currentScoreUp == 12 || currentScoreUp == 13 || currentScoreUp == 15)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
            }

            if (currentScoreDown == 10 && currentScoreUp == 10)
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));

                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
            }
            else if (currentScoreDown == 5)
            {
                if (currentScoreUp == 3)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));

                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                }
            }
            else if (currentScoreDown == 10)
            {
                if (currentScoreUp == 3)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));

                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                }
            }
            else if (currentScoreDown == 16)
            {
                if (currentScoreUp == 16)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }
                else
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
            }
            else if (currentScoreDown == 20)
            {
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
            }
            else if (currentScoreDown == 24)
            {
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
            }
            else if (currentScoreDown == 30)
            {
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
            }

            if (currentScoreUp == 16)
            {
                if (currentScoreDown == 16)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }
                else
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
            }
            else if (currentScoreUp == 18)
            {
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
            }
            else if (currentScoreUp == 24)
            {
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
            }
            else if (currentScoreUp == 28)
            {
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
            }
        }

        if (currentScoreThisTileAttribute == 11)
        {
            if (currentScoreUp == 6 || currentScoreUp == 9 || currentScoreUp == 12 || currentScoreUp == 13 || currentScoreUp == 15 ||
                currentScoreLeft == 6 || currentScoreLeft == 8 || currentScoreLeft == 12 || currentScoreLeft == 13 || currentScoreLeft == 14)
            {
                // do nothing
            }
            //else if(currentScoreUp == 16)
            //{

            //}
            //else if(currentScoreLeft == 16)
            //{

            //} 
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(26));
                if (currentScoreUp == 3)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreUp == 30)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }

                if (currentScoreLeft == 2)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                }
                else if (currentScoreLeft == 7)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreLeft == 29)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }
            }
        }

        if (currentScoreThisTileAttribute == 12)
        {
            if (currentScoreRight == 7)
            {
                if (currentScoreDown == 10)
                {
                    if (currentScoreLeft == 7)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(31));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                    }
                    else if (currentScoreLeft == 2)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(31));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                    }
                }
                else if (currentScoreDown == 5)
                {
                    if (currentScoreLeft == 7)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(31));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                    }
                    else if (currentScoreLeft == 2)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(31));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                    }
                }
            }
            else if (currentScoreRight == 4)
            {
                if (currentScoreDown == 10)
                {
                    if (currentScoreLeft == 7)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(31));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                    }
                    else if (currentScoreLeft == 2)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(31));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                    }
                }
                else if (currentScoreDown == 5)
                {
                    if (currentScoreLeft == 7)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(31));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                    }
                    else if (currentScoreLeft == 2)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(31));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                    }
                }
            }
        }

        if (currentScoreThisTileAttribute == 13)
        {
            if (currentScoreRight == 7)
            {
                if (currentScoreDown == 10)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
                else if (currentScoreDown == 5)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
            }
            else if (currentScoreRight == 4)
            {
                if (currentScoreDown == 10)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
                else if (currentScoreDown == 5)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
            }
            else if(currentScoreRight == 16)
            {
                if(currentScoreDown == 10)
                {
                    if(currentScoreUp == 13)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                    }
                }
            }
        }

        if (currentScoreThisTileAttribute == 14)
        {
            if (currentScoreRight == 7)
            {
                if (currentScoreLeft == 7)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(33));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(33));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
                else if (currentScoreLeft == 2)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(33));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(33));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
            }
            else if (currentScoreRight == 4)
            {
                if (currentScoreLeft == 7)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(33));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(33));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
                else if (currentScoreLeft == 2)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(33));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(33));
                        _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
            }
        }

        if (currentScoreThisTileAttribute == 15)
        {
            if (currentScoreDown == 10)
            {
                if (currentScoreLeft == 7)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
                else if (currentScoreLeft == 2)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
            }
            if (currentScoreDown == 5)
            {
                if (currentScoreLeft == 7)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
                else if (currentScoreLeft == 2)
                {
                    if (currentScoreUp == 10)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    }
                    else if (currentScoreUp == 3)
                    {
                        _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(32));
                        _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                        _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                        _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    }
                }
            }
        }

        if (currentScoreThisTileAttribute == 16)
        {
            if (currentScoreRight == 4)
            {
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
            }
            else if (currentScoreRight == 7)
            {
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
            }
            else if (currentScoreLeft == 27)
            {
                _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
            }

            if (currentScoreDown == 5)
            {
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
            }
            else if (currentScoreDown == 10)
            {
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
            }
            else if (currentScoreDown == 28)
            {
                _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
            }

            if (currentScoreLeft == 2)
            {
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
            }
            else if (currentScoreLeft == 7)
            {
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
            }
            else if (currentScoreLeft == 27)
            {
                _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
            }

            if (currentScoreUp == 3)
            {
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
            }
            else if (currentScoreUp == 10)
            {
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
            }
            else if (currentScoreUp == 30)
            {
                _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
            }
        }

        if (currentScoreThisTileAttribute == 17)
        {
            if (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 32 || currentScoreRight == 33 || currentScoreRight == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));

                if (currentScoreRight == 19)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
            }
        }

        if (currentScoreThisTileAttribute == 18)
        {
            if (currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 25 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));

                if (currentScoreDown == 20)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        if (currentScoreThisTileAttribute == 19)
        {
            if (currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }
            }
        }

        else if (currentScoreThisTileAttribute == 20)
        {
            if (currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));

                if (currentScoreUp == 18)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileAttribute == 21)
        {
            if (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(6));

                if (currentScoreRight == 19)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreDown == 20)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileAttribute == 22)
        {
            if ((currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33) &&
                (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34))
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if ((currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33) && currentScoreRight != 25 && currentScoreRight != 26 && currentScoreRight != 31 && currentScoreRight != 33 && currentScoreRight != 34)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreRight == 19)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if ((currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34) && currentScoreLeft != 21 && currentScoreLeft != 23 && currentScoreLeft != 31 && currentScoreLeft != 32 && currentScoreLeft != 33)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }
            }
        }

        else if (currentScoreThisTileAttribute == 23)
        {
            if (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(8));

                if (currentScoreRight == 19)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileAttribute == 24)
        {
            if ((currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34) &&
                (currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34))
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));

                if (currentScoreDown == 20)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if ((currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34) && currentScoreUp != 21 && currentScoreUp != 25 && currentScoreUp != 31 && currentScoreUp != 32 && currentScoreUp != 34)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if ((currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34) && currentScoreDown != 23 && currentScoreDown != 26 && currentScoreDown != 32 && currentScoreDown != 33 && currentScoreDown != 34)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileAttribute == 25)
        {
            if (currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 33 || currentScoreLeft == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(9));

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreDown == 20)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileAttribute == 26)
        {
            if (currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 33 || currentScoreLeft == 34 ||
                currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(11));

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileAttribute == 27)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34)
            {
                if (currentScoreLeft == 2)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                }
                else if (currentScoreLeft == 7)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));

                if (currentScoreRight == 19)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }
            }
        }

        else if (currentScoreThisTileAttribute == 28)
        {
            if (currentScoreDown == 23 || currentScoreDown == 26 || currentScoreUp == 31 || currentScoreDown == 33 || currentScoreDown == 34)
            {
                if (currentScoreUp == 3)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));

                if (currentScoreDown == 20)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileAttribute == 29)
        {
            if (currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {
                if (currentScoreRight == 4)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                }
                if (currentScoreRight == 7)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }

                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }
            }
        }

        else if (currentScoreThisTileAttribute == 30)
        {
            if (currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreDown == 5)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                }
                else if (currentScoreDown == 10)
                {
                    _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));

                if (currentScoreUp == 18)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileAttribute == 31)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {
                if (currentScoreRight == 4)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                }
                else if (currentScoreRight == 7)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreRight == 27)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }

                if (currentScoreDown == 5)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                }
                else if (currentScoreDown == 10)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreDown == 28)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }

                if (currentScoreLeft == 2)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                }
                else if (currentScoreLeft == 7)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreLeft == 29)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(12));

                if (currentScoreRight == 19)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreDown == 20)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }
            }
        }

        else if (currentScoreThisTileAttribute == 32)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreRight == 4)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                }
                else if (currentScoreRight == 7)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreRight == 27)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }

                if (currentScoreDown == 5)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                }
                else if (currentScoreDown == 10)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreDown == 28)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }

                if (currentScoreUp == 3)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreUp == 30)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(13));

                if (currentScoreRight == 19)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreDown == 20)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileAttribute == 33)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreLeft == 21 || currentScoreLeft == 2 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33 ||
                currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreRight == 4)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(19));
                }
                else if (currentScoreRight == 7)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreRight == 27)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }

                if (currentScoreLeft == 2)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                }
                else if (currentScoreLeft == 7)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreLeft == 29)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }

                if (currentScoreUp == 3)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreUp == 30)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(14));

                if (currentScoreRight == 19)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(4));
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        else if (currentScoreThisTileAttribute == 34)
        {
            if (currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33 ||
                currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreDown == 5)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(20));
                }
                else if (currentScoreDown == 10)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreDown == 28)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }

                if (currentScoreLeft == 2)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(17));
                }
                else if (currentScoreLeft == 7)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(27));
                }
                else if (currentScoreLeft == 29)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(22));
                }

                if (currentScoreUp == 3)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(18));
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreUp == 30)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(24));
                }
            }
            else
            {
                _thisMazeTileAttribute.WithConnectionScoreInfo(GenerateConnectionScoreInfo(15));

                if (currentScoreDown == 20)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(5));
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(28));
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(2));
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(29));
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.WithConnectionScoreInfo(GenerateConnectionScoreInfo(7));
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(3));
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(30));
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.WithConnectionScoreInfo(GenerateConnectionScoreInfo(10));
                }
            }
        }

        if (currentScoreThisTileAttribute != _thisMazeTileAttribute.ConnectionScore)
            updatedConnections.Add((T)_thisMazeTileAttribute);
        if (_connectionRight != null && currentScoreRight != _connectionRight.ConnectionScore)
            updatedConnections.Add((T)_connectionRight);
        if (_connectionDown != null && currentScoreDown != _connectionDown.ConnectionScore)
            updatedConnections.Add((T)_connectionDown);
        if (_connectionLeft != null && currentScoreLeft != _connectionLeft.ConnectionScore)
            updatedConnections.Add((T)_connectionLeft);
        if (_connectionUp != null && currentScoreUp != _connectionUp.ConnectionScore)
            updatedConnections.Add((T)_connectionUp);

        return updatedConnections;
    }
}