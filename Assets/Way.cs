using System.Collections.Generic;
using System.Linq;
using UnityEngine;

struct WayPoint
{
    private Vector3 currentPoint;
    private Vector3 previousPoint;

    public WayPoint(Vector3 current, Vector3 previous)
    {
        currentPoint = current;
        previousPoint = previous;
    }

    public Vector3 GetPreviousPoint(Vector3 comparedVector3)
    {
        if (comparedVector3 == currentPoint)
        {
            return previousPoint;
        }

        return Vector3.zero;
    }
}

struct RangeWayPoint
{
    private List<Vector3> points;

    //private Color32 color;
    private Vector3 testVector3;

    public RangeWayPoint(Vector3 vector)
    {
        float X = vector.x - vector.x % 20;
        float Y = vector.y - vector.y % 20;
        float Z = vector.z - vector.z % 20;
        testVector3 = new Vector3(X, Y, Z);
        points = new List<Vector3>();
        points.Add(vector);
        //color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
    }

    public void AddPoint3(Vector3 vector)
    {
        foreach (var point in points)
        {
            if (Vector3.Distance(point, vector) < 4 - 0.1f)
            {
                return;
            }
        }

        points.Add(vector);
        //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //sphere.transform.position = vector;
        //sphere.GetComponent<MeshRenderer>().material.color = color;
    }

    public bool Match(Vector3 vector)
    {
        float X = vector.x - vector.x % 20;
        float Y = vector.y - vector.y % 20;
        float Z = vector.z - vector.z % 20;
        Vector3 newVector3 = new Vector3(X, Y, Z);
        if (newVector3 == testVector3)
        {
            return true;
        }

        return false;
    }

    public bool MatchPoints(Vector3 vector)
    {
        foreach (var point in points)
        {
            if (Vector3.Distance(point, vector) < 4 - 0.1f)
            {
                return true;
            }
        }

        return false;
    }
}

public class Way
{
    List<WayPoint> wayPoints = new List<WayPoint>();
    List<RangeWayPoint> rangeWayPoints = new List<RangeWayPoint>();
    Vector3 startVector3;
    Vector3 finishVector3;
    Vector3 originalFinish;
    List<Vector3> wayList = new List<Vector3>();
    List<Vector3> newWayList = new List<Vector3>();
    List<Vector3> lastWayList = new List<Vector3>();
    List<Vector3> pointList = new List<Vector3>();
    float strideLength = 4.0f;
    float oneDegree = 0.0174532862792735f;
    int angleRotation = 90;
    Vector3 finishPoint;
    bool math = false;
    float allDistance = 0;

    public void AddStartAndFinish(Vector3 startVector, Vector3 finishVector)
    {
        startVector3 = startVector;
        originalFinish = finishVector3 = finishVector;
    }

    public List<Vector3> StartSearch()
    {
        finishVector3 = originalFinish;
        rangeWayPoints.Add(new RangeWayPoint(startVector3));
        SearchingAllPoint();
        if (finishPoint != Vector3.zero)
        {
            AddPointToWay();
            PathDrawing(wayList, Color.green);
            Debug.Log(DistanceSummation(wayList, wayList.Count - 1));

            WayOptimization();
            PathDrawing(newWayList, Color.red);
            Debug.Log(DistanceSummation(newWayList, newWayList.Count - 1));

            //WayOptimization2();
            //PathDrawing(lastWayList, Color.white);
            //Debug.Log(DistanceSummation(lastWayList, lastWayList.Count - 1));
        }

        return newWayList;
    }

    void SearchingAllPoint()
    {
        ResetParameters();
        pointList.Add(startVector3);
        Vector3 startVector = startVector3;
        int value = 0;
        while (value < 40000 && pointList.Count > 0)
        {
            value++;
            pointList = pointList.OrderBy(x => Vector3.Distance(finishVector3, x)).ToList();
            startVector = pointList[0];
            pointList.RemoveAt(0);
            float currentAngle = 0;
            currentAngle += oneDegree * Random.Range(0, 100);
            Vector3 tempVector;
            for (float i = 0; i <= 360 / angleRotation; i++)
            {
                tempVector = AngleCalculationAroundY(currentAngle, startVector, startVector.y) + startVector;
                AddPoin(tempVector, startVector);
                if (math == true)
                {
                    return;
                }

                currentAngle += oneDegree * angleRotation;
            }

            tempVector = startVector + Vector3.up * strideLength;
            AddPoin(tempVector, startVector);
            tempVector = startVector + Vector3.down * strideLength;
            AddPoin(tempVector, startVector);
            if (math == true)
            {
                return;
            }
        }
    }

    Vector3 AngleCalculationAroundY(float angle, Vector3 center, float y)
    {
        float x = center.x + strideLength * Mathf.Sin(angle);
        float z = center.z - strideLength * (1 - Mathf.Cos(angle));
        return new Vector3(x, y, z + strideLength) - center;
    }

