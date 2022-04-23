using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// This the main Generator, the topest module
/// It is the very beginning of this demo
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    public int levelWid;
    public int levelLen;
    public int minDungeonWid;
    public int minDungeonLen;
    public int maxCount;
    public int bridgeWidth;

    [Range(0.0f, 0.3f)]
    public float dungeonBottomCornerModifier;
    [Range(0.7f, 1.0f)]
    public float dungeonTopCornerMidifier;
    [Range(0, 2)]
    public int dungeonOffset;

    public NavMeshSurface navSurface;

    private Vector3[] Vers = {};

    private Vector3 AgentInitPos; 

    private List<DungeonNode> dungeonList = new List<DungeonNode>();

    void Start()
    {
        DestroyAllChildren();
        GenerateLevel();
        BuildNavMesh();
        InitAgentChanPos();
        InitMonsters();
    }

    /// <summary>
    /// Bake NavMesh on runtime since the level are gernated after the game begins
    /// </summary>
    private void BuildNavMesh()
    {
        navSurface.BuildNavMesh();
    }

    
    void GenerateLevel()
    {
        var listOfDungeons = CalculateDungeon(maxCount,
            minDungeonWid,
            minDungeonLen,
            dungeonBottomCornerModifier,
            dungeonTopCornerMidifier,
            dungeonOffset,
            bridgeWidth);
        
        GenerateFloor(listOfDungeons);
        GenerateAgentBornPos(listOfDungeons);
    }


    /// <summary>
    /// Get the all the dungeons and bridges that as the connectioin of dungeons
    /// </summary>
    public List<Node> CalculateDungeon(int maxDunCount, int minWidth, int minLength, float roomBottomCornerModifier, float roomTopCornerMidifier, int roomOffset, int bridgeWidth)
    {
        // BSP
        BinarySpacePartition bsp = new BinarySpacePartition(levelWid, levelLen);

        List<DungeonNode> allNodeSet = new List<DungeonNode>();
        allNodeSet = bsp.PrepareNodesColletion(maxDunCount, minWidth, minLength);

        //Get all leaf nodes as the dungeon node
        List<Node> roomSpaces = Helper.GetTreeLeaves(bsp.rootNode);

        //Generator of each Dungeons
        dungeonList = GenerateDungeonsInGivenSpaces(roomSpaces, roomBottomCornerModifier, roomTopCornerMidifier, roomOffset);

        //Generator of passway that connect dungeons
        var vbridgeList = GenerateVBridgeNew(dungeonList, bridgeWidth);
        var hbridgeList = GenerateHBridgeNew(dungeonList, bridgeWidth);

        var allList = new List<Node>(dungeonList).Concat(vbridgeList).ToList().Concat(hbridgeList).ToList();
        // var allList = new List<Node>(dungeonList).Concat(hbridgeList).ToList();
        return allList;
    }
    
    /// <summary>
    /// GenerateDungeons in the given space, random shorter and smaller than given space
    /// </summary>
    public List<DungeonNode> GenerateDungeonsInGivenSpaces(List<Node> roomSpaces, float roomBottomCornerModifier, float roomTopCornerMidifier, int roomOffset)
    {
        List<DungeonNode> resList = new List<DungeonNode>();
        foreach (var space in roomSpaces)
        {
            Vector2Int newBottomLeftNode = Helper.GenerateLefBottomBetween(
                space.leftBottom, space.rightTop, roomBottomCornerModifier, roomOffset);

            Vector2Int newTopRightNode = Helper.GenerateRightTopBetween(
                space.leftBottom, space.rightTop, roomTopCornerMidifier, roomOffset);

            space.leftBottom = newBottomLeftNode;
            space.rightTop = newTopRightNode;
            space.rightBottom = new Vector2Int(newTopRightNode.x, newBottomLeftNode.y);
            space.leftTop = new Vector2Int(newBottomLeftNode.x, newTopRightNode.y);

            resList.Add((DungeonNode)space);
                
        }
        return resList;
    }

    /// <summary>
    /// Generate BridgeNode between two closest dongeon when they have intersection on X axis
    /// </summary>
    public List<Node> GenerateHBridgeNew(List<DungeonNode> allNodesCollection, int bridgeWidth)
    {
        List<Node> bridgeList = new List<Node>();
        Queue<DungeonNode> nodesQue = new Queue<DungeonNode>(allNodesCollection.OrderByDescending(node => node.layerIndex).ToList());

        while(nodesQue.Count > 0){
            DungeonNode curNode = nodesQue.Dequeue();

            List<Node> tempList = new List<Node>();
            foreach (var tarNode in nodesQue)
            {
                BridgeNode bridge;

                if(HasXCoordIntersection(curNode, tarNode))
                {
                    bridge = GenerateBridgeNode(curNode, tarNode, true);
                    tempList.Add(bridge);
                }
            }

            tempList.Sort(delegate(Node node1, Node node2)
            {
                int distance1;
                int distance2;

                if(node1.center.z < curNode.center.z){
                    distance1 = Mathf.Abs(curNode.leftBottom.y - node1.rightTop.y );
                }else{
                    distance1 = Mathf.Abs(curNode.rightTop.y - node1.leftBottom.y);
                }

                if(node2.center.z < curNode.center.z){
                    distance2 = Mathf.Abs(curNode.leftBottom.y - node2.rightTop.y );
                }else{
                    distance2 = Mathf.Abs(curNode.rightTop.y - node2.leftBottom.y);
                }

                if(distance1 < distance2){
                    return 1;
                }else{
                    return -1;
                }
            });

            if(tempList.Count > 0)
            {
                bridgeList.Add(tempList[0]);
            }
        }

        // List<Node> resList = bridgeList.Where((x,i)=>bridgeList.FindIndex(z=>z.leftBottom.y == x.leftBottom.y) == i).ToList();
        return bridgeList;
    }

    /// <summary>
    /// Generate BridgeNode between two closest dongeon when they have intersection on Y axis
    /// </summary>
    public List<Node> GenerateVBridgeNew(List<DungeonNode> allNodesCollection, int bridgeWidth)
    {
        List<Node> bridgeList = new List<Node>();
        Queue<DungeonNode> nodesQue = new Queue<DungeonNode>(allNodesCollection.OrderByDescending(node => node.layerIndex).ToList());

        while(nodesQue.Count > 0){
            DungeonNode curNode = nodesQue.Dequeue();

            List<Node> tempList = new List<Node>();
            foreach (var tarNode in nodesQue)
            {
                BridgeNode bridge;

                if(HasYCoordIntersection(curNode, tarNode))
                {
                    bridge = GenerateBridgeNode(curNode, tarNode, false);
                    tempList.Add(bridge);
                }
            }

            tempList.Sort(delegate(Node node1, Node node2)
            {
                int distance1;
                int distance2;

                if(node1.center.x < curNode.center.x){
                    distance1 = Mathf.Abs(curNode.leftBottom.x - node1.rightTop.x );
                }else{
                    distance1 = Mathf.Abs(curNode.rightTop.x - node1.leftBottom.x);
                }

                if(node2.center.x < curNode.center.x){
                    distance2 = Mathf.Abs(curNode.leftBottom.x - node2.rightTop.x );
                }else{
                    distance2 = Mathf.Abs(curNode.rightTop.x - node2.leftBottom.x);
                }

                if(distance1 < distance2){
                    return 1;
                }else{
                    return -1;
                }
            });

            if(tempList.Count > 0)
            {
                bridgeList.Add(tempList[0]);
            }
        }

        // List<Node> resList = bridgeList.Where((x,i)=>bridgeList.FindIndex(z=>z.leftBottom.y == x.leftBottom.y) == i).ToList();
        return bridgeList;
    }

    BridgeNode GenerateBridgeNode(DungeonNode curNode, DungeonNode tarNode, bool isHorizontal)
    {   
        int c1, c2, c3, c4;
        if(isHorizontal)
        {
            c1 = curNode.leftBottom.x;
            c2 = curNode.rightTop.x;
            c3 = tarNode.leftBottom.x;
            c4 = tarNode.rightTop.x;
        }else
        {
            c1 = curNode.leftBottom.y;
            c2 = curNode.rightTop.y;
            c3 = tarNode.leftBottom.y;
            c4 = tarNode.rightTop.y;
        }

        List<int> coords = new List<int>{c1, c2, c3, c4};

        coords.Sort();
        Vector2Int cRange = new Vector2Int(coords[1], coords[2]); // get the two middle index value
        
        int randomNum = UnityEngine.Random.Range(cRange.x, cRange.y);
        int randomNum2 = randomNum + bridgeWidth; // the bridge width it is 

        Vector2Int lb = new Vector2Int();
        Vector2Int rt = new Vector2Int();

        if(isHorizontal)
        {
            lb.x = randomNum;
            rt.x = randomNum2;

            if(curNode.leftBottom.y > tarNode.rightTop.y)
            {
                lb.y = tarNode.rightTop.y;
                rt.y = curNode.leftBottom.y;
            }else if(curNode.rightTop.y < tarNode.leftBottom.y)
            {
                lb.y = curNode.rightTop.y;
                rt.y = tarNode.leftBottom.y;
            }
        }else
        {
            lb.y = randomNum;
            rt.y = randomNum2;

            if(curNode.leftBottom.x > tarNode.rightTop.x)
            {
                lb.x = tarNode.rightTop.x;
                rt.x = curNode.leftBottom.x;
            }else if(curNode.rightTop.x < tarNode.leftBottom.x)
            {
                lb.x = curNode.rightTop.x;
                rt.x = tarNode.leftBottom.x;
            }
        }

        BridgeNode newBridge = new BridgeNode(null, lb, rt, 3);
        return newBridge;
    }

    bool HasXCoordIntersection(DungeonNode curNode, DungeonNode tarNode)
    {
        // BridgeNode bNode = new BridgeNode(curNode, tarNode, 5);
        if(curNode.leftBottom.x >= tarNode.rightTop.x || curNode.rightTop.x <= tarNode.leftBottom.x)
        {
            return false;
        }
        else{
            return true;
        }
    }

    bool HasYCoordIntersection(DungeonNode curNode, DungeonNode tarNode)
    {
        // BridgeNode bNode = new BridgeNode(curNode, tarNode, 5);
        if(curNode.leftBottom.y >= tarNode.rightTop.y || curNode.rightTop.y <= tarNode.leftBottom.y)
        {
            return false;
        }
        else{
            return true;
        }
    }

    /// <summary>
    /// Move Agent at the first dungeon in the list
    /// </summary>
    /// <param name="listOfDungeons"></param>
    void GenerateAgentBornPos(List<Node> listOfDungeons){
        Vector3 point = listOfDungeons[0].center;
        AgentInitPos = point;
    }

    void InitAgentChanPos()
    {
        GameObject AgentPlayer = GameObject.Find("AgentChan");
        if(AgentPlayer != null){
            AgentPlayer.transform.position = AgentInitPos;
        }
    }

    void InitMonsters()
    {
        MonsterGenerator MonsterGenerator = GameObject.Find("MonsterGenerator").GetComponent<MonsterGenerator>();
        MonsterGenerator.GenerateMonsters(dungeonList);
    }

    /// <summary>
    /// Gernerate Floor mesh by listOfDungeons, each dungeons has a mesh
    /// </summary>
    /// <param name="listOfDungeons"></param>
    private void GenerateFloor(List<Node> listOfDungeons)
    {
        GameObject modelRoot = new GameObject("modelRoot");
        modelRoot.transform.parent = transform;

        for (int i = 0; i < listOfDungeons.Count; i++)
        {
            CreateFloorMesh(listOfDungeons[i]);
        }
        // CreateFloorMesh(listOfDungeons[0].leftBottom, listOfDungeons[0].rightTop);
    }

    void CreateFloorMesh(Node roomNode)
    {
        //Debug.Log("Create Mesh " + leftBottom + rightTop);
        // Vertice array for 4 corner areas
        Vector2 leftBottom = roomNode.leftBottom; 
        Vector2 rightTop = roomNode.rightTop;
        Vector3[] vertices = CaculateVertices(leftBottom, rightTop);
        Vector2[] uvs = CaculateUVs(vertices);
        int[] triangles = CaculateTriangles(leftBottom, rightTop);

        Vers = Vers.Concat(vertices).ToArray();
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GameObject dungeonFloor = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer));

        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        // dungeonFloor.GetComponent<MeshRenderer>().material = material;
        dungeonFloor.AddComponent<MeshCollider>();
        dungeonFloor.AddComponent<ChangeTextureColor>();
        dungeonFloor.transform.parent = transform;
    }

