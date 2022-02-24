using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Way
{
    Vector3 startVectror3;
    Vector3 finishVectror3;
    List<Vector3> wayList = new List<Vector3>();
    List<Vector3> newWayList = new List<Vector3>();
    List<Vector3> lastWayList = new List<Vector3>();
    List<Vector3> pointList = new List<Vector3>();
    List<Vector3> allPointList = new List<Vector3>();
    List<Vector3> previousPointList = new List<Vector3>();
    float strideLength = 4.0f;
    float oneDegree = 0.0174532862792735f;
    int angleRotation = 90;
    Vector3 finishPoint;
    bool math = false;
    float allDistance = 0;
    [SerializeField] private GameObject prefab;
    public void AddStartAndFinish(Vector3 startVector, Vector3 finishVector)
    {
        startVectror3 = startVector;
        finishVectror3 = finishVector;
        pointList.Add(startVectror3);
        allPointList.Add(startVectror3);
        previousPointList.Add(startVectror3);
    }
    public void StartSearch()
    {
        SearchingAllPoint();
        if (finishPoint != Vector3.zero)
        {
            SearchingNextPoint();
            PathDrawing(wayList, Color.green);
        }
        else
        {
            ResetParameters();
        }
    }
    public void StartOneOptimization()
    {
        if (finishPoint != Vector3.zero)
        {
            WayOptimization();
            PathDrawing(newWayList, Color.red);
        }
        ResetParameters();
    }
    public void StartTwoOptimization()
    {
        if (finishPoint != Vector3.zero)
        {
            WayOptimization();
            PathDrawing(newWayList, Color.red);
            WayOptimization2();
            PathDrawing(lastWayList, Color.white);
        }
        ResetParameters();
    }
    void SearchingAllPoint()
    {
        Vector3 startVector = startVectror3;
        while (pointList.Count < 1000 && pointList.Count > 0)
        {
            pointList = pointList.OrderBy(x => Vector3.Distance(finishVectror3, x)).ToList();
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
                if (Vector3.Distance(tempVector, finishVectror3) <= strideLength)
                {
                    float distance = Vector3.Distance(tempVector, finishVectror3);
                    Ray ray2 = new Ray(tempVector, finishVectror3 - tempVector);
                    if (!Physics.Raycast(ray2, out hit, distance))
                    {
                        finishPoint = tempVector;
                        previousPointList.Add(startVector);
                        allPointList.Add(tempVector);
                        wayList.Add(finishVectror3);
                        math = true;
                        return;
                    }
                }
                Debug.DrawRay(startVector, tempVector - startVector, Color.blue, 5);
                pointList.Add(tempVector);
                previousPointList.Add(startVector);
                allPointList.Add(tempVector);
            }
        }
    }
    bool FindingNearestVectors(Vector3 tempVector)
    {
        foreach (var point in allPointList)
        {
            if (Vector3.Distance(tempVector, point) < strideLength - 0.1f)
            {
                return true;
            }
        }
        return false;
    }
    void SearchingNextPoint()
    {
        if (finishPoint == startVectror3)
        {
            wayList.Add(startVectror3);
            return;
        }
        else
        {
            int index = allPointList.IndexOf(finishPoint);
            wayList.Add(allPointList[index]);
            finishPoint = previousPointList[index];
            SearchingNextPoint();
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
            ResetParameters();
            finishVectror3 = tempWayList[i];
            SearchingAllPoint();
            if (finishPoint != Vector3.zero)
            {
                SearchingNextPoint();
                WayOptimization();
                PathDrawing(newWayList, Color.magenta);
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
        lastWayList.Clear();
        newWayList.Clear();
        pointList.Clear();
        allPointList.Clear();
        previousPointList.Clear();
        wayList.Clear();
        pointList.Add(startVectror3);
        allPointList.Add(startVectror3);
        previousPointList.Add(startVectror3);
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