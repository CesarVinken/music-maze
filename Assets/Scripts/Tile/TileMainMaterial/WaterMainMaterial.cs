using UnityEngine;

public class WaterMainMaterial : MonoBehaviour, ITileMainMaterial
{
    public Tile Tile;
    public string ParentId;

    public void SetTile(Tile tile)
    {
        Tile = tile;
        ParentId = tile.TileId;
    }
}
