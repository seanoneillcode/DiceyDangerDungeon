using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Point
{
    public PointType type;
    public Vector3 pos;
    public int risk;

    public Point(PointType type, Vector3 pos)
    {
        this.type = type;
        this.pos = pos;
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