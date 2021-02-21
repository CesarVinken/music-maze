using UnityEngine;

public class InGameOverworldTile : OverworldTile
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Walkable) return;

        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        if (player != null)
        {
            //Logger.Log("{0} entered tile {1},{2}", player.name, GridLocation.X, GridLocation.Y);
            if (GameManager.GameType == GameType.Multiplayer && !player.PhotonView.IsMine) return;

            player.UpdateCurrentGridLocation(GridLocation);
        }
    }
}