/// <summary>
/// Caluate Vertices for the mesh
/// The y coord is generate by PerlinNoise which makes each vertices has different heightness when it's not on the boundary 
/// All boundary has the same 0.5 height to make them connect to each other
/// </summary>
    Vector3[] CaculateVertices(Vector2 lb, Vector2 rt){
        int wid = (int)(rt.x - lb.x);
        int len = (int)(rt.y - lb.y);
        Vector3[] vers = new Vector3[(wid + 1) * (len + 1)];
        // Debug.Log("Length " + vers.Length);
        int i = 0;

        for (int z = 0; z <= len; z++)
        {
            for (int x = 0; x <= wid; x++)
            {
                bool isEdge = z==0 || z == len || x == 0 || x == wid;
                bool isWithPerlinNoise = !isEdge;
                float perlinY = isWithPerlinNoise ? Mathf.PerlinNoise(x * 0.3f, z*0.3f) * 2.0f: 0.5f; //Use PerlinNoise Or not
                Vector3 v = new Vector3(lb.x + x, perlinY, lb.y + z);
                vers[i] = v;
                // Debug.Log("i "+ i);
                i++;
            }
        }
        // Vers = vers;

        return vers;
    }

    Vector2[] CaculateUVs(Vector3[] vertices)
    {
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        return uvs;
    }

