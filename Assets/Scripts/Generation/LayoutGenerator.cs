using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lovely
{
    public class LayoutGenerator : MonoBehaviour
    {
        internal List<PointLink> links;
        private Point lastPoint;
        private List<List<Point>> channels;
        private Dictionary<String, Point> points;

        private void GenerateLine(Vector2Int from, Vector2Int to, List<Point> channel)
        {
            //Debug.Log("starting line");

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
                SetPoint(point);
                //Debug.Log("set point " + currentPos.x + " " + currentPos.y);
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
            SetPoint(finalPoint);
            //Debug.Log("setting final line point " + currentPos.x + " " + currentPos.y);
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
                if (channel[index].pos.x == channel[index + 1].pos.x)
                {
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
                    movePoints(a, b, new Vector3(val, 0, 0), channel);
                }
            }
            else
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
                    movePoints(a, b, new Vector3(0, 0, val), channel);
                }
            }
        }

        private void movePoints(Point a, Point b, Vector3 move, List<Point> channel)
        {
            Point c = new Point(PointType.NONE, a.pos + move);
            channel.Add(c);
            SetPoint(c);
            links.Add(new PointLink(a, c));

            Point d = new Point(PointType.NONE, b.pos + move);
            channel.Add(d);
            SetPoint(d);

            bool didBreak = links.Remove(new PointLink(a, b));
            if (!didBreak)
            {
                Debug.LogError("failed to remove link");
            }

            links.Add(new PointLink(b, d));
            links.Add(new PointLink(c, d));
        }

        private Point movePoint(Point oldPoint, Vector3 newPointPos, List<Point> channel)
        {
            Point newPoint = new Point(PointType.NONE, newPointPos);
            channel.Add(newPoint);
            SetPoint(newPoint);
            links.Add(new PointLink(oldPoint, newPoint));
            return newPoint;
        }

        internal List<Point> GetAllPoints()
        {
            List<Point> allPoints = new List<Point>();
            foreach (List<Point> channel in channels)
            {
                allPoints.AddRange(channel);
            }
            return allPoints;
        }

        internal Point GenerateLayout()
        {
            points = new Dictionary<string, Point>();
            links = new List<PointLink>();

            List<Point> mainChannel = new List<Point>();
            List<Point> leftChannel = new List<Point>();
            List<Point> rightChannel = new List<Point>();
            channels = new List<List<Point>>();
            channels.Add(mainChannel);
            channels.Add(leftChannel);
            channels.Add(rightChannel);

            GenerateLine(new Vector2Int(LevelGenerator.SIZE / 2, 0), new Vector2Int(LevelGenerator.SIZE / 2, LevelGenerator.SIZE - 1), mainChannel);

            int sectionLength = LevelGenerator.SIZE / 3;

            // one side
            Vector2Int a = new Vector2Int(LevelGenerator.SIZE / 2, UnityEngine.Random.Range(0, sectionLength));
            Vector2Int b = new Vector2Int(LevelGenerator.SIZE / 2, a.y + UnityEngine.Random.Range(sectionLength, sectionLength * 2));
            Vector2Int c = new Vector2Int(UnityEngine.Random.Range(1, (LevelGenerator.SIZE / 2)), UnityEngine.Random.Range(a.y, b.y + 1));
            //Debug.Log("set point a " + a.x + " " + a.y);
            //Debug.Log("set point b " + b.x + " " + b.y);
            //Debug.Log("set point c " + c.x + " " + c.y);
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
            Point startPoint = points[GetKey(new Vector3((LevelGenerator.SIZE / 2) * 4, 0, 0))];
            lastPoint = points[GetKey(new Vector3((LevelGenerator.SIZE / 2) * 4, 0, (LevelGenerator.SIZE - 1) * 4))];

            // add enemies to channels

            channels = channels.OrderBy(channel => UnityEngine.Random.value).ToList();
            int avgRisk = UnityEngine.Random.Range(2, 4);
            foreach (List<Point> channel in channels)
            {
                AddGameplayToChannel(channel, avgRisk);
                avgRisk += 1;
            }

            // add perm item
            List<Point> permChannel = UnityEngine.Random.Range(0, 2) == 0 ? leftChannel : rightChannel;
            Point permPoint = permChannel[rightChannel.Count / 2];

            permPoint.type = new List<PointType>() {
            PointType.PERM_HEALTH_INC,
            PointType.PERM_ROLL_INC,
            PointType.PERM_START_ARMOUR
        }[UnityEngine.Random.Range(0, 3)];
            permPoint.risk = 1;

            // choose endpoint
            lastPoint = points[GetKey(new Vector3((LevelGenerator.SIZE / 2) * 4, 0, (LevelGenerator.SIZE - 1) * 4))];
            lastPoint.type = PointType.END;
            lastPoint.risk = 0;

            // confirm startpoint
            startPoint.type = PointType.START;
            startPoint.risk = 0;

            return startPoint;
        }

        private int GetRisk(int avgRisk)
        {
            int chances = UnityEngine.Random.Range(0, 100);
            if (chances <= 10)
            {
                return GetSafeRisk(avgRisk - 2);
            }
            if (chances <= 35)
            {
                return GetSafeRisk(avgRisk - 1);
            }
            if (chances <= 65)
            {
                return GetSafeRisk(avgRisk);
            }
            if (chances <= 90)
            {
                return GetSafeRisk(avgRisk + 1);
            }
            return GetSafeRisk(avgRisk + 2);
        }

        private int GetSafeRisk(int risk)
        {
            return Mathf.Min(Mathf.Max(1, risk), 6);
        }

        private void AddGameplayToChannel(List<Point> channel, int avgRisk)
        {
            int gap = UnityEngine.Random.Range(0, 3);
            int riskTotal = 0;
            foreach (Point point in channel)
            {
                if (point.risk == 0 && gap % 3 != 0 && point.type == PointType.NONE)
                {
                    point.risk = GetRisk(avgRisk);
                    riskTotal += point.risk;
                    point.type = PointType.RISK;
                    if (UnityEngine.Random.Range(0, 4) == 0)
                    {
                        switch (UnityEngine.Random.Range(0, 4))
                        {
                            case 0:
                                point.type = PointType.POISON;
                                point.risk = 3;
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
                gap += 1;
            }
            AddHealthToChannel(channel, riskTotal);
        }

        private void AddHealthToChannel(List<Point> channel, int riskTotal)
        {
            int currentRisk = riskTotal;
            int preventInfinite = 0;
            while (currentRisk > 0 && preventInfinite < 100)
            {
                Point point = channel[UnityEngine.Random.Range(0, channel.Count)];
                if (point.Equals(lastPoint))
                {
                    preventInfinite++;
                    continue;
                }
                Vector3 offset = new Vector3();
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    offset.x = UnityEngine.Random.Range(0, 2) == 0 ? -4 : 4;
                }
                else
                {
                    offset.z = UnityEngine.Random.Range(0, 2) == 0 ? -4 : 4;
                }
                Vector3 newPointPos = point.pos + offset;
                Point c = GetPoint(newPointPos);
                if (c == null)
                {
                    Point newPoint = movePoint(point, newPointPos, channel);

                    if (UnityEngine.Random.Range(0, 4) == 0)
                    {
                        switch (UnityEngine.Random.Range(0, 4))
                        {
                            case 1:
                                newPoint.type = PointType.ARMOUR;
                                newPoint.risk = 2;
                                currentRisk -= 6;
                                break;
                            case 2:
                                newPoint.type = PointType.FRIEND;
                                newPoint.risk = 2;
                                currentRisk -= 6;
                                break;
                            case 3:
                                newPoint.type = PointType.POTION;
                                newPoint.risk = 4;
                                currentRisk -= 6;
                                break;
                            case 0:
                                newPoint.type = PointType.SWORD;
                                newPoint.risk = 2;
                                currentRisk -= 6;
                                break;
                        }
                    }
                    else
                    {
                        newPoint.risk = 0;
                        newPoint.type = PointType.HEALTH;
                        int amount = UnityEngine.Random.Range(1, 4);
                        currentRisk -= (amount * 3);
                        newPoint.amount = amount;
                    }
                }
                preventInfinite++;
            }
        }

        private void SetPoint(Point point)
        {
            points[GetKey(point.pos)] = point;
        }

        private String GetKey(Vector3 pos)
        {
            return "" + Mathf.FloorToInt(pos.z / 4) + ":" + Mathf.FloorToInt(pos.x / 4);
        }

        private void RemovePoint(Point point)
        {
            points[GetKey(point.pos)] = null;
        }

        private Point GetPoint(Vector3 pos)
        {
            String key = GetKey(pos);
            if (!points.ContainsKey(key))
            {
                return null;
            }
            return points[key];
        }
    }
}