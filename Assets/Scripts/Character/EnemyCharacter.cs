using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    public CharacterType CharacterType;

    public void CatchPlayer(PlayerCharacter playerCharacter)
    {
        playerCharacter.ResetCharacterPosition();
    }
}
