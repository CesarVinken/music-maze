using Pathfinding;

public class CharacterPath : AILerp
{
    public Character Character;

    public new void Awake()
    {
        base.Awake();
        Character = gameObject.GetComponentInParent<Character>();
    }

    public override void OnTargetReached()
    {
        Character.ReachLocomotionTarget();
    }

    public void OnPathCalculated(Path p)
    {
        Character.SetHasCalculatedTarget(true);
    }
}
