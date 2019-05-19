using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LayoutGenerator : MonoBehaviour
{

    internal Point[][] points;
    internal List<PointLink> links;
    private Point lastPoint;
    internal List<Point> pointList;

    private bool RollCheck(int odds)
    {
        return Random.Range(1, odds + 1) == 1;
    }

    private HashSet<Point> MergeGroups(HashSet<Point> groupA, HashSet<Point> groupB)
    {
        if (groupA.Equals(groupB))
        {
            return groupA;
        }
        foreach (Point point in groupB)
        {
            groupA.Add(point);
        }
        return groupA;
    }

    private HashSet<Point> GetGroup(List<HashSet<Point>> groups, Point point)
    {
        if (point == null)
        {
            return null;
        }
        foreach (HashSet<Point> group in groups)
        {
            if (group.Contains(point))
            {
                return group;
            }
        }
        return null;
    }

    private void CheckPoint(List<HashSet<Point>> groups, Point origin, Point other)
    {
        HashSet<Point> otherGroup = GetGroup(groups, other);
        if (otherGroup != null && !otherGroup.Contains(origin))
        {
            links.Add(new PointLink(origin, other));
            HashSet<Point> originGroup = GetGroup(groups, origin);
            if (!originGroup.Equals(otherGroup))
            {
                MergeGroups(originGroup, otherGroup);
                groups.Remove(otherGroup);
            }
            else
            {
                Debug.LogError("wat");
            }
        }
    }

    private void GenerateLine(Vector2Int from, Vector2Int to)
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
                pointList.Add(point);
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
            pointList.Add(finalPoint);
        }
        finalPoint.risk = 0;
        SetPoint(finalPoint);
        Debug.Log("setting final line point " + currentPos.x + " " + currentPos.y);
        if (previousPoint != null && finalPoint != null)
        {
            links.Add(new PointLink(finalPoint, previousPoint));
        }
    }

    internal void PushPoints()
    {
        Point a = null;
        Point b = null;

        int count = 0;
        bool done = false;
        while (!done && count < 40)
        {
            count++;
            int index = Random.Range(1, pointList.Count - 1);
            if (pointList[index].pos.x == pointList[index + 1].pos.x) {
                done = true;
                a = pointList[index];
                b = pointList[index + 1];
                break;
            }
            if (pointList[index].pos.z == pointList[index + 1].pos.z)
            {
                done = true;
                a = pointList[index];
                b = pointList[index + 1];
            }
        }
        if (a == null || b == null)
        {
            return;
        }
        Debug.Log("got two points");
        Debug.Log("point a " + a.pos.x / 4 + " " + a.pos.z / 4);
        Debug.Log("point b " + b.pos.x / 4 + " " + b.pos.z / 4);

        if (a.pos.x == b.pos.x)
        {
            int val = 4;
            if (Random.Range(0, 2) == 0)
            {
                val = -4;
            }
            Point c = GetPoint(a.pos + new Vector3(val, 0, 0));
            Point d = GetPoint(b.pos + new Vector3(val, 0, 0));
            if (c == null && d == null)
            {
                Debug.Log("c and d are null");
                if(IsPosValid(a.pos + new Vector3(val, 0, 0)) && IsPosValid(b.pos + new Vector3(val, 0, 0)))
                {
                    Debug.Log("c and d are valid");
                    movePoints(a, b, new Vector3(val, 0, 0));
                }
            } else
            {
                Debug.Log("c or d are not null");
            }
        } else
        {
            int val = 4;
            if (Random.Range(0, 2) == 0)
            {
                val = -4;
            }
            Point c = GetPoint(a.pos + new Vector3(0, 0, val));
            Point d = GetPoint(b.pos + new Vector3(0, 0, val));
            if (c == null && d == null)
            {
                Debug.Log("c and d are null");
                if (IsPosValid(a.pos + new Vector3(0, 0, val)) && IsPosValid(b.pos + new Vector3(0, 0, val)))
                {
                    Debug.Log("c and d are valid");
                    movePoints(a, b, new Vector3(0, 0, val));
                }
            }
            else
            {
                Debug.Log("c or d are not null");
            }
        }
    }

    private void movePoints(Point a, Point b, Vector3 move)
    {
        Point c = new Point(PointType.NONE, a.pos + move);
        pointList.Add(c);    
        c.risk = 0;
        SetPoint(c);
        links.Add(new PointLink(a, c));

        Point d = new Point(PointType.NONE, b.pos + move);
        pointList.Add(d);
        d.risk = 0;
        SetPoint(d);
        links.Add(new PointLink(b, d));
        links.Add(new PointLink(c, d));

        bool didBreak = links.Remove(new PointLink(a,b));
        if (!didBreak)
        {
            Debug.LogError("failed to remove link");
        }
        //links = links.Where(link => (link.from == b && link.to == a)).ToList();
        Debug.Log("pushed two points");
    }

    internal Point GenerateLayout()
    {
        pointList = new List<Point>();
        points = new Point[LevelGenerator.SIZE + 1][];
        for (int i = 0; i < LevelGenerator.SIZE; i++)
        {
            points[i] = new Point[LevelGenerator.SIZE + 1];
        }
        links = new List<PointLink>();

        GenerateLine(new Vector2Int(LevelGenerator.SIZE / 2, 0), new Vector2Int(LevelGenerator.SIZE / 2, LevelGenerator.SIZE - 1));

        // one side
        Vector2Int a = new Vector2Int(LevelGenerator.SIZE / 2, UnityEngine.Random.Range(0, LevelGenerator.SIZE - 1 - 1));
        Vector2Int b = new Vector2Int(LevelGenerator.SIZE / 2, UnityEngine.Random.Range(a.y + 1, LevelGenerator.SIZE - 1));
        Vector2Int c = new Vector2Int(UnityEngine.Random.Range(0, (LevelGenerator.SIZE / 2)), UnityEngine.Random.Range(a.y, b.y + 1 ));
        Debug.Log("set point a " + a.x + " " + a.y);
        Debug.Log("set point b " + b.x + " " + b.y);
        Debug.Log("set point c " + c.x + " " + c.y);
        GenerateLine(a, c);
        GenerateLine(b, c);

        // other side
        Vector2Int e = new Vector2Int(LevelGenerator.SIZE / 2, UnityEngine.Random.Range(0, LevelGenerator.SIZE - 1 - 1));
        Vector2Int f = new Vector2Int(LevelGenerator.SIZE / 2, UnityEngine.Random.Range(e.y + 1, LevelGenerator.SIZE - 1));
        Vector2Int g = new Vector2Int(UnityEngine.Random.Range((LevelGenerator.SIZE / 2) + 1, LevelGenerator.SIZE - 1), UnityEngine.Random.Range(e.y, f.y + 1));
        GenerateLine(e, g);
        GenerateLine(f, g);

        // push points
        for (int j = 0; j < pointList.Count; j++)
        {
            Debug.Log("point " + pointList[j].pos.x / 4 + " " + pointList[j].pos.z / 4);
        }
        int numPushes = LevelGenerator.SIZE * LevelGenerator.SIZE;
        for (int i = 0; i < numPushes; i++)
        {
            PushPoints();
        }


        Point startPoint = points[0][LevelGenerator.SIZE / 2];
        lastPoint = points[LevelGenerator.SIZE - 1][LevelGenerator.SIZE / 2];
        startPoint.type = PointType.START;
        startPoint.risk = 0;
        lastPoint.type = PointType.END;
        lastPoint.risk = 0;
        return startPoint;
    }

    internal Point GenerateGrid()
    {
        List<HashSet<Point>> groups = new List<HashSet<Point>>();

        for (int x = 0; x < LevelGenerator.SIZE; x++)
        {
            for (int z = 0; z < LevelGenerator.SIZE; z++)
            {
                PointType thisType = PointType.NONE;

                Point point = new Point(thisType, new Vector3(x * 4, 0, z * 4));
                point.risk = 0;
                SetPoint(point);

                HashSet<Point> newGroup = new HashSet<Point>();
                newGroup.Add(point);
                groups.Add(newGroup);

                if (x > 0 && RollCheck(2))
                {
                    Point previousPoint = GetPoint(new Vector3(point.pos.x - 4, point.pos.y, point.pos.z));
                    if (previousPoint != null)
                    {
                        links.Add(new PointLink(point, previousPoint));
                        HashSet<Point> otherGroup = GetGroup(groups, previousPoint);
                        if (!otherGroup.Equals(newGroup))
                        {
                            MergeGroups(otherGroup, newGroup);
                            groups.Remove(newGroup);
                        }
                        newGroup = otherGroup;
                    }
                }
                if (z > 0 && RollCheck(2))
                {
                    Point previousPoint = GetPoint(new Vector3(point.pos.x, point.pos.y, point.pos.z - 4));
                    if (previousPoint != null)
                    {
                        links.Add(new PointLink(point, previousPoint));
                        HashSet<Point> otherGroup = GetGroup(groups, previousPoint);
                        if (!otherGroup.Equals(newGroup))
                        {
                            MergeGroups(otherGroup, newGroup);
                            groups.Remove(newGroup);
                        }
                        newGroup = otherGroup;
                    }
                }
                if ((z + 1 + (x % 2)) % 2 == 0)
                {
                    point.risk = Random.Range(2, 7);
                    point.type = PointType.RISK;
                    if (UnityEngine.Random.Range(0, 3) == 0)
                    {
                        switch (Random.Range(0, 4))
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
            }
        }

        // if there's more than 1 group
        while (groups.Count > 1)
        {
            // get the first group
            HashSet<Point> group = new HashSet<Point>(groups[0]);
            // iterate over ever point
            foreach (Point point in group)
            {
                // if point has neighbour point in other group
                Point left = GetPoint(point.pos + new Vector3(-4, 0, 0));
                Point right = GetPoint(point.pos + new Vector3(4, 0, 0));
                Point up = GetPoint(point.pos + new Vector3(0, 0, 4));
                Point down = GetPoint(point.pos + new Vector3(0, 0, -4));

                CheckPoint(groups, point, left);
                CheckPoint(groups, point, right);
                CheckPoint(groups, point, up);
                CheckPoint(groups, point, down);
            }
        }

        // iterate over ever point
        foreach (Point point in groups[0])
        {
            int count = 0;
            foreach (PointLink link in links)
            {
                if (link.from.Equals(point) || link.to.Equals(point))
                {
                    count++;
                }
            }
            if (count < 2)
            {
                point.type = PointType.HEALTH;
                point.risk = 0;
                if (UnityEngine.Random.Range(0, 2) == 1)
                {
                    point.risk = Random.Range(2, 7);
                    switch (Random.Range(0, 6))
                    {
                        case 0:
                            point.type = PointType.POTION;
                            break;
                        case 1:
                            point.type = PointType.FRIEND;
                            break;
                        case 2:
                            point.type = PointType.SWORD;
                            break;
                        case 3:
                            point.type = PointType.ARMOUR;
                            break;
                        case 4:
                            point.type = PointType.PRISONER;
                            break;
                        case 5:
                            point.type = PointType.GOLD;
                            point.risk = 1;
                            break;
                    }
                }
            }
        }

        Point startPoint = points[0][0];
        lastPoint = points[LevelGenerator.SIZE - 1][LevelGenerator.SIZE - 1];
        startPoint.type = PointType.START;
        startPoint.risk = 0;
        lastPoint.type = PointType.END;
        lastPoint.risk = 0;
        return startPoint;
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
