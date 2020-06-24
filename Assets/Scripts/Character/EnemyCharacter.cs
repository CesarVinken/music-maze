using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    public CharacterType CharacterType;

    public void CatchPlayer(PlayerCharacter playerCharacter)
    {
        float freezeTime = 2.0f;
        IEnumerator coroutine = FreezePlayer(playerCharacter, freezeTime);
        StartCoroutine(coroutine);
    }

    private IEnumerator FreezePlayer(PlayerCharacter playerCharacter, float freezeTime)
    {
        playerCharacter.CharacterLocomotion.IsFrozen = true;

        playerCharacter.ResetCharacterPosition();
        yield return new WaitForSeconds(freezeTime);

        playerCharacter.CharacterLocomotion.IsFrozen = false;

    }
}
