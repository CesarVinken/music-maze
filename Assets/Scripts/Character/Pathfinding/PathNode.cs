using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode {

    public Tile Tile;

    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public PathNode cameFromNode;

    public void CalculateFCost() {
        fCost = gCost + hCost;
    }

    public override string ToString() {
        return $"{x}, {y}";
    }

    public void SetTile(Tile tile)
    {
        Tile = tile;
        x = tile.GridLocation.X;
        y = tile.GridLocation.Y;
    }
}
