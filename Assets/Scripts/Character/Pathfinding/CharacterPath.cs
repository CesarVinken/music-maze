using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;

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

    public List<GraphNode> GetNodesOfPath()
    {
        // Note: Doesn't do any bounds checking, you should probably add that
        // Also only works if you don't use any modifiers that e.g smooth or simplify the path

        for (int i = 0; i < path.path.Count; i++)
        {
            GraphNode nextNodee = path.path[i];
            //Logger.Warning($"The node {i} on the path is {((Vector3)nextNodee.position).x}, {((Vector3)nextNodee.position).y}");
        }
        //GraphNode nextNode = path.path[interpolator.segmentIndex + 1];
        //Logger.Warning($"The next node on the path is {((Vector3)nextNode.position).x}, {((Vector3)nextNode.position).y}");
        return path.path;
    }
}
