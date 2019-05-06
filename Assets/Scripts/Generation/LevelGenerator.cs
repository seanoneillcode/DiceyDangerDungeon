using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public GameObject nodePrefab;
    public GameObject linkPrefab;
    public GameObject goalPrefab;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public Transform map;

    private Point[][] points;
    private const int SIZE = 100;

    // Start is called before the first frame update
    void Start()
    {
        points = new Point[SIZE][];
        for (int i = 0; i < SIZE; i++)
        {
            points[i] = new Point[SIZE];
        }
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        Point startPoint = GeneratePoints();
        ConvertPointRecursively(startPoint);
        GameObject playerObj = Instantiate(playerPrefab, startPoint.pos, Quaternion.identity, map);
        playerObj.GetComponent<Player>().speed = 4f;
    }

    private void ConvertPointRecursively(Point point)
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

        foreach (Point linkedPoint in point.linked)
        {
            Instantiate(linkPrefab, (point.pos + linkedPoint.pos) / 2f, Quaternion.identity, map);
            ConvertPointRecursively(linkedPoint);
        }
    }

    private GameObject GetPrefab(PointType type)
    {
        if (type == PointType.END)
        {
            return goalPrefab;
        }
        return nodePrefab;
    }

    private Point GeneratePoints()
    {
        // start point
        Point startPoint = new Point(PointType.START, new Vector3());
        AddPoint(startPoint);
        Point currentPoint = startPoint;

        // generate structure with points
        int currentRisk = 2;
        int numOfPoints = 6; // UnityEngine.Random.Range(1, 6);
        bool isDone = false;
        Point lastPoint = startPoint;
        int riskNodeCounter = 0;
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
            
            currentPoint.linked.Add(nextPoint);

            lastPoint = nextPoint;
            currentPoint = nextPoint;

            riskNodeCounter++;
            numOfPoints--;
            if (numOfPoints == 0)
            {
                isDone = true;
            }
        }

        // end point
        currentPoint.type = PointType.END;

        return startPoint;
    }

    private void AddPoint(Point point)
    {
        points[(int)point.pos.y][(int)point.pos.x] = point;
    }

    private Point GetPoint(Vector3 pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x > SIZE - 1 || pos.y > SIZE - 1)
        {
            Debug.LogError("tried to get point that doesn't exist.");
            return null;
        }
        return points[(int)pos.y][(int)pos.x];
    }
}
