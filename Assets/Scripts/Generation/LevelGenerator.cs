using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    private Game game;

    public GameObject nodePrefab;
    public GameObject linkPrefab;
    public GameObject goalPrefab;
    public GameObject healthPrefab;
    public GameObject potionPrefab;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public Transform map;

    private Point[][] points;
    private const int SIZE = 4;

    private Point lastPoint;
    private List<PointLink> links;

    // Start is called before the first frame update
    void Start()
    {
        game = FindObjectOfType<Game>();
        points = new Point[SIZE + 1][];
        for (int i = 0; i < SIZE; i++)
        {
            points[i] = new Point[SIZE + 1];
        }
        links = new List<PointLink>();
        GenerateLevel();
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

    private Point GenerateGrid()
    {
        List<HashSet<Point>> groups = new List<HashSet<Point>>();

        for (int x = 0; x < SIZE; x++)
        {
            for (int z = 0; z < SIZE; z++)
            {
                PointType thisType = PointType.NONE;
                if ((z + 1 + (x % 2)) % 2 == 0)
                {
                    thisType = PointType.RISK;
                }
                
                Point point = new Point(thisType, new Vector3(x * 4, 0, z * 4));
                point.risk = Random.Range(1, 6);
                AddPoint(point);

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
            }
        }

        // if there's more than 1 group
        while (groups.Count > 1)
        {
            // get the first group
            HashSet<Point> group = new HashSet<Point>(groups[0]);
            // iterate over ever point
            foreach ( Point point in group)
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
                Debug.Log("found lonely node");
                point.type = PointType.HEALTH;
                if (UnityEngine.Random.Range(0, 4) == 1)
                {
                    point.type = PointType.POTION;
                }
            }
        }


        Point startPoint = points[0][0];
        lastPoint = points[SIZE - 1][SIZE - 1];
        startPoint.type = PointType.START;
        lastPoint.type = PointType.END;
        return startPoint;
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
            else {
                Debug.LogError("wat");
            }
        }
    }

    public void GenerateLevel()
    {
        //Point startPoint = GeneratePoints();

        Point startPoint = GenerateGrid();

        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                if (points[i][j] != null)
                {
                    ConvertPoint(points[i][j]);
                }
            }
        }

        Debug.Log("adding links " + links.Count);
        foreach (PointLink pointLink in links)
        {
            CreateLink(pointLink.from, pointLink.to);
        }

        GameObject playerObj = Instantiate(playerPrefab, startPoint.pos, Quaternion.identity, map);
        Player player = playerObj.GetComponent<Player>();
        player.speed = 4f;
        game.player = player;
        game.selectedPlayer = player;
    }

    private void ConvertPoint(Point point)
    {
        GameObject nodeTypePrefab = GetPrefab(point.type);
        GameObject nodeObject = Instantiate(nodeTypePrefab, point.pos, Quaternion.identity, map);
        if (point.type == PointType.RISK)
        {
            Node node = nodeObject.GetComponent<Node>();
            node.risk = point.risk;
            GameObject enemyObject = Instantiate(enemyPrefab, point.pos, Quaternion.identity, map);
            Actor enemyActor = enemyObject.GetComponent<Actor>();
            node.actor = enemyActor;
            enemyActor.node = node;
        }
    }

    private void CreateLink(Point from, Point to)
    {
        if (from != null && to != null)
        {
            GameObject linkObj = Instantiate(linkPrefab, (from.pos + to.pos) / 2f, Quaternion.identity, map);
            if (to.pos.x != from.pos.x)
            {
                linkObj.transform.Rotate(0, 90, 0);
            }
        }
    }

    private bool RollCheck(int odds)
    {
        return Random.Range(1, odds + 1) == 1;
    }

    private GameObject GetPrefab(PointType type)
    {
        if (type == PointType.END)
        {
            return goalPrefab;
        }
        if (type == PointType.HEALTH)
        {
            return healthPrefab;
        }
        if (type == PointType.POTION)
        {
            return potionPrefab;
        }
        return nodePrefab;
    }

    private Point GeneratePoints()
    {
        // start point
        Point startPoint = new Point(PointType.START, new Vector3(24, 0, 0));
        AddPoint(startPoint);

        GenerateLine(startPoint, 7);

        // end point
        lastPoint.type = PointType.END;

        return startPoint;
    }

    private void GenerateLine(Point startPoint, int numPoints)
    {
        List<Split> splits = new List<Split>();
        int currentRisk = 2;
        bool isDone = false;
        Point currentPoint = startPoint;
        int riskNodeCounter = 0;
        int currentPoints = numPoints;
        while (!isDone)
        {
            PointType thisType = PointType.NONE;
            if (riskNodeCounter % 2 == 0)
            {
                thisType = PointType.RISK;
                if (currentRisk < 6)
                {
                    currentRisk++;
                }
            }
            // add point to current
            Point nextPoint = new Point(thisType, currentPoint.pos + new Vector3(0, 0, 4));
            AddPoint(nextPoint);
            nextPoint.risk = currentRisk;

            // split
            if (RollCheck(4) && currentPoints > 1)
            {
                Point splitPoint = GetPoint(currentPoint.pos + new Vector3(4, 0, 0));
                if (splitPoint == null)
                {
                    Debug.Log("splitting");
                    splitPoint = new Point(PointType.NONE, currentPoint.pos + new Vector3(4, 0, 0));
                    AddPoint(splitPoint);
                    links.Add(new PointLink(currentPoint, splitPoint));
                    splits.Add(new Split(splitPoint, currentPoints));
                }
            }

            // merge
            if (RollCheck(6))
            {
                Debug.Log("merging");
                Point mergePoint = GetPoint(currentPoint.pos - new Vector3(4, 0, 0));
                if (mergePoint != null)
                {
                    Debug.Log("found merge point");
                    links.Add(new PointLink(currentPoint, mergePoint));
                    isDone = true;
                }
            }

            links.Add(new PointLink(currentPoint, nextPoint));
            currentPoint = nextPoint;
            riskNodeCounter++;
            currentPoints--;
            if (currentPoints == 0)
            {
                isDone = true;
            }
        }
        lastPoint = currentPoint;

        foreach (Split split in splits)
        {
            GenerateLine(split.point, split.points);
        }
    }

    private void AddPoint(Point point)
    {
        points[Mathf.FloorToInt(point.pos.z / 4)][Mathf.FloorToInt(point.pos.x / 4)] = point;
    }

    private void RemovePoint(Point point)
    {
        points[Mathf.FloorToInt(point.pos.z / 4)][Mathf.FloorToInt(point.pos.x / 4)] = null;
    }

    private Point GetPoint(Vector3 pos)
    {
        if (pos.x < 0 || pos.z < 0 || pos.x > (SIZE * 4) - 1 || pos.z > (SIZE * 4) - 1)
        {
            //Debug.LogError("tried to get point that doesn't exist. " + pos.x + "," + pos.z);
            return null;
        }
        return points[Mathf.FloorToInt(pos.z / 4)][Mathf.FloorToInt(pos.x / 4)];
    }
}