/// <summary>
/// Caculate triangles array which value saves the index of vertices array
/// One triangle is made by 3 vertices, we caculate 6 vertices index in each iterate around 
/// so that we get 2 triangles a around
/// </summary>
    int[] CaculateTriangles(Vector2 lb, Vector2 rt)
    {
        int wid = (int)(rt.x - lb.x);
        int len = (int)(rt.y - lb.y);
        int[] triangles = new int[(wid+1) * (len+1) * 6];

        int t = 0;
        int v = 0;
        for (int z = 0; z < len; z++)
        {

            for (int i = 0; i < wid; i++)
            {
                triangles[t] = v;
                triangles[t+1] = v + wid + 1;
                triangles[t+2] = v + 1;
                triangles[t+3] = v + 1;
                triangles[t+4] = v + wid + 1;
                triangles[t+5] = v + wid + 2;
                
                v++;
                t += 6;
            }
            v++;
        }

        return triangles;
    }

/// <summary>
/// Change texture color to make the terrain more visible
/// </summary>
    private void GenerateTexture()
    {
        int width = 256;
        int height = 256;

        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color cr =  GetColor(x, y);

                texture.SetPixel(x, y, cr);
            }
        }

        texture.Apply();
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = texture;
    }

    private Color GetColor(int x, int y)
    {
        float r = x / 256;
        float g = y / 256;
        float b = x / 256 + y / 256;
        return new Color(r, g, b);
    }
/// <summary>
/// Make the vertices visible
/// </summary>
    private void OnDrawGizmos() {
        if(Vers == null){
            return;
        }    

        for (int i = 0; i < Vers.Length; i++)
        {
            Gizmos.DrawSphere(Vers[i], 0.1f);
        }
    }

    private void DestroyAllChildren()
    {
        while(transform.childCount != 0)
        {
            foreach(Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }
}
