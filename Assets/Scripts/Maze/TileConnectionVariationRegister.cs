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

    private Dictionary<int, int[]> _mazeTilePathSpriteNumberByConnectionScore = new Dictionary<int, int[]>
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

    public TileConnectionVariationRegister(T _thisMazeTileAttribute, TileModifierConnectionInfo<T> connectionRight, TileModifierConnectionInfo<T> connectionDown, TileModifierConnectionInfo<T> connectionLeft, TileModifierConnectionInfo<T> connectionUp)
    {
        this._thisMazeTileAttribute = _thisMazeTileAttribute;
        _connectionRight = connectionRight.TileModifier;
        _connectionDown = connectionDown.TileModifier;
        _connectionLeft = connectionLeft.TileModifier;
        _connectionUp = connectionUp.TileModifier;
    }

    private int _pickSpriteNumber(int connectionScore)
    {
        int randomVariation = Random.Range(0, _mazeTilePathSpriteNumberByConnectionScore[connectionScore].Length - 1);

        int spriteNumber = _mazeTilePathSpriteNumberByConnectionScore[connectionScore][randomVariation];

        return spriteNumber;
    }

    public List<MazeTilePath> MazeTilePath()
    {
        List<MazeTilePath> updatedConnections = new List<MazeTilePath>();

        int currentScoreThisTilePath = _thisMazeTileAttribute.ConnectionScore;
        int currentScoreRight = _connectionRight != null ? _connectionRight.ConnectionScore : -1;
        int currentScoreDown = _connectionDown != null ? _connectionDown.ConnectionScore : -1;
        int currentScoreLeft = _connectionLeft != null ? _connectionLeft.ConnectionScore : -1;
        int currentScoreUp = _connectionUp != null ? _connectionUp.ConnectionScore : -1;

        if (currentScoreThisTilePath == 2)
        {
            if (currentScoreRight == 7)
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(17);
                _connectionRight.ConnectionScore = _pickSpriteNumber(29);
            }
            else if (currentScoreRight == 22)
            {
                _connectionRight.ConnectionScore = _pickSpriteNumber(27);
            }
            else if (currentScoreRight == 29)
            {
                _connectionRight.ConnectionScore = _pickSpriteNumber(7);
            }
        }

        if (currentScoreThisTilePath == 3)
        {
            if (currentScoreDown == 10)
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(18);
                _connectionDown.ConnectionScore = _pickSpriteNumber(30);
            }
            else if (currentScoreDown == 24)
            {
                _connectionDown.ConnectionScore = _pickSpriteNumber(28);
            }
            else if (currentScoreDown == 30)
            {
                _connectionDown.ConnectionScore = _pickSpriteNumber(10);
            }
        }

        if (currentScoreThisTilePath == 4)
        {
            if (currentScoreLeft == 7)
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(19);
                _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
            }
            else if (currentScoreLeft == 22)
            {
                _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
            }
            else if (currentScoreLeft == 27)
            {
                _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
            }
        }

        if (currentScoreThisTilePath == 5)
        {
            if (currentScoreUp == 10)
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(20);
                _connectionUp.ConnectionScore = _pickSpriteNumber(28);
            }
            else if (currentScoreUp == 24)
            {
                _connectionUp.ConnectionScore = _pickSpriteNumber(30);
            }
            else if (currentScoreUp == 28)
            {
                _connectionUp.ConnectionScore = _pickSpriteNumber(10);
            }
        }

        if (currentScoreThisTilePath == 6)
        {
            if (currentScoreRight == 22 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 29 || currentScoreRight == 31 || currentScoreRight == 32 || currentScoreRight == 33)
            {
                if (currentScoreDown == 23 || currentScoreDown == 24 || currentScoreDown == 26 || currentScoreDown == 30 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                }
                else if (currentScoreDown == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreDown == 28)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                }
            }
            else if (currentScoreRight == 7)
            {
                if (currentScoreDown == 23 || currentScoreDown == 24 || currentScoreDown == 26 || currentScoreDown == 30 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreDown == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreDown == 28)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                }
            }
            else if (currentScoreRight == 27)
            {
                if (currentScoreDown == 23 || currentScoreDown == 24 || currentScoreDown == 26 || currentScoreDown == 30 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                }
                else if (currentScoreDown == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreDown == 28)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                }
            }
        }

        if (currentScoreThisTilePath == 8)
        {
            if (currentScoreRight == 22 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 29 || currentScoreRight == 31 || currentScoreRight == 32 || currentScoreRight == 33)
            {
                if (currentScoreUp == 21 || currentScoreUp == 24 || currentScoreUp == 25 || currentScoreUp == 28 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                }
                else if (currentScoreUp == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);

                }
                else if(currentScoreUp == 30)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
            }
            else if (currentScoreRight == 7)
            {
                if (currentScoreUp == 23 || currentScoreUp == 24 || currentScoreUp == 26 || currentScoreUp == 30 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreUp == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreUp == 30)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
            }
            else if (currentScoreRight == 27)
            {
                if (currentScoreUp == 23 || currentScoreUp == 24 || currentScoreUp == 26 || currentScoreUp == 30 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                }
                else if (currentScoreUp == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreUp == 30)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
            }
        }

        if (currentScoreThisTilePath == 9)
        {
            if (currentScoreLeft == 21 || currentScoreLeft == 22 || currentScoreLeft == 23 || currentScoreLeft == 27 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {
                if (currentScoreDown == 23 || currentScoreDown == 24 || currentScoreDown == 26 || currentScoreDown == 30 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                }
                else if (currentScoreDown == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreDown == 28)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                }
            }
            else if(currentScoreLeft == 7)
            {
                if (currentScoreDown == 23 || currentScoreDown == 24 || currentScoreDown == 26 || currentScoreDown == 30 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreDown == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreDown == 28)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                }
            }
            else if(currentScoreLeft == 29)
            {
                if (currentScoreDown == 23 || currentScoreDown == 24 || currentScoreDown == 26 || currentScoreDown == 30 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                }
                else if (currentScoreDown == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                }
                else if (currentScoreDown == 28)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                }
            }
        }

        if (currentScoreThisTilePath == 11)
        {
            if (currentScoreUp == 21 || currentScoreUp == 24 || currentScoreUp == 25 || currentScoreUp == 28 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {
                if (currentScoreLeft == 21 || currentScoreLeft == 22 || currentScoreLeft == 23 || currentScoreLeft == 27 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                }
                else if (currentScoreLeft == 7)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreLeft == 29)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                }
            }
            else if (currentScoreUp == 10)
            {
                if (currentScoreLeft == 21 || currentScoreLeft == 22 || currentScoreLeft == 23 || currentScoreLeft == 27 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreLeft == 7)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreLeft == 29)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);
                }
            }
            else if (currentScoreUp == 30)
            {
                if (currentScoreLeft == 21 || currentScoreLeft == 22 || currentScoreLeft == 23 || currentScoreLeft == 27 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
                else if (currentScoreLeft == 7)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
                else if (currentScoreLeft == 29)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
            }
        }

        if (currentScoreThisTilePath == 7)
        {
            if (currentScoreRight == 19)
            {
                _connectionRight.ConnectionScore = _pickSpriteNumber(4);
            }
            else if (currentScoreRight == 22)
            {
                _connectionRight.ConnectionScore = _pickSpriteNumber(27);
            }
            else if (currentScoreRight == 29)
            {
                _connectionRight.ConnectionScore = _pickSpriteNumber(7);
            }

            if (currentScoreLeft == 17)
            {
                _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
            }
            else if (currentScoreLeft == 22)
            {
                _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
            }
            else if (currentScoreLeft == 27)
            {
                _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
            }
        }

        if (currentScoreThisTilePath == 10)
        {
            if (currentScoreDown == 20)
            {
                _connectionDown.ConnectionScore = _pickSpriteNumber(5);
            }
            else if (currentScoreDown == 24)
            {
                _connectionDown.ConnectionScore = _pickSpriteNumber(28);
            }
            else if (currentScoreDown == 30)
            {
                _connectionDown.ConnectionScore = _pickSpriteNumber(10);
            }

            if (currentScoreUp == 18)
            {
                _connectionUp.ConnectionScore = _pickSpriteNumber(3);
            }
            else if (currentScoreUp == 24)
            {
                _connectionUp.ConnectionScore = _pickSpriteNumber(30);
            }
            else if (currentScoreUp == 28)
            {
                _connectionUp.ConnectionScore = _pickSpriteNumber(10);
            }
        }

        if (currentScoreThisTilePath == 16)
        {
            if (currentScoreRight == 7)
            {
                _connectionRight.ConnectionScore = _pickSpriteNumber(29);
            }
            else if (currentScoreLeft == 27)
            {
                _connectionRight.ConnectionScore = _pickSpriteNumber(22);
            }

            if (currentScoreDown == 10)
            {
                _connectionDown.ConnectionScore = _pickSpriteNumber(30);
            }
            else if (currentScoreDown == 28)
            {
                _connectionDown.ConnectionScore = _pickSpriteNumber(24);
            }

            if (currentScoreLeft == 7)
            {
                _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
            }
            else if (currentScoreLeft == 27)
            {
                _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
            }

            if (currentScoreUp == 10)
            {
                _connectionUp.ConnectionScore = _pickSpriteNumber(28);
            }
            else if (currentScoreUp == 30)
            {
                _connectionUp.ConnectionScore = _pickSpriteNumber(24);
            }
        }

        if (currentScoreThisTilePath == 17)
        {
            if (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 32 || currentScoreRight == 33 || currentScoreRight == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(2);

                if (currentScoreRight == 19)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(4);
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
            }
        }

        if (currentScoreThisTilePath == 18)
        {
            if (currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 25 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(3);

                if (currentScoreDown == 20)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(5);
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        if (currentScoreThisTilePath == 19)
        {
            if (currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(4);

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }
            }
        }

        else if (currentScoreThisTilePath == 20)
        {
            if (currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(5);

                if (currentScoreUp == 18)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(3);
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 21)
        {
            if (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(6);

                if (currentScoreRight == 19)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(4);
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreDown == 20)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(5);
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 22)
        {
            if ((currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33) &&
                (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34))
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(7);

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }
                else if ((currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33) && currentScoreRight != 25 && currentScoreRight != 26 && currentScoreRight != 31 && currentScoreRight != 33 && currentScoreRight != 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreRight == 19)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(4);
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if ((currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34) && currentScoreLeft != 21 && currentScoreLeft != 23 && currentScoreLeft != 31 && currentScoreLeft != 32 && currentScoreLeft != 33)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(7);
                }
            }
        }

        else if (currentScoreThisTilePath == 23)
        {
            if (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(8);

                if (currentScoreRight == 19)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(4);
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(3);
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 24)
        {
            if ((currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34) &&
                (currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34))
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(10);

                if (currentScoreDown == 20)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(5);
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if ((currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34) && currentScoreUp != 21 && currentScoreUp != 25 && currentScoreUp != 31 && currentScoreUp != 32 && currentScoreUp != 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(3);
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if ((currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34) && currentScoreDown != 23 && currentScoreDown != 26 && currentScoreDown != 32 && currentScoreDown != 33 && currentScoreDown != 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 25)
        {
            if (currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 33 || currentScoreLeft == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(9);

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreDown == 20)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(5);
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 26)
        {
            if (currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 33 || currentScoreLeft == 34 ||
                currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(11);

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(3);
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 27)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34)
            {
                if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }
            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(7);

                if (currentScoreRight == 19)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(4);
                }
                else if(currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(7);
                }
            }
        }

        else if (currentScoreThisTilePath == 28)
        {
            if (currentScoreDown == 23 || currentScoreDown == 26 || currentScoreUp == 31 || currentScoreDown == 33 || currentScoreDown == 34)
            {
                if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(10);

                if (currentScoreDown == 20)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(5);
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 29)
        {
            if (currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {
                if (currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(7);
                }
            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(7);

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }

                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }
            }
        }

        else if (currentScoreThisTilePath == 30)
        {
            if (currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }
            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(10);

                if (currentScoreUp == 18)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(3);
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 31)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {
                if (currentScoreRight == 4)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(19);
                }
                else if (currentScoreRight == 7)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreRight == 27)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                }

                if (currentScoreDown == 5)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(20);
                }
                else if (currentScoreDown == 10)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreDown == 28)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                }

                if (currentScoreLeft == 2)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(17);
                }
                else if (currentScoreLeft == 7)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreLeft == 29)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                }
            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(12);

                if (currentScoreRight == 19)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(4);
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreDown == 20)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(5);
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }
            }
        }

        else if (currentScoreThisTilePath == 32)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreRight == 4)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(19);
                }
                else if (currentScoreRight == 7)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreRight == 27)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                }

                if (currentScoreDown == 5)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(20);
                }
                else if (currentScoreDown == 10)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreDown == 28)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                }

                if (currentScoreUp == 3)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(18);
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreUp == 30)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(13);

                if (currentScoreRight == 19)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(4);
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreDown == 20)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(5);
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(3);
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 33)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreLeft == 21 || currentScoreLeft == 2 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33 ||
                currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreRight == 4)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(19);
                }
                else if (currentScoreRight == 7)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreRight == 27)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                }

                if (currentScoreLeft == 2)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(17);
                }
                else if (currentScoreLeft == 7)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreLeft == 29)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                }

                if (currentScoreUp == 3)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(18);
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreUp == 30)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(14);

                if (currentScoreRight == 19)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(4);
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(3);
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 34)
        {
            if (currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33 ||
                currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreDown == 5)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(20);
                }
                else if (currentScoreDown == 10)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreDown == 28)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                }

                if (currentScoreLeft == 2)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(17);
                }
                else if (currentScoreLeft == 7)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreLeft == 29)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                }

                if (currentScoreUp == 3)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(18);
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreUp == 30)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(15);

                if (currentScoreDown == 20)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(5);
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(3);
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        if (currentScoreThisTilePath != _thisMazeTileAttribute.ConnectionScore)
            updatedConnections.Add((MazeTilePath)_thisMazeTileAttribute);
        if (_connectionRight != null && currentScoreRight != _connectionRight.ConnectionScore)
            updatedConnections.Add((MazeTilePath)_connectionRight);
        if (_connectionDown != null && currentScoreDown != _connectionDown.ConnectionScore)
            updatedConnections.Add((MazeTilePath)_connectionDown);
        if (_connectionLeft != null && currentScoreLeft != _connectionLeft.ConnectionScore)
            updatedConnections.Add((MazeTilePath)_connectionLeft);
        if (_connectionUp != null && currentScoreUp != _connectionUp.ConnectionScore)
            updatedConnections.Add((MazeTilePath)_connectionUp);

        return updatedConnections;
    }

    public List<TileObstacle> TileObstacle()
    {
        List<TileObstacle> updatedConnections = new List<TileObstacle>();

        int currentScoreThisTilePath = _thisMazeTileAttribute.ConnectionScore;
        int currentScoreRight = _connectionRight != null ? _connectionRight.ConnectionScore : -1;
        int currentScoreDown = _connectionDown != null ? _connectionDown.ConnectionScore : -1;
        int currentScoreLeft = _connectionLeft != null ? _connectionLeft.ConnectionScore : -1;
        int currentScoreUp = _connectionUp != null ? _connectionUp.ConnectionScore : -1;

        if (currentScoreThisTilePath == 2)
        {
            if (currentScoreRight == 7)
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(17);
                _connectionRight.ConnectionScore = _pickSpriteNumber(29);
            }
            else if (currentScoreRight == 22)
            {
                _connectionRight.ConnectionScore = _pickSpriteNumber(27);
            }
            else if (currentScoreRight == 29)
            {
                _connectionRight.ConnectionScore = _pickSpriteNumber(7);
            }
        }

        if (currentScoreThisTilePath == 3)
        {
            if (currentScoreDown == 10)
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(18);
                _connectionDown.ConnectionScore = _pickSpriteNumber(30);
            }
            else if (currentScoreDown == 24)
            {
                _connectionDown.ConnectionScore = _pickSpriteNumber(28);
            }
            else if (currentScoreDown == 30)
            {
                _connectionDown.ConnectionScore = _pickSpriteNumber(10);
            }
        }

        if (currentScoreThisTilePath == 4)
        {
            if (currentScoreLeft == 7)
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(19);
                _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
            }
            else if (currentScoreLeft == 22)
            {
                _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
            }
            else if (currentScoreLeft == 27)
            {
                _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
            }
        }

        if (currentScoreThisTilePath == 5)
        {
            if (currentScoreUp == 10)
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(20);
                _connectionUp.ConnectionScore = _pickSpriteNumber(28);
            }
            else if (currentScoreUp == 24)
            {
                _connectionUp.ConnectionScore = _pickSpriteNumber(30);
            }
            else if (currentScoreUp == 28)
            {
                _connectionUp.ConnectionScore = _pickSpriteNumber(10);
            }
        }

        if (currentScoreThisTilePath == 6)
        {
            if (currentScoreRight == 22 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 29 || currentScoreRight == 31 || currentScoreRight == 32 || currentScoreRight == 33)
            {
                if (currentScoreDown == 23 || currentScoreDown == 24 || currentScoreDown == 26 || currentScoreDown == 30 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                }
                else if (currentScoreDown == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreDown == 28)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                }
            }
            else if (currentScoreRight == 7)
            {
                if (currentScoreDown == 23 || currentScoreDown == 24 || currentScoreDown == 26 || currentScoreDown == 30 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreDown == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreDown == 28)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                }
            }
            else if (currentScoreRight == 27)
            {
                if (currentScoreDown == 23 || currentScoreDown == 24 || currentScoreDown == 26 || currentScoreDown == 30 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                }
                else if (currentScoreDown == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreDown == 28)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(21);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                }
            }
        }

        if (currentScoreThisTilePath == 8)
        {
            if (currentScoreRight == 22 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 29 || currentScoreRight == 31 || currentScoreRight == 32 || currentScoreRight == 33)
            {
                if (currentScoreUp == 21 || currentScoreUp == 24 || currentScoreUp == 25 || currentScoreUp == 28 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                }
                else if (currentScoreUp == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);

                }
                else if (currentScoreUp == 30)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
            }
            else if (currentScoreRight == 7)
            {
                if (currentScoreUp == 23 || currentScoreUp == 24 || currentScoreUp == 26 || currentScoreUp == 30 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreUp == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreUp == 30)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
            }
            else if (currentScoreRight == 27)
            {
                if (currentScoreUp == 23 || currentScoreUp == 24 || currentScoreUp == 26 || currentScoreUp == 30 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                }
                else if (currentScoreUp == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreUp == 30)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(23);
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
            }
        }

        if (currentScoreThisTilePath == 9)
        {
            if (currentScoreLeft == 21 || currentScoreLeft == 22 || currentScoreLeft == 23 || currentScoreLeft == 27 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {
                if (currentScoreDown == 23 || currentScoreDown == 24 || currentScoreDown == 26 || currentScoreDown == 30 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                }
                else if (currentScoreDown == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreDown == 28)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                }
            }
            else if (currentScoreLeft == 7)
            {
                if (currentScoreDown == 23 || currentScoreDown == 24 || currentScoreDown == 26 || currentScoreDown == 30 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreDown == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreDown == 28)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                }
            }
            else if (currentScoreLeft == 29)
            {
                if (currentScoreDown == 23 || currentScoreDown == 24 || currentScoreDown == 26 || currentScoreDown == 30 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                }
                else if (currentScoreDown == 10)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                }
                else if (currentScoreDown == 28)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(25);
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                }
            }
        }

        if (currentScoreThisTilePath == 11)
        {
            if (currentScoreUp == 21 || currentScoreUp == 24 || currentScoreUp == 25 || currentScoreUp == 28 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {
                if (currentScoreLeft == 21 || currentScoreLeft == 22 || currentScoreLeft == 23 || currentScoreLeft == 27 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                }
                else if (currentScoreLeft == 7)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreLeft == 29)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                }
            }
            else if (currentScoreUp == 10)
            {
                if (currentScoreLeft == 21 || currentScoreLeft == 22 || currentScoreLeft == 23 || currentScoreLeft == 27 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreLeft == 7)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreLeft == 29)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);
                }
            }
            else if (currentScoreUp == 30)
            {
                if (currentScoreLeft == 21 || currentScoreLeft == 22 || currentScoreLeft == 23 || currentScoreLeft == 27 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
                else if (currentScoreLeft == 7)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
                else if (currentScoreLeft == 29)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(26);
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
            }
        }

        if (currentScoreThisTilePath == 7)
        {
            if (currentScoreRight == 19)
            {
                _connectionRight.ConnectionScore = _pickSpriteNumber(4);
            }
            else if (currentScoreRight == 22)
            {
                _connectionRight.ConnectionScore = _pickSpriteNumber(27);
            }
            else if (currentScoreRight == 29)
            {
                _connectionRight.ConnectionScore = _pickSpriteNumber(7);
            }

            if (currentScoreLeft == 17)
            {
                _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
            }
            else if (currentScoreLeft == 22)
            {
                _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
            }
            else if (currentScoreLeft == 27)
            {
                _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
            }
        }

        if (currentScoreThisTilePath == 10)
        {
            if (currentScoreDown == 20)
            {
                _connectionDown.ConnectionScore = _pickSpriteNumber(5);
            }
            else if (currentScoreDown == 24)
            {
                _connectionDown.ConnectionScore = _pickSpriteNumber(28);
            }
            else if (currentScoreDown == 30)
            {
                _connectionDown.ConnectionScore = _pickSpriteNumber(10);
            }

            if (currentScoreUp == 18)
            {
                _connectionUp.ConnectionScore = _pickSpriteNumber(3);
            }
            else if (currentScoreUp == 24)
            {
                _connectionUp.ConnectionScore = _pickSpriteNumber(30);
            }
            else if (currentScoreUp == 28)
            {
                _connectionUp.ConnectionScore = _pickSpriteNumber(10);
            }
        }

        if (currentScoreThisTilePath == 16)
        {
            if (currentScoreRight == 7)
            {
                _connectionRight.ConnectionScore = _pickSpriteNumber(29);
            }
            else if (currentScoreLeft == 27)
            {
                _connectionRight.ConnectionScore = _pickSpriteNumber(22);
            }

            if (currentScoreDown == 10)
            {
                _connectionDown.ConnectionScore = _pickSpriteNumber(30);
            }
            else if (currentScoreDown == 28)
            {
                _connectionDown.ConnectionScore = _pickSpriteNumber(24);
            }

            if (currentScoreLeft == 7)
            {
                _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
            }
            else if (currentScoreLeft == 27)
            {
                _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
            }

            if (currentScoreUp == 10)
            {
                _connectionUp.ConnectionScore = _pickSpriteNumber(28);
            }
            else if (currentScoreUp == 30)
            {
                _connectionUp.ConnectionScore = _pickSpriteNumber(24);
            }
        }

        if (currentScoreThisTilePath == 17)
        {
            if (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 32 || currentScoreRight == 33 || currentScoreRight == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(2);

                if (currentScoreRight == 19)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(4);
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
            }
        }

        if (currentScoreThisTilePath == 18)
        {
            if (currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 25 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(3);

                if (currentScoreDown == 20)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(5);
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        if (currentScoreThisTilePath == 19)
        {
            if (currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(4);

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }
            }
        }

        else if (currentScoreThisTilePath == 20)
        {
            if (currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(5);

                if (currentScoreUp == 18)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(3);
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 21)
        {
            if (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(6);

                if (currentScoreRight == 19)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(4);
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreDown == 20)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(5);
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 22)
        {
            if ((currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33) &&
                (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34))
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(7);

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }
                else if ((currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33) && currentScoreRight != 25 && currentScoreRight != 26 && currentScoreRight != 31 && currentScoreRight != 33 && currentScoreRight != 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreRight == 19)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(4);
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if ((currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34) && currentScoreLeft != 21 && currentScoreLeft != 23 && currentScoreLeft != 31 && currentScoreLeft != 32 && currentScoreLeft != 33)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(7);
                }
            }
        }

        else if (currentScoreThisTilePath == 23)
        {
            if (currentScoreRight == 16 || currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(8);

                if (currentScoreRight == 19)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(4);
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(3);
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 24)
        {
            if ((currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34) &&
                (currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34))
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(10);

                if (currentScoreDown == 20)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(5);
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if ((currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34) && currentScoreUp != 21 && currentScoreUp != 25 && currentScoreUp != 31 && currentScoreUp != 32 && currentScoreUp != 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(3);
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if ((currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34) && currentScoreDown != 23 && currentScoreDown != 26 && currentScoreDown != 32 && currentScoreDown != 33 && currentScoreDown != 34)
                {
                    _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 25)
        {
            if (currentScoreDown == 16 || currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 33 || currentScoreLeft == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(9);

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreDown == 20)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(5);
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 26)
        {
            if (currentScoreLeft == 16 || currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 33 || currentScoreLeft == 34 ||
                currentScoreUp == 16 || currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 34)
            {

            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(11);

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(3);
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 27)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34)
            {
                if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }
            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(7);

                if (currentScoreRight == 19)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(4);
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(7);
                }
            }
        }

        else if (currentScoreThisTilePath == 28)
        {
            if (currentScoreDown == 23 || currentScoreDown == 26 || currentScoreUp == 31 || currentScoreDown == 33 || currentScoreDown == 34)
            {
                if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(10);

                if (currentScoreDown == 20)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(5);
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 29)
        {
            if (currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {
                if (currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(7);
                }
            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(7);

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }

                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }
            }
        }

        else if (currentScoreThisTilePath == 30)
        {
            if (currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 31 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }
            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(10);

                if (currentScoreUp == 18)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(3);
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 31)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33)
            {
                if (currentScoreRight == 4)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(19);
                }
                else if (currentScoreRight == 7)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreRight == 27)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                }

                if (currentScoreDown == 5)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(20);
                }
                else if (currentScoreDown == 10)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreDown == 28)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                }

                if (currentScoreLeft == 2)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(17);
                }
                else if (currentScoreLeft == 7)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreLeft == 29)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                }
            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(12);

                if (currentScoreRight == 19)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(4);
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreDown == 20)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(5);
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }
            }
        }

        else if (currentScoreThisTilePath == 32)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreRight == 4)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(19);
                }
                else if (currentScoreRight == 7)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreRight == 27)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                }

                if (currentScoreDown == 5)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(20);
                }
                else if (currentScoreDown == 10)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreDown == 28)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                }

                if (currentScoreUp == 3)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(18);
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreUp == 30)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(13);

                if (currentScoreRight == 19)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(4);
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreDown == 20)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(5);
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(3);
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 33)
        {
            if (currentScoreRight == 25 || currentScoreRight == 26 || currentScoreRight == 31 || currentScoreRight == 33 || currentScoreRight == 34 ||
                currentScoreLeft == 21 || currentScoreLeft == 2 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33 ||
                currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreRight == 4)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(19);
                }
                else if (currentScoreRight == 7)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreRight == 27)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(22);
                }

                if (currentScoreLeft == 2)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(17);
                }
                else if (currentScoreLeft == 7)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreLeft == 29)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                }

                if (currentScoreUp == 3)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(18);
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreUp == 30)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(14);

                if (currentScoreRight == 19)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(4);
                }
                else if (currentScoreRight == 22)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreRight == 29)
                {
                    _connectionRight.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(3);
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        else if (currentScoreThisTilePath == 34)
        {
            if (currentScoreDown == 23 || currentScoreDown == 26 || currentScoreDown == 32 || currentScoreDown == 33 || currentScoreDown == 34 ||
                currentScoreLeft == 21 || currentScoreLeft == 23 || currentScoreLeft == 31 || currentScoreLeft == 32 || currentScoreLeft == 33 ||
                currentScoreUp == 21 || currentScoreUp == 25 || currentScoreUp == 32 || currentScoreUp == 33 || currentScoreUp == 34)
            {
                if (currentScoreDown == 5)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(20);
                }
                else if (currentScoreDown == 10)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreDown == 28)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(24);
                }

                if (currentScoreLeft == 2)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(17);
                }
                else if (currentScoreLeft == 7)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(27);
                }
                else if (currentScoreLeft == 29)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(22);
                }

                if (currentScoreUp == 3)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(18);
                }
                else if (currentScoreUp == 10)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreUp == 30)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(24);
                }
            }
            else
            {
                _thisMazeTileAttribute.ConnectionScore = _pickSpriteNumber(15);

                if (currentScoreDown == 20)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(5);
                }
                else if (currentScoreDown == 24)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(28);
                }
                else if (currentScoreDown == 30)
                {
                    _connectionDown.ConnectionScore = _pickSpriteNumber(10);
                }

                if (currentScoreLeft == 17)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(2);
                }
                else if (currentScoreLeft == 22)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(29);
                }
                else if (currentScoreLeft == 27)
                {
                    _connectionLeft.ConnectionScore = _pickSpriteNumber(7);
                }

                if (currentScoreUp == 18)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(3);
                }
                else if (currentScoreUp == 24)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(30);
                }
                else if (currentScoreUp == 28)
                {
                    _connectionUp.ConnectionScore = _pickSpriteNumber(10);
                }
            }
        }

        if (currentScoreThisTilePath != _thisMazeTileAttribute.ConnectionScore)
            updatedConnections.Add((TileObstacle)_thisMazeTileAttribute);
        if (_connectionRight != null && currentScoreRight != _connectionRight.ConnectionScore)
            updatedConnections.Add((TileObstacle)_connectionRight);
        if (_connectionDown != null && currentScoreDown != _connectionDown.ConnectionScore)
            updatedConnections.Add((TileObstacle)_connectionDown);
        if (_connectionLeft != null && currentScoreLeft != _connectionLeft.ConnectionScore)
            updatedConnections.Add((TileObstacle)_connectionLeft);
        if (_connectionUp != null && currentScoreUp != _connectionUp.ConnectionScore)
            updatedConnections.Add((TileObstacle)_connectionUp);

        return updatedConnections;
    }
}