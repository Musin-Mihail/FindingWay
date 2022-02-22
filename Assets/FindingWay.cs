using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class FindingWay : MonoBehaviour
{
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform finishTransform;
    List<Vector3> wayList = new List<Vector3>();
    List<Vector3> newWayList = new List<Vector3>();
    List<Vector3> pointList = new List<Vector3>();
    List<Vector3> allPointList = new List<Vector3>();
    List<Vector3> previousPointList = new List<Vector3>();
    float strideLength = 2.0f;
    float oneDegree = 0.0174532862792735f;
    int angleRotation = 10;
    Vector3 finishPoint;
    bool math = false;
    void Start()
    {
        pointList.Add(startTransform.position);

        allPointList.Add(startTransform.position);

        previousPointList.Add(startTransform.position);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SearchingAllPoint();
            if (finishPoint != Vector3.zero)
            {
                SearchingNextPoint();
                PathDrawing2();
                WayOptimization();
                PathDrawing();
            }
        }
    }
    void WayOptimization()
    {
        // newWayList = new List<Vector3>(wayList);
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
    Vector3 AngleCalculationAroundY(float angle, Vector3 center, float y)
    {
        float x = center.x + strideLength * Mathf.Sin(angle);
        float z = center.z - strideLength * (1 - Mathf.Cos(angle));
        return new Vector3(x, y, z + strideLength) - center;
    }
    void SearchingAllPoint()
    {
        while (pointList.Count > 0)
        {
            pointList = pointList.OrderBy(x => Vector3.Distance(finishTransform.position, x)).ToList();
            Vector3 startVector = pointList[0];
            pointList.RemoveAt(0);
            float currentAngle = 0;
            currentAngle += oneDegree * Random.Range(0, 100);
            Vector3 tempVector;
            for (float i = 0; i < 360 / angleRotation; i++)
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
        }
    }
    void AddPoin(Vector3 tempVector, Vector3 startVector)
    {
        bool bool1 = false;
        foreach (var point in allPointList)
        {
            if (Vector3.Distance(tempVector, point) < strideLength - 0.1f)
            {
                bool1 = true;
                break;
            }
        }
        if (bool1 == false)
        {
            RaycastHit hit;
            Ray ray = new Ray(startVector, tempVector - startVector);
            if (Physics.Raycast(ray, out hit, strideLength))
            {
            }
            else
            {
                if (Vector3.Distance(tempVector, finishTransform.position) <= strideLength)
                {
                    float distance = Vector3.Distance(tempVector, finishTransform.position);
                    Ray ray2 = new Ray(tempVector, finishTransform.position - tempVector);
                    if (Physics.Raycast(ray2, out hit, distance))
                    {
                    }
                    else
                    {
                        finishPoint = tempVector;
                        previousPointList.Add(startVector);
                        allPointList.Add(tempVector);
                        wayList.Add(finishTransform.position);
                        math = true;
                    }
                }
                Debug.DrawRay(startVector, tempVector - startVector, Color.blue, 100);
                pointList.Add(tempVector);
                previousPointList.Add(startVector);
                allPointList.Add(tempVector);
            }
        }
    }
    void SearchingNextPoint()
    {
        if (finishPoint == startTransform.position)
        {
            wayList.Add(startTransform.position);
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
    void PathDrawing()
    {
        float distance = 0;
        for (int i = 0; i < newWayList.Count - 1; i++)
        {
            distance += Vector3.Distance(newWayList[i], newWayList[i + 1]);
            Debug.DrawRay(newWayList[i], newWayList[i + 1] - newWayList[i], Color.red, 20);
        }
        print("red - " + distance);
    }
    void PathDrawing2()
    {
        float distance = 0;
        for (int i = 0; i < wayList.Count - 1; i++)
        {
            distance += Vector3.Distance(wayList[i], wayList[i + 1]);
            Debug.DrawRay(wayList[i], wayList[i + 1] - wayList[i], Color.green, 20);
        }
        print("Green - " + distance);
    }
}