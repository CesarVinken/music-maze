using System.Collections.Generic;
using UnityEngine;

public class Pathfinding {

    private const int MOVE_STRAIGHT_COST = 10;

    public static Pathfinding Instance { get; private set; }
    public static List<PathNode> AllPathNodes = new List<PathNode>();
    private List<PathNode> openList;
    private List<PathNode> closedList;

    private Character _character;

    public Pathfinding(Character character) {
        Instance = this;
        _character = character;

        // currently availablePathNodes is ALL nodes. In the future, only add nodes from the areas in which the particular enemy is allowed to travel.
        for (int i = 0; i < GameManager.Instance.CurrentGameLevel.Tiles.Count; i++)
        {
            PathNode pathNode = GameManager.Instance.CurrentGameLevel.Tiles[i].PathNode;
            if (AllPathNodes.Contains(pathNode)){
                continue;
            }

            AllPathNodes.Add(GameManager.Instance.CurrentGameLevel.Tiles[i].PathNode);
        }
    }

    public List<PathNode> FindNodePath(GridLocation startGridLocation, GridLocation endGridLocation) {
        PathNode startNode = GameManager.Instance.CurrentGameLevel.TilesByLocation[startGridLocation].PathNode;
        PathNode endNode = GameManager.Instance.CurrentGameLevel.TilesByLocation[endGridLocation].PathNode;

        if (startNode == null || endNode == null) {
            // Invalid Path
            Logger.Log("path is invalid. Could not find a node.");
            return new List<PathNode>();
        }

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        // go through all nodes. LATER: go through all nodes of a particular area, to filter out number of available nodes.
        // Because it seems like currently a path from tile A in accessible zone to tile B in an accessible zone could be calculated outside of the area
        for (int x = 0; x < AllPathNodes.Count; x++)
        {
            PathNode pathNode = AllPathNodes[x];
            pathNode.gCost = 99999999;
            pathNode.CalculateFCost();
            pathNode.cameFromNode = null;
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();
        
        while (openList.Count > 0) {

            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode) {
                // Reached final node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (KeyValuePair<ObjectDirection, PathNode> item in GetNeighbourList(currentNode)) {
                PathNode neighbourNode = item.Value;
                if (closedList.Contains(neighbourNode)) continue;

                if (!neighbourNode.Tile.Walkable) {
                    closedList.Add(neighbourNode);
                    continue;
                }

                // restrictions for Enemies
                if(_character is EnemyCharacter)
                {
                    if (neighbourNode.Tile.TryGetPlayerOnly())
                    {
                        closedList.Add(neighbourNode);
                        continue;
                    }

                    //restrictions for walking on bridges in the right direction
                    if (!ValidateForBridge(currentNode, neighbourNode, item.Key))
                    {
                        continue;
                    }
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost) {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode)) {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // Out of nodes on the openList
        return new List<PathNode>();
    }

    private Dictionary<ObjectDirection, PathNode> GetNeighbourList(PathNode currentNode) {
        return currentNode.Tile.PathNodeNeighbours;
    }

    public PathNode GetNode(GridLocation gridLocation) {
        return GameManager.Instance.CurrentGameLevel.TilesByLocation[gridLocation].PathNode;
    }

    private List<PathNode> CalculatePath(PathNode endNode) {
        Logger.Log(Logger.Pathfinding, $"Calculate path to endnode {endNode.Tile.GridLocation.X}, {endNode.Tile.GridLocation.Y}");
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null) {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        Logger.Log(Logger.Pathfinding, $"found path with length of {path.Count}:");
        for (int i = 0; i < path.Count; i++)
        {
            Logger.Log(Logger.Pathfinding, $"path step: {path[i].Tile.GridLocation.X}, {path[i].Tile.GridLocation.Y}");
        }
        return path;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b) {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList) {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++) {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost) {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    public bool ValidateForBridge(PathNode currentNode, PathNode targetNode, ObjectDirection direction)
    {
        Tile targetTile = targetNode.Tile;
        Tile currentTile = currentNode.Tile;

        if (targetTile.Walkable)
        {
            BridgePiece bridgePieceOnCurrentTile = currentTile.TryGetBridgePiece();
            BridgePiece bridgePieceOnTarget = targetTile.TryGetBridgePiece(); // optimisation: keep bridge locations of the level in a separate list, so we don't have to go over all the tiles in the level

            // there are no bridges involved
            if (bridgePieceOnCurrentTile == null && bridgePieceOnTarget == null)
            {
                return true;
            }

            // Make sure we go in the correct bridge direction
            if (bridgePieceOnCurrentTile && bridgePieceOnTarget)
            {

                if (bridgePieceOnCurrentTile.BridgePieceDirection == BridgePieceDirection.Horizontal &&
                    bridgePieceOnTarget.BridgePieceDirection == BridgePieceDirection.Horizontal &&
                    (direction == ObjectDirection.Left || direction == ObjectDirection.Right))
                {
                    return true;
                }

                if (bridgePieceOnCurrentTile.BridgePieceDirection == BridgePieceDirection.Vertical &&
                    bridgePieceOnTarget.BridgePieceDirection == BridgePieceDirection.Vertical &&
                    (direction == ObjectDirection.Up || direction == ObjectDirection.Down))
                {
                    return true;
                }

                return false;
            }

            if ((bridgePieceOnCurrentTile?.BridgePieceDirection == BridgePieceDirection.Horizontal ||
                bridgePieceOnTarget?.BridgePieceDirection == BridgePieceDirection.Horizontal) &&
                (direction == ObjectDirection.Left || direction == ObjectDirection.Right))
            {
                return true;
            }

            if ((bridgePieceOnCurrentTile?.BridgePieceDirection == BridgePieceDirection.Vertical ||
                bridgePieceOnTarget?.BridgePieceDirection == BridgePieceDirection.Vertical) &&
                (direction == ObjectDirection.Up || direction == ObjectDirection.Down))
            {
                return true;
            }
            return false;
        }
        return false;
    }
}
