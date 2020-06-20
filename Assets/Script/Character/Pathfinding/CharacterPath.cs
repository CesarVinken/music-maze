﻿using Pathfinding;

public class CharacterPath : AILerp
{
    public CharacterLocomotion CharacterLocomotion;

    public new void Awake()
    {
        base.Awake();
        CharacterLocomotion = gameObject.GetComponentInParent<CharacterLocomotion>();
    }

    public override void OnTargetReached()
    {
        CharacterLocomotion.ReachLocomotionTarget();
    }
}
