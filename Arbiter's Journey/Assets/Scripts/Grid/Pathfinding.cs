using DebugUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pathfinding", menuName = "ScriptableObjects/Pathfinding", order = 0)]
public class Pathfinding : ScriptableObject
{
    private SmartTileMap tilemap;

    private List<PathNode> openList;
    private List<PathNode> finalList;

    [SerializeField]
    private int DIAG_COST = 14;

    public void SetTilemap(SmartTileMap tilemap)
    {
        this.tilemap = tilemap;
    }

    /**
     * G = cost FROM START node
     * H = heuristic cost to reach END node
     * F = G + H
     */
    public List<PathNode> FindPath(SmartTile startTile, SmartTile endTile)
    {
        if (endTile.tilemap.name != tilemap.name) { return null; }

        PathNode startNode = new PathNode(startTile, tilemap);
        PathNode endNode = new PathNode(endTile, tilemap);

        openList = new List<PathNode> { startNode };
        finalList = new List<PathNode>();

        startNode.gCost = 0;
        startNode.hCost = CalculateHCost(startNode, endNode);
        startNode.CalcFCost();

        while(openList.Count > 0)
        {
            PathNode current = GetLowestFCostNode(openList);
            if (current.GetLocation() == endNode.GetLocation()) 
            {
                Interactable obj = endTile.GetObjectOnIt();
                if (obj != null) 
                {
                    if (!obj.IsCollectible())
                    {
                        List<PathNode> retPath = CalcPath(current.GetPreviousNode());
                        current.SetPreviousNode(null);
                        obj.Interact();
                        return retPath;
                    }

                    obj.Interact();
                }
                var path = CalcPath(current);
                DebugDraw.DebugPath(path);
                return path;
            }

            openList.Remove(current);
            finalList.Add(current);

            foreach (PathNode neighbor in current.GetNeighbors())
            {
                if (finalList.Contains(neighbor)) continue;

                // 100
                int tempG = CalculateHCost(current, neighbor);
                if (tempG <= neighbor.gCost)
                {
                    neighbor.SetPreviousNode(current);
                    neighbor.gCost = tempG;
                    neighbor.hCost = CalculateHCost(neighbor, endNode);
                    neighbor.CalcFCost();

                    if(!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        } // end of while

        // No path found!
        Debug.Log("Pathfinding: No path was found to " + endTile.ToString());
        return null;
    }

    private List<PathNode> CalcPath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode> ();
        path.Add(endNode);
        PathNode current = endNode;
        while (current.GetPreviousNode() != null) 
        {
            path.Add(current.GetPreviousNode());
            current = current.GetPreviousNode();
        }

        path.Reverse();

        return path;
    }

    private PathNode GetLowestFCostNode(List<PathNode> list)
    {
        if (list.Count == 1)
        {
            return list[0];
        }

        PathNode lowestFCostNode = list[0];
        for (int f = 1; f < list.Count; f++)
        {
            if (list[f].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = list[f];
            }
        }

        return lowestFCostNode;
    }

    // Distance cost to END node, used as H in F = G + H (where F is the deciding cost)
    // This ignores individual tile costs, obstacles, etc. 
    // TODO: Update this to incorporate individual tiles scores ?
    private int CalculateHCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.GetLocation().x - b.GetLocation().x);
        int yDistance = Mathf.Abs(a.GetLocation().y - b.GetLocation().y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        return DIAG_COST * Mathf.Min(xDistance, yDistance) +  b.GetMoveCost() * remaining;
    }

    // Clear the SetPreviousNode var in each node.
    internal void ClearPathNodes(List<PathNode> debugPath)
    {
        foreach (PathNode pathNode in debugPath)
        {
            pathNode.gCost = int.MaxValue;
            pathNode.SetPreviousNode(null);
        }
    }
}


