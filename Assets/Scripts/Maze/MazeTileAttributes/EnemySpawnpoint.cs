using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnpoint : CharacterSpawnpoint
{
    public override void RegisterSpawnpoint()
    {
        GridLocation = GridLocation.VectorToGrid(transform.position);

        MazeLevelManager.Instance.Level.EnemyCharacterSpawnpoints.Add(this);
    }
}
