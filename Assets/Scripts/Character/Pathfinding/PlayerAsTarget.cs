public class PlayerAsTarget
{
    public PlayerCharacter TargettedPlayer { get; private set; }
    public GridLocation TargetGridLocation { get; private set; }

    public PlayerAsTarget(PlayerCharacter playerCharacter, GridLocation targetLocation)
    {
        TargettedPlayer = playerCharacter;
        TargetGridLocation = targetLocation;
    }
}