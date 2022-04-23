using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinarySpacePartition
{
    public DungeonNode rootNode;

    public DungeonNode RootNode { get => rootNode;}

    //The rootNode of level is the entire rectangle
    //so it has null parent
    //and the index will be the zero of course
    //set levelWidth as its width, and levelLength as the length
    public BinarySpacePartition(int levelWidth, int levelLength)
    {
        this.rootNode = new DungeonNode(null, 0, new Vector2Int(0, 0), new Vector2Int(levelWidth, levelLength));
    }


    public List<DungeonNode> PrepareNodesColletion(int maxIterations, int minRoomWid, int minRoomLen)
    {
        Queue<DungeonNode> graph = new Queue<DungeonNode>();
        List<DungeonNode> listToReturn = new List<DungeonNode>();
        graph.Enqueue(this.rootNode);
        listToReturn.Add(this.rootNode);
        int iteritions = 0;
        while(iteritions < maxIterations && graph.Count > 0)
        {
            iteritions++;
            DungeonNode curNode = graph.Dequeue();
            if(curNode.roomWidth >= minRoomWid * 2 || curNode.roomLength >= minRoomLen * 2)
            {
                SpacePartition(graph, listToReturn, curNode, minRoomWid, minRoomLen ); //
            }
        }

        return listToReturn;
    }

    private void SpacePartition(Queue<DungeonNode> graph, List<DungeonNode> listToReturn, DungeonNode curNode, int minRoomWid, int minRoomLen )
    {
        SplitLine wallLine = GetWallDividSpace(
            curNode.leftBottom,
            curNode.rightTop,
            minRoomWid,
            minRoomLen
        );

        Vector2Int p1leftBottom, p1RightTop, p2LeftBottom, p2RightTop;
        
        p1leftBottom = curNode.leftBottom;
        p2RightTop = curNode.rightTop;

        if(wallLine.Orientation == Orientation.Horizontal)
        {            
            p1RightTop = new Vector2Int(curNode.rightTop.x, wallLine.Coordinates.y);
            p2LeftBottom = new Vector2Int(curNode.leftBottom.x, wallLine.Coordinates.y);
        }else
        {
            p1RightTop = new Vector2Int(wallLine.Coordinates.x, curNode.rightTop.y);
            p2LeftBottom = new Vector2Int(wallLine.Coordinates.x, curNode.leftBottom.y);

        }

        DungeonNode p1 =  new DungeonNode(curNode, curNode.layerIndex + 1, p1leftBottom, p1RightTop);
        DungeonNode p2 =  new DungeonNode(curNode, curNode.layerIndex + 1, p2LeftBottom, p2RightTop);
        
        AddNewChild(listToReturn, graph, p1);
        AddNewChild(listToReturn, graph, p2);
    }

    private void AddNewChild(List<DungeonNode> listToReturn, Queue<DungeonNode> graph, DungeonNode node)
    {
        listToReturn.Add(node);
        graph.Enqueue(node);
    }

    private SplitLine GetWallDividSpace(Vector2Int leftBottom, Vector2Int rightTop, int minRoomWid, int minRoomLen)
    {
        Orientation orient;
        bool widStatus = (rightTop.x - leftBottom.x) >= 2 * minRoomWid;
        bool lenStatus = (rightTop.y - leftBottom.y) >= 2 * minRoomLen;
        if(widStatus && lenStatus)
        {
            orient = (Orientation)(Random.Range(0,2)); //Selet a random number from 0 or 1, and set it as orientaion
        }else if(widStatus)
        {
            orient = Orientation.Vertical;
        }else
        {
            orient = Orientation.Horizontal;
        }

        Vector2Int coord = GetCoordinatesFororientation(orient, leftBottom, rightTop, minRoomWid, minRoomLen);
        SplitLine resWall = new SplitLine(orient, coord);

        return resWall;
    }

    private Vector2Int GetCoordinatesFororientation(Orientation orientation, Vector2Int leftBottom, Vector2Int rightTop, int minRoomWid, int minRoomLen)
    {
        Vector2Int coord = Vector2Int.zero;
        int coordX = 0;
        int coordY = 0;

        if(orientation == Orientation.Horizontal)
        {
            int lbLength = leftBottom.y + minRoomLen;
            int rtLength = rightTop.y - minRoomLen;
            coordY = Random.Range(lbLength, rtLength);
        }else
        {
            int lbLength = leftBottom.x + minRoomWid;
            int rtLength = rightTop.x - minRoomWid;
            coordX = Random.Range(lbLength, rtLength);
        }

        coord = new Vector2Int(coordX, coordY);

        return coord;
    }
}
