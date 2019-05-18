using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutGenerator : MonoBehaviour
{

    internal Point[][] points;
    internal List<PointLink> links;
    private Point lastPoint;

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
        }
        finalPoint.risk = 0;
        SetPoint(finalPoint);
        Debug.Log("setting final line point " + currentPos.x + " " + currentPos.y);
        if (previousPoint != null && finalPoint != null)
        {
            links.Add(new PointLink(finalPoint, previousPoint));
        }
    }

    internal Point GenerateLayout()
    {
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
        //GenerateLine(c, b);

        // other side
        Vector2Int e = new Vector2Int(LevelGenerator.SIZE / 2, UnityEngine.Random.Range(0, LevelGenerator.SIZE - 1 - 1));
        Vector2Int f = new Vector2Int(LevelGenerator.SIZE / 2, UnityEngine.Random.Range(e.y + 1, LevelGenerator.SIZE - 1));
        Vector2Int g = new Vector2Int(UnityEngine.Random.Range((LevelGenerator.SIZE / 2) + 1, LevelGenerator.SIZE - 1), UnityEngine.Random.Range(e.y, f.y + 1));
        GenerateLine(e, g);
        GenerateLine(f, g);

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
        if (point.pos.x < 0 || point.pos.z < 0 || point.pos.x > (LevelGenerator.SIZE * 4) - 1 || point.pos.z > (LevelGenerator.SIZE * 4) - 1)
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
        if (pos.x < 0 || pos.z < 0 || pos.x > (LevelGenerator.SIZE * 4) - 1 || pos.z > (LevelGenerator.SIZE * 4) - 1)
        {
            return null;
        }
        return points[Mathf.FloorToInt(pos.z / 4)][Mathf.FloorToInt(pos.x / 4)];
    }
}
