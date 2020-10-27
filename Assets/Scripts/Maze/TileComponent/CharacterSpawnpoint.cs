﻿using UnityEngine;

public class CharacterSpawnpoint : MonoBehaviour
{
    public CharacterType CharacterType;
    public bool IsPlayer;
    public CharacterBlueprint CharacterBlueprint;
    public GridLocation GridLocation;

    public void Awake()
    {
        CharacterBlueprint = new CharacterBlueprint(CharacterType);

        RegisterSpawnpoint();
    }

    public void RegisterSpawnpoint()
    {
        GridLocation = GridLocation.VectorToGrid(transform.position);
        if (IsPlayer)
        {
            MazeLevelManager.Instance.Level.PlayerCharacterSpawnpoints.Add(this);
        } else
        {
            MazeLevelManager.Instance.Level.EnemyCharacterSpawnpoints.Add(this);
        }
    }
}
