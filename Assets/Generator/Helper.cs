using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    /// <summary>
    /// Traverse the all nodes of tree from the given parenet node, and return lowest leaves nodes
    /// </summary>
    public static List<Node> GetTreeLeaves(Node parenetNode)
    {
        List<Node> resList = new List<Node>();

        Queue<Node> pointQue = new Queue<Node>();

        if (parenetNode.childNodeList.Count == 0)
        {
            return new List<Node>() { parenetNode };
        }

        foreach (var child in parenetNode.childNodeList)
        {
            pointQue.Enqueue(child);
        }

        while (pointQue.Count > 0)
        {
            var currentNode = pointQue.Dequeue();
            if (currentNode.childNodeList.Count == 0)
            {
                resList.Add(currentNode);
            }
            else
            {
                foreach (var child in currentNode.childNodeList)
                {
                    pointQue.Enqueue(child);
                }
            }
        }

        return resList;
    }


    //Genatera top right after cut off some random space
    public static Vector2Int GenerateRightTopBetween(
        Vector2Int boundaryLeftNode, Vector2Int boundaryRightNode, float pointModifier, int offset)
    {
        int minX = boundaryLeftNode.x + offset;
        int maxX = boundaryRightNode.x - offset;
        int minY = boundaryLeftNode.y + offset;
        int maxY = boundaryRightNode.y - offset;
        return new Vector2Int(
            Random.Range((int)(minX+(maxX-minX)*pointModifier),maxX),
            Random.Range((int)(minY+(maxY-minY)*pointModifier),maxY)
            );
    }

    //Genatera leftBootem after cut off some random space
    public static Vector2Int GenerateLefBottomBetween(
    Vector2Int boundaryLeftNode, Vector2Int boundaryRightNode, float pointModifier, int offset)
    {
        int minX = boundaryLeftNode.x + offset;
        int maxX = boundaryRightNode.x - offset;
        int minY = boundaryLeftNode.y + offset;
        int maxY = boundaryRightNode.y - offset;
        return new Vector2Int(
            Random.Range(minX, (int)(minX + (maxX - minX) * pointModifier)),
            Random.Range(minY, (int)(minY + (minY - minY) * pointModifier)));
    }

    public static Vector2Int GetMiddleNodeBetween(Vector2Int node1, Vector2Int node2)
    {
        int x = (node1.x + node2.x) / 2;
        int y = (node1.y + node2.y) / 2;
        Vector2Int midNode = new Vector2Int((int)x, (int)y); 
        return midNode;
    }
}

public enum RelativePosition
{
    Up,
    Down,
    Right,
    Left
}