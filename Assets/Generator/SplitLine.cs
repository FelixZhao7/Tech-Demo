using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Orientation
{
    Horizontal = 0,
    Vertical = 1
}

public class SplitLine
{
    Orientation orientation;
    Vector2Int coordinates;
    
    public Orientation Orientation {get => orientation; set => orientation = value;}

    public Vector2Int Coordinates {get => coordinates; set => coordinates = value;}

    public SplitLine(Orientation orient, Vector2Int coord)
    {
        this.orientation = orient;
        this.coordinates = coord;
    }

}