    void AddPoin(Vector3 tempVector, Vector3 startVector)
    {
        if (FindingNearestVectors(tempVector) == false)
        {
            Ray ray = new Ray(startVector, tempVector - startVector);
            if (!Physics.Raycast(ray, strideLength))
            {
                if (Vector3.Distance(tempVector, finishVector3) <= strideLength)
                {
                    float distance = Vector3.Distance(tempVector, finishVector3);
                    Ray ray2 = new Ray(tempVector, finishVector3 - tempVector);
                    if (!Physics.Raycast(ray2, distance))
                    {
                        finishPoint = tempVector;
                        wayPoints.Add(new WayPoint(tempVector, startVector));
                        wayList.Add(finishVector3);
                        math = true;
                        return;
                    }
                }

                Debug.DrawRay(startVector, tempVector - startVector, Color.blue, 5);
                pointList.Add(tempVector);
                wayPoints.Add(new WayPoint(tempVector, startVector));
                foreach (var range2 in rangeWayPoints)
                {
                    if (range2.Match(tempVector) == true)
                    {
                        range2.AddPoint3(tempVector);
                        return;
                    }
                }

                rangeWayPoints.Add(new RangeWayPoint(tempVector));
            }
        }
    }

    bool FindingNearestVectors(Vector3 tempVector)
    {
        foreach (var range in rangeWayPoints)
        {
            if (range.Match(tempVector) == true)
            {
                if (range.MatchPoints(tempVector))
                {
                    return true;
                }
            }
        }

        return false;
    }

    void AddPointToWay()
    {
        if (finishPoint == startVector3)
        {
            wayList.Add(startVector3);
            return;
        }

        for (int i = wayPoints.Count - 1; i > 0; i--)
        {
            Vector3 newPoint = wayPoints[i].GetPreviousPoint(finishPoint);
            if (newPoint != Vector3.zero)
            {
                wayList.Add(newPoint);
                finishPoint = newPoint;
            }
        }
    }

    void WayOptimization()
    {
        Vector3 endPoint = wayList[0];
        wayList.RemoveAt(0);
        Vector3 startPoint = wayList[wayList.Count - 1];
        wayList.RemoveAt(wayList.Count - 1);
        newWayList.Add(endPoint);
        SearchPoint();
        newWayList.Add(startPoint);
    }

    void SearchPoint()
    {
        newWayList.Add(wayList[0]);
        Vector3 save = Vector3.zero;
        while (wayList.Count > 1)
        {
            Vector3 vector = wayList[0];
            Ray ray = new Ray(newWayList[newWayList.Count - 1], vector - newWayList[newWayList.Count - 1]);
            float distance = Vector3.Distance(newWayList[newWayList.Count - 1], vector);
            if (!Physics.Raycast(ray, distance))
            {
                save = wayList[0];
                wayList.RemoveAt(0);
            }
            else
            {
                newWayList.Add(save);
            }
        }
    }

    void WayOptimization2()
    {
        // Исправить вторую оптимизацию. Иногла не находит новый путь. Инагда строит только последнюю часть пути.
        allDistance = DistanceSummation(newWayList, newWayList.Count - 1);
        int index = 0;
        List<Vector3> tempList = new List<Vector3>();
        List<Vector3> tempWayList = new List<Vector3>(newWayList);
        for (int i = 1; i < tempWayList.Count - 1; i++)
        {
            finishVector3 = tempWayList[i];
            SearchingAllPoint();
            if (finishPoint != Vector3.zero)
            {
                AddPointToWay();
                WayOptimization();
                // PathDrawing(newWayList, Color.magenta);
            }

            float distance = DistanceSummation(tempWayList, i) + DistanceSummation(newWayList, newWayList.Count - 1);
            if (distance < allDistance)
            {
                allDistance = distance;
                index = i;
                tempList = new List<Vector3>(newWayList);
            }
        }

        lastWayList.AddRange(tempWayList.GetRange(0, index));
        lastWayList.AddRange(tempList);
    }

    void ResetParameters()
    {
        rangeWayPoints.Clear();
        wayPoints.Clear();
        lastWayList.Clear();
        newWayList.Clear();
        pointList.Clear();
        wayList.Clear();
        finishPoint = Vector3.zero;
        math = false;
    }

    float DistanceSummation(List<Vector3> list, int index1)
    {
        float distance = 0;
        for (int i = 0; i < index1; i++)
        {
            distance += Vector3.Distance(list[i], list[i + 1]);
        }

        return distance;
    }

    void PathDrawing(List<Vector3> list, Color color)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            Debug.DrawRay(list[i], list[i + 1] - list[i], color, 1000);
        }
    }
}