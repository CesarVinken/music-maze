using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnpoint : CharacterSpawnpoint
{
    public override void RegisterSpawnpoint()
    {
        GridLocation = GridLocation.VectorToGrid(transform.position);

        MazeLevelManager.Instance.Level.PlayerCharacterSpawnpoints.Add(this);
    }
}
