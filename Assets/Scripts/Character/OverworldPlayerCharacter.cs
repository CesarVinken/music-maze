using System.Collections.Generic;
using UnityEngine;

public class OverworldPlayerCharacter : PlayerCharacter
{
    private OverworldCharacterManager _characterManager;
    
    public override void Awake()
    {
        base.Awake();

        _characterManager = CharacterManager.Instance as OverworldCharacterManager;

        if (_characterManager == null) return;

        _characterManager.Players.Add(PlayerNumber, this);
    }

    public override void Start()
    {
        base.Start();

        //transform the player's starting tile and surrounding tiles
        InGameOverworldTile currentTile = GameManager.Instance.CurrentGameLevel.TilesByLocation[StartingPosition] as InGameOverworldTile;
        CurrentGridLocation = currentTile.GridLocation;
    }
}
