using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonNode : Node
{
    public DungeonNode(Node parentNode, int index, Vector2Int lBottom, Vector2Int rTop) : base(parentNode)
    {
        this.layerIndex = index;
        this.leftBottom = lBottom;
        // Debug.Log("leftBottom "+ leftBottom);
        this.rightTop = rTop;
        // Debug.Log("rightTop "+ rightTop);
        this.rightBottom = new Vector2Int(rTop.x, lBottom.y);
        // Debug.Log("rightBottom "+ rightBottom);
        this.leftTop = new Vector2Int(lBottom.x, rightTop.y);
        // Debug.Log("leftTop "+ leftTop);
        InitRoomCenter(lBottom, rTop);
    }

    private void InitRoomCenter(Vector2Int lBottom, Vector2Int rTop)
    {
        int x = (int)((rTop.x + lBottom.x) / 2);
        int z = (int)((rTop.y + lBottom.y) / 2);
        this.center = new Vector3(x, 0, z);
    }

    public int roomWidth { get => (int)(rightTop.x - leftBottom.x); }
    public int roomLength { get => (int)(rightTop.y - leftBottom.y); }
}
