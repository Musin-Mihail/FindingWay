using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
    private Vector3 currentVector3;
    private List<Vector3> points;
    private Color32 color;
    public RangeWayPoint(Vector3 current)
    {
        currentVector3 = current;
        points = new List<Vector3>();
        points.Add(current);
        color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
    }
    public Vector3 GetCurrentVector3()
    {
        return currentVector3;
    }
    public bool AddPoint3(Vector3 vector)
    {
        foreach (var point in points)
        {
            if (Vector3.Distance(point, vector) < 4 - 0.1f)
            {
                return false;
            }
        }
        points.Add(vector);
        // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // sphere.transform.position = vector;
        // sphere.GetComponent<MeshRenderer>().material.color = color;
        return true;
    }
    public bool Match(Vector3 vector)
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
    float distanceRange = 40;
    public void AddStartAndFinish(Vector3 startVector, Vector3 finishVector)
    {
        startVector3 = startVector;
        originalFinish = finishVector3 = finishVector;
    }
    public void StartSearch()
    {
        finishVector3 = originalFinish;
        rangeWayPoints.Add(new RangeWayPoint(startVector3));
        SearchingAllPoint();
        if (finishPoint != Vector3.zero)
        {
            AddPointToWay();
            PathDrawing(wayList, Color.green);
            WayOptimization();
            PathDrawing(newWayList, Color.red);
            WayOptimization2();
            PathDrawing(lastWayList, Color.white);
        }
    }
    void SearchingAllPoint()
    {
        ResetParameters();
        pointList.Add(startVector3);
        Vector3 startVector = startVector3;
        int value = 0;
        while (value < 10000 && pointList.Count > 0)
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
            RaycastHit hit;
            Ray ray = new Ray(startVector, tempVector - startVector);
            if (!Physics.Raycast(ray, out hit, strideLength))
            {
                if (Vector3.Distance(tempVector, finishVector3) <= strideLength)
                {
                    float distance = Vector3.Distance(tempVector, finishVector3);
                    Ray ray2 = new Ray(tempVector, finishVector3 - tempVector);
                    if (!Physics.Raycast(ray2, out hit, distance))
                    {
                        finishPoint = tempVector;
                        wayPoints.Add(new WayPoint(tempVector, startVector));
                        wayList.Add(finishVector3);
                        math = true;
                        return;
                    }
                }
                // Debug.DrawRay(startVector, tempVector - startVector, Color.blue, 5);
                pointList.Add(tempVector);
                wayPoints.Add(new WayPoint(tempVector, startVector));

                bool bool1 = false;
                foreach (var range in rangeWayPoints)
                {
                    if (Vector3.Distance(range.GetCurrentVector3(), tempVector) < distanceRange)
                    {
                        bool1 = range.Match(tempVector);
                        if (bool1 == true)
                        {
                            break;
                        }
                    }
                }


                if (bool1 == false)
                {
                    for (int i = rangeWayPoints.Count - 1; i > 0; i--)
                    {
                        if (Vector3.Distance(rangeWayPoints[i].GetCurrentVector3(), tempVector) < distanceRange)
                        {
                            bool1 = rangeWayPoints[i].AddPoint3(tempVector);
                            if (bool1 == true)
                            {
                                break;
                            }
                        }

                    }
                    // foreach (var range in rangeWayPoints)
                    // {
                    //     if (Vector3.Distance(range.GetCurrentVector3(), tempVector) < distanceRange)
                    //     {
                    //         bool1 = range.AddPoint3(tempVector);
                    //         if (bool1 == true)
                    //         {
                    //             break;
                    //         }
                    //     }
                    // }
                }


                if (bool1 == false)
                {
                    rangeWayPoints.Add(new RangeWayPoint(tempVector));

                    // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    // sphere.transform.position = tempVector;
                }
            }
        }
    }
    bool FindingNearestVectors(Vector3 tempVector)
    {
        foreach (var range in rangeWayPoints)
        {
            if (Vector3.Distance(range.GetCurrentVector3(), tempVector) < distanceRange)
            {
                if (range.Match(tempVector) == true)
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
            newWayList.Add(wayList[0]);
            wayList.RemoveAt(0);
            RaycastHit hit;
            Vector3 vector = wayList[0];
            float distance;
            while (wayList.Count > 1)
            {
                for (int i = wayList.Count - 1; i > 0; i--)
                {
                    distance = Vector3.Distance(vector, wayList[i - 1]);
                    Ray ray = new Ray(vector, wayList[i - 1] - vector);
                    if (Physics.Raycast(ray, out hit, distance))
                    {
                    }
                    else
                    {
                        newWayList.Add(wayList[i - 1]);
                        vector = wayList[i - 1];
                        for (int y = 0; y < i; y++)
                        {
                            wayList.RemoveAt(0);
                        }
                        break;
                    }
                }
            }
            newWayList.Add(wayList[0]);
    }
    void WayOptimization2()
    {
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
                //Ошибка в скиске не хватает элементов
                Debug.Log(wayList.Count);
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
            Debug.DrawRay(list[i], list[i + 1] - list[i], color, 5);
        }
    }
}