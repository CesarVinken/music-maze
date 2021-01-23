using System;

[Serializable]
public class SerialisableGridLocation
{
    public int X;
    public int Y;

    public SerialisableGridLocation(int x, int y)
    {
        X = x;
        Y = y;
    }
}
