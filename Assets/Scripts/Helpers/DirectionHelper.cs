public static class DirectionHelper
{
    public static Direction OppositeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Right:
                return Direction.Left;
            case Direction.Down:
                return Direction.Up;
            case Direction.Left:
                return Direction.Right;
            case Direction.Up:
                return Direction.Down;
            default:
                return Direction.Right;
        }
    }

    public static bool IsAlligningWithFerry(FerryDirection ferryDirection, Direction direction)
    {
        if (ferryDirection == FerryDirection.Horizontal)
        {
            if (direction == Direction.Left || direction == Direction.Right)
            {
                return true;
            }
            return false;
        }
        else
        {
            if (direction == Direction.Up || direction == Direction.Down)
            {
                return true;
            }
            return false;
        }
    }
}
