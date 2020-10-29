using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorPlayerExitTileAttribute : IEditorMazeTileAttribute
{
    public string Name { get => "Player Exit"; }

    public Sprite Sprite { get => null; }

    public EditorMazeTileAttributeType AttributeType { get => EditorMazeTileAttributeType.PlayerExit; }

    public void PlaceAttribute(Tile tile)
    {
        IMazeTileAttribute playerExit = (PlayerExit)tile.MazeTileAttributes.FirstOrDefault(attribute => attribute is PlayerExit);
        if (playerExit == null)
        {
            tile.PlacePlayerExit();
            return;
        }
        tile.RemovePlayerExit();
    }
}
