using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BridgeNode : Node
{
    public BridgeNode(Node parentNode, Vector2Int lBottom, Vector2Int rTop, int bridgeWidth) : base(parentNode)
    {
        this.leftBottom = lBottom;
        this.rightTop = rTop;
        this.rightBottom = new Vector2Int(rTop.x, lBottom.y);
        this.leftTop = new Vector2Int(lBottom.x, rightTop.y);

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