using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{

    public List<Node> childNodeList;

    public Vector2Int leftBottom {get; set;}

    public Vector2Int leftTop {get; set;}

    public Vector2Int rightTop {get; set;}

    public Vector2Int rightBottom {get; set;}

    public Vector3 center {get; set;}

    public int layerIndex { get; set;} // Deep in tree
    
    public Node parent { get; set;}

    public Node(Node parentNode)
    {
        childNodeList = new List<Node>();
        this.parent = parentNode;

        if(parentNode != null)
        {
            parentNode.AddChild(this);
        }
    }

    public void AddChild(Node Node)
    {
        childNodeList.Add(Node);
    }

    public void RemoveChild(Node Node)
    {
        childNodeList.Remove(Node);
    }
}
