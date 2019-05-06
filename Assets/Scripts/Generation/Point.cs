using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Point
{
    public PointType type;
    public Vector3 pos;
    public List<Point> linked;
    public int risk;

    public Point(PointType type, Vector3 pos)
    {
        this.type = type;
        this.pos = pos;
        this.linked = new List<Point>();
    }
}

public enum PointType {

    START,
    RISK,
    TRAP,
    BOON,
    TREASURE,
    NONE,
    END
}