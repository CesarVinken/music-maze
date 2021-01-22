using UnityEngine;

public class EditorMazeTileTransformationTriggerer : IEditorMazeTileTransformationTriggerer
{
    public string Name { get => "Transformation Triggerer"; }

    public Sprite Sprite => throw new System.NotImplementedException();

    public EditorTile SelectedTile;

    public Sprite GetSprite()
    {
        return SpriteManager.Instance.DefaultDoor[0];
    }

    public void SetSelectedTile(EditorTile tile)
    {
        SelectedTile = tile;
    }

    public void UnsetSelectedTile()
    {
        Logger.Log("unset selected tile");
        SelectedTile = null;
    }

    public void HandleTransformationTriggerPlacement(EditorTile tile)
    {
        if(tile.Markable)
        {
            // if we have a tile selected, add the markable tile as triggerer
            if(SelectedTile != null)
            {
                Logger.Log($"TODO: add markable tile as registered transformation triggerer for tile at {SelectedTile.GridLocation.X},{SelectedTile.GridLocation.Y}");
                if (SelectedTile.TransformationTriggerers.Contains(tile))
                {
                    SelectedTile.TransformationTriggerers.Remove(tile);
                }
                else
                {
                    SelectedTile.TransformationTriggerers.Add(tile);
                }
                //TODO paint tile to indicate that it is a triggerer for the selected tile
            }
        }
        else 
        {
            SetSelectedTile(tile);
        }
    }
}
