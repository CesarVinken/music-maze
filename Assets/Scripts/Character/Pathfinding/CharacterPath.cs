using Pathfinding;
using System;

public class CharacterPath : AILerp
{
    public Character Character;
    public event Action CharacterReachesTarget;

    public new void Awake()
    {
        base.Awake();
        Character = gameObject.GetComponentInParent<Character>();
    }

    public override void OnTargetReached()
    {
        CharacterReachesTarget?.Invoke();
    }

    public void OnPathCalculated(Path p)
    {
        Character.IsCalculatingPath = false; // Move to Character.function
        Character.SetHasCalculatedTarget(true);
    }
}
