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
        Character.ReachTarget();
    }

    public void OnPathCalculated(Path p)
    {
        Logger.Log("Path count: " + p.path.Count);
        Character.IsCalculatingPath = false; // Move to Character.function
        Character.SetHasCalculatedTarget(true);
    }
}
