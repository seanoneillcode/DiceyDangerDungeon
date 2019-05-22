using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LayoutGenerator : MonoBehaviour
{

    internal Point[][] points;
    internal List<PointLink> links;
    private Point lastPoint;
    

    private void GenerateLine(Vector2Int from, Vector2Int to, List<Point> channel)
    {
        Debug.Log("starting line");

        Point previousPoint = null;
        Vector2Int currentPos = new Vector2Int(from.x, from.y);
        while (currentPos.x != to.x || currentPos.y != to.y)
        {
            Point point = GetPoint(new Vector3(currentPos.x * 4, 0, currentPos.y * 4));
            if (point == null)
            {
                point = new Point(PointType.NONE, new Vector3(currentPos.x * 4, 0, currentPos.y * 4));
                channel.Add(point);
            }
            point.risk = 0;
            SetPoint(point);
            Debug.Log("set point " + currentPos.x + " " + currentPos.y);
            if (previousPoint != null && point != null)
            {
                links.Add(new PointLink(point, previousPoint));
            }
            previousPoint = point;
            if (currentPos.x < to.x)
            {
                currentPos.x += 1;
                continue;
            }
            if (currentPos.x > to.x)
            {
                currentPos.x -= 1;
                continue;
            }
            if (currentPos.y < to.y)
            {
                currentPos.y += 1;
                continue;
            }
            if (currentPos.y > to.y)
            {
                currentPos.y -= 1;
                continue;
            }
        }
        Point finalPoint = GetPoint(new Vector3(currentPos.x * 4, 0, currentPos.y * 4));
        if (finalPoint == null)
        {
            finalPoint = new Point(PointType.NONE, new Vector3(currentPos.x * 4, 0, currentPos.y * 4));
            channel.Add(finalPoint);
        }
        finalPoint.risk = 0;
        SetPoint(finalPoint);
        Debug.Log("setting final line point " + currentPos.x + " " + currentPos.y);
        if (previousPoint != null && finalPoint != null)
        {
            links.Add(new PointLink(finalPoint, previousPoint));
        }
    }

    internal void PushPoints(List<Point> channel)
    {
        Point a = null;
        Point b = null;

        int count = 0;
        bool done = false;
        while (!done && count < 40)
        {
            count++;
            int index = UnityEngine.Random.Range(1, channel.Count - 1);
            if (channel[index].pos.x == channel[index + 1].pos.x) {
                done = true;
                a = channel[index];
                b = channel[index + 1];
                break;
            }
            if (channel[index].pos.z == channel[index + 1].pos.z)
            {
                done = true;
                a = channel[index];
                b = channel[index + 1];
            }
        }
        if (a == null || b == null)
        {
            return;
        }

        if (a.pos.x == b.pos.x)
        {
            int val = 4;
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                val = -4;
            }
            Point c = GetPoint(a.pos + new Vector3(val, 0, 0));
            Point d = GetPoint(b.pos + new Vector3(val, 0, 0));
            if (c == null && d == null)
            {
                if(IsPosValid(a.pos + new Vector3(val, 0, 0)) && IsPosValid(b.pos + new Vector3(val, 0, 0)))
                {
                    movePoints(a, b, new Vector3(val, 0, 0), channel);
                }
            }
        } else
        {
            int val = 4;
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                val = -4;
            }
            Point c = GetPoint(a.pos + new Vector3(0, 0, val));
            Point d = GetPoint(b.pos + new Vector3(0, 0, val));
            if (c == null && d == null)
            {
                if (IsPosValid(a.pos + new Vector3(0, 0, val)) && IsPosValid(b.pos + new Vector3(0, 0, val)))
                {
                    movePoints(a, b, new Vector3(0, 0, val), channel);
                }
            }
        }
    }

    private void movePoints(Point a, Point b, Vector3 move, List<Point> channel)
    {
        Point c = new Point(PointType.NONE, a.pos + move);
        channel.Add(c);    
        c.risk = 0;
        SetPoint(c);
        links.Add(new PointLink(a, c));

        Point d = new Point(PointType.NONE, b.pos + move);
        channel.Add(d);
        d.risk = 0;
        SetPoint(d);

        bool didBreak = links.Remove(new PointLink(a,b));
        if (!didBreak)
        {
            Debug.LogError("failed to remove link");
        }

        links.Add(new PointLink(b, d));
        links.Add(new PointLink(c, d));
    }

    internal Point GenerateLayout()
    {
        points = new Point[LevelGenerator.SIZE + 1][];
        for (int i = 0; i < LevelGenerator.SIZE; i++)
        {
            points[i] = new Point[LevelGenerator.SIZE + 1];
        }
        links = new List<PointLink>();

        List<Point> mainChannel = new List<Point>();
        List<Point> leftChannel = new List<Point>();
        List<Point> rightChannel = new List<Point>();

        GenerateLine(new Vector2Int(LevelGenerator.SIZE / 2, 0), new Vector2Int(LevelGenerator.SIZE / 2, LevelGenerator.SIZE - 1), mainChannel);

        int sectionLength = LevelGenerator.SIZE / 3;

        // one side
        Vector2Int a = new Vector2Int(LevelGenerator.SIZE / 2, UnityEngine.Random.Range(0, sectionLength));
        Vector2Int b = new Vector2Int(LevelGenerator.SIZE / 2, a.y + UnityEngine.Random.Range(sectionLength, sectionLength * 2));
        Vector2Int c = new Vector2Int(UnityEngine.Random.Range(1, (LevelGenerator.SIZE / 2)), UnityEngine.Random.Range(a.y, b.y + 1));
        Debug.Log("set point a " + a.x + " " + a.y);
        Debug.Log("set point b " + b.x + " " + b.y);
        Debug.Log("set point c " + c.x + " " + c.y);
        GenerateLine(a, c, leftChannel);
        GenerateLine(b, c, leftChannel);

        // other side
        Vector2Int e = new Vector2Int(LevelGenerator.SIZE / 2, UnityEngine.Random.Range(0, sectionLength));
        Vector2Int f = new Vector2Int(LevelGenerator.SIZE / 2, e.y + UnityEngine.Random.Range(sectionLength, sectionLength * 2));
        Vector2Int g = new Vector2Int(UnityEngine.Random.Range((LevelGenerator.SIZE / 2) + 1, LevelGenerator.SIZE), UnityEngine.Random.Range(e.y, f.y + 1));
        GenerateLine(e, g, rightChannel);
        GenerateLine(f, g, rightChannel);

        // push points
        int numPushes = 2;
        for (int i = 0; i < numPushes; i++)
        {
            PushPoints(mainChannel);
            PushPoints(leftChannel);
            PushPoints(rightChannel);
        }

        // choose start point
        Point startPoint = points[0][LevelGenerator.SIZE / 2];
        
        // add enemies to channels
        AddEnemiesToChannel(startPoint, false);

        // choose endpoint
        lastPoint = points[LevelGenerator.SIZE - 1][LevelGenerator.SIZE / 2];
        lastPoint.type = PointType.END;
        lastPoint.risk = 0;

        // confirm startpoint
        startPoint.type = PointType.START;
        startPoint.risk = 0;

        return startPoint;
    }

    private int GetRisk()
    {
        int chances = UnityEngine.Random.Range(0, 10);
        if (chances <= 1)
        {
            return 2;
        }
        if (chances <= 4)
        {
            return 3;
        }
        if (chances <= 7)
        {
            return 4;
        }
        if (chances <= 8)
        {
            return 5;
        }
        if (chances <= 9)
        {
            return 6;
        }
        return 1;
    }

    private void AddEnemiesToChannel(Point point, bool flip)
    {
        if (point.risk != 0)
        {
            return;
        }
        if (flip)
        {
            point.risk = GetRisk();
            point.type = PointType.RISK;
            if (UnityEngine.Random.Range(0, 4) == 0)
            {
                switch (UnityEngine.Random.Range(0, 4))
                {
                    case 0:
                        point.type = PointType.POISON;
                        break;
                    case 1:
                        point.type = PointType.GHOST;
                        break;
                    case 2:
                        point.type = PointType.TELEPORT;
                        break;
                    case 3:
                        point.type = PointType.TRAP;
                        break;
                }
            }
        }
        flip = !flip;
        List<PointLink> neighbours = links.Where(l => l.from == point || l.to == point).ToList();
        foreach ( PointLink link in neighbours)
        {
            Point other = link.from == point ? link.to : link.from;
            AddEnemiesToChannel(other, flip);
        }
    }

    private void SetPoint(Point point)
    {
        if (!IsPosValid(point.pos))
        {
            Debug.LogError("setting point out of bounds " + point.pos.x + " " + point.pos.z);
        }
        points[Mathf.FloorToInt(point.pos.z / 4)][Mathf.FloorToInt(point.pos.x / 4)] = point;
    }

    private void RemovePoint(Point point)
    {
        points[Mathf.FloorToInt(point.pos.z / 4)][Mathf.FloorToInt(point.pos.x / 4)] = null;
    }

    private Point GetPoint(Vector3 pos)
    {
        if (!IsPosValid(pos))
        {
            return null;
        }
        return points[Mathf.FloorToInt(pos.z / 4)][Mathf.FloorToInt(pos.x / 4)];
    }

    private bool IsPosValid(Vector3 pos)
    {
        if (pos.x < 0 || pos.z < 0 || pos.x > (LevelGenerator.SIZE * 4) - 1 || pos.z > (LevelGenerator.SIZE * 4) - 1)
        {
            return false;
        }
        return true;
    }
}
