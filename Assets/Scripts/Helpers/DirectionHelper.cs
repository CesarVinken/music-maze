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

    public static Direction GetDirectionFromGridLocation(GridLocation thisLocation, GridLocation thatLocation)
    {
        try
        {
            if (thatLocation.X > thisLocation.X) return Direction.Right;
            if (thatLocation.X < thisLocation.X) return Direction.Left;
            if (thatLocation.Y < thisLocation.Y) return Direction.Down;
            if (thatLocation.Y > thisLocation.Y) return Direction.Up;

            throw new System.Exception();
        }
        catch (System.Exception)
        {
            Logger.Error($"thisLocation at {thisLocation.X} {thisLocation.Y} is not position in any expected direction to thatLocation at {thatLocation.X} {thatLocation.Y}");
            throw;
        }
    }
}
