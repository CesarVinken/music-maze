using Character;
using System.Collections.Generic;
using UnityEngine;

public class InGameMazeTile : MazeTile
{
    [SerializeField] private List<InGameMazeTile> _tilesToTransform = new List<InGameMazeTile>();

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Walkable) return;

        MusicInstrumentCase musicInstrumentCase = TryGetMusicInstrumentCase();
        MazePlayerCharacter player = collision.gameObject.GetComponent<MazePlayerCharacter>();

        if (player != null)
        {
            if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && !player.PhotonView.IsMine) return;

            // Try to mark the tile
            if (PlayerMarkRenderer.sprite == null && Markable)
            {
                MazeLevelGameplayManager.Instance.SetTileMarker(this, player);
            }

            if (musicInstrumentCase != null)
            {
                MazeLevelGameplayManager.Instance.PlayerCollisionWithMusicInstrumentCase(this, player, musicInstrumentCase);
            }
        }
        else
        {
            if (musicInstrumentCase != null)
            {
                EnemyCharacter enemy = collision.gameObject.GetComponent<EnemyCharacter>();

                if (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && !enemy.PhotonView.IsMine) return;

                MazeLevelGameplayManager.Instance.EnemyCollisionWithMusicInstrumentCase(this, enemy, musicInstrumentCase);
            }
        }
    }

    public void AddTilesToTransform(List<InGameMazeTile> tilesToTransform)
    {
        _tilesToTransform = tilesToTransform;
    }

    // Once the tile is marked, trigger the transformation of all tiles set up for this tile in the TilesToTransform list
    public void TriggerTransformations()
    {
        if (TransformationState == TransformationState.Bleak)
            TriggerTransformationOnSelf();

        for (int i = 0; i < _tilesToTransform.Count; i++)
        {
            InGameMazeTile tileToTransform = _tilesToTransform[i];

            if (tileToTransform.TransformationState == TransformationState.Colourful)
                continue;

            tileToTransform.TriggerTransformationOnSelf();
        }
    }

    public void TriggerTransformationOnSelf()
    {
        for (int i = 0; i < _tileAttributes.Count; i++)
        {
            ITransformable attribute = _tileAttributes[i] as ITransformable;
            if (attribute is BridgeEdge) continue; // Bridge edges should not be transformed with the tile, but with the connected bridge piece

            if (attribute != null)
            {
                attribute.TriggerTransformation();
            }
        }

        for (int j = 0; j < _tileBackgrounds.Count; j++)
        {
            ITransformable background = _tileBackgrounds[j] as ITransformable;
            if (background != null)
            {
                background.TriggerTransformation();
            }
        }

        TransformationState = TransformationState.Colourful;
    }
}