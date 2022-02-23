using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class FindingWay : MonoBehaviour
{
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform finishTransform;
    Vector3 startVectror3;
    Vector3 finishVectror3;
    public List<Vector3> wayList = new List<Vector3>();
    public List<Vector3> newWayList = new List<Vector3>();
    public List<Vector3> tempWayList = new List<Vector3>();
    public List<Vector3> lastWayList = new List<Vector3>();
    public List<Vector3> pointList = new List<Vector3>();
    public List<Vector3> allPointList = new List<Vector3>();
    public List<Vector3> previousPointList = new List<Vector3>();
    float strideLength = 2.0f;
    float oneDegree = 0.0174532862792735f;
    int angleRotation = 10;
    Vector3 finishPoint;
    bool math = false;
    float allDistance = 0;
    void Start()
    {
        startVectror3 = startTransform.position;
        finishVectror3 = finishTransform.position;
        pointList.Add(startVectror3);
        allPointList.Add(startVectror3);
        previousPointList.Add(startVectror3);
    }
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     SearchingAllPoint();
        //     if (finishPoint != Vector3.zero)
        //     {
        //         SearchingNextPoint();
        //         PathDrawingGreen();
        //         WayOptimization();
        //         PathDrawingRed();
        //         WayOptimization2();
        //     }
        // }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SearchingAllPoint();
            SearchingNextPoint();
            PathDrawingGreen();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            WayOptimization();
            PathDrawingRed();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            WayOptimization2();
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
        allDistance = MeasureDistance(newWayList, newWayList.Count - 1);
        int index = 0;
        List<Vector3> tempList = new List<Vector3>();
        tempWayList = new List<Vector3>(newWayList);
        for (int i = 1; i < tempWayList.Count - 1; i++)
        {
            newWayList.Clear();
            pointList.Clear();
            allPointList.Clear();
            previousPointList.Clear();
            wayList.Clear();
            pointList.Add(startVectror3);
            allPointList.Add(startVectror3);
            previousPointList.Add(startVectror3);
            finishPoint = Vector3.zero;
            finishVectror3 = tempWayList[i];
            math = false;
            SearchingAllPoint();
            if (finishPoint != Vector3.zero)
            {
                SearchingNextPoint();
                WayOptimization();
                // PathDrawingMagenta();
            }
            float distance = MeasureDistance(tempWayList, i) + MeasureDistance(newWayList, newWayList.Count - 1);
            if (distance < allDistance)
            {
                allDistance = distance;
                index = i;
                tempList = new List<Vector3>(newWayList);
            }
        }
        lastWayList.AddRange(tempWayList.GetRange(0, index));
        lastWayList.AddRange(tempList);
        PathDrawingWhite();
    }
    float MeasureDistance(List<Vector3> list, int index1)
    {
        float distance = 0;
        for (int i = 0; i < index1; i++)
        {
            distance += Vector3.Distance(list[i], list[i + 1]);
        }
        return distance;
    }
    Vector3 AngleCalculationAroundY(float angle, Vector3 center, float y)
    {
        float x = center.x + strideLength * Mathf.Sin(angle);
        float z = center.z - strideLength * (1 - Mathf.Cos(angle));
        return new Vector3(x, y, z + strideLength) - center;
    }
    void SearchingAllPoint()
    {
        while (pointList.Count < 2000)
        {
            pointList = pointList.OrderBy(x => Vector3.Distance(finishVectror3, x)).ToList();
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
            if (math == true)
            {
                return;
            }
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
                if (Vector3.Distance(tempVector, finishVectror3) <= strideLength)
                {
                    float distance = Vector3.Distance(tempVector, finishVectror3);
                    Ray ray2 = new Ray(tempVector, finishVectror3 - tempVector);
                    if (Physics.Raycast(ray2, out hit, distance))
                    {
                    }
                    else
                    {
                        finishPoint = tempVector;
                        previousPointList.Add(startVector);
                        allPointList.Add(tempVector);
                        wayList.Add(finishVectror3);
                        math = true;
                    }
                }
                // Debug.DrawRay(startVector, tempVector - startVector, Color.blue, 100);
                pointList.Add(tempVector);
                previousPointList.Add(startVector);
                allPointList.Add(tempVector);
            }
        }
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
    void PathDrawingGreen()
    {
        float distance = 0;
        for (int i = 0; i < wayList.Count - 1; i++)
        {
            distance += Vector3.Distance(wayList[i], wayList[i + 1]);
            Debug.DrawRay(wayList[i], wayList[i + 1] - wayList[i], Color.green, 20);
        }
        print("Green - " + distance);
    }
    void PathDrawingRed()
    {
        float distance = 0;
        for (int i = 0; i < newWayList.Count - 1; i++)
        {
            distance += Vector3.Distance(newWayList[i], newWayList[i + 1]);
            Debug.DrawRay(newWayList[i], newWayList[i + 1] - newWayList[i], Color.red, 20);
        }
        print("red - " + distance);
    }
    void PathDrawingWhite()
    {
        float distance = 0;
        for (int i = 0; i < lastWayList.Count - 1; i++)
        {
            distance += Vector3.Distance(lastWayList[i], lastWayList[i + 1]);
            Debug.DrawRay(lastWayList[i], lastWayList[i + 1] - lastWayList[i], Color.white, 20);
        }
        allDistance = distance;
        print("white - " + distance);
    }
    void PathDrawingMagenta()
    {
        for (int i = 0; i < newWayList.Count - 1; i++)
        {
            Debug.DrawRay(newWayList[i], newWayList[i + 1] - newWayList[i], Color.magenta, 20);
        }
    }
}