using UnityEngine;
using UnityEditor;

public class PointLink
{
    public Point from;
    public Point to;

    public PointLink(Point from, Point to)
    {
        this.from = from;
        this.to = to;
    }
}