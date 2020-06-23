using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    public KeyboardInput KeyboardInput = KeyboardInput.None;
    public int PlayerNoInGame = 1;  // in multiplayer on multiple computers there can be a player 1 and player 2, while both use their Player1 keyboard input

    public void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyCharacter enemy = collision.gameObject.GetComponent<EnemyCharacter>();
        if (enemy != null)
        {
            Logger.Log("Collider enter player!");
            enemy.CatchPlayer(this);
        }
    }
}
