using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class FindingWay : MonoBehaviour
{
    public Transform startTransform;
    public Transform finishTransform;
    List<Vector3> wayList = new List<Vector3>();
    List<Vector3> newWayList = new List<Vector3>();
    List<Vector3> pointList = new List<Vector3>();
    List<Vector3> allPointList = new List<Vector3>();
    List<Vector3> previousPointList = new List<Vector3>();
    List<Vector3> newAllPointList = new List<Vector3>();
    float strideLength = 2;
    float oneDegree = 0.0174532862792735f;
    int angleRotation = 40;
    Vector3 finishPoint;
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
            SearchingNextPoint();
            WayOptimization();
            PathDrawing();
        }
    }
    void WayOptimization()
    {
        RaycastHit hit;
        newWayList.Add(wayList[0]);
        Vector3 vector = wayList[0];
        float distance;
        for (int i = 0; i < wayList.Count - 1; i++)
        {
            distance = Vector3.Distance(vector, wayList[i + 1]);
            Ray ray = new Ray(vector, wayList[i + 1] - vector);
            if (Physics.Raycast(ray, out hit, distance))
            {
                vector = wayList[i];
                newWayList.Add(wayList[i]);
            }
        }
        newWayList.Add(wayList[wayList.Count - 1]);
    }
    Vector3 AngleCalculationAroundY(float angle, Vector3 center)
    {
        float x = center.x + strideLength * Mathf.Sin(angle);
        float z = center.z - strideLength * (1 - Mathf.Cos(angle));
        return new Vector3(x, 0, z + strideLength) - center;
    }
    void SearchingAllPoint()
    {
        while (pointList.Count > 0)
        {
            Vector3 startVector = pointList[0];
            pointList.RemoveAt(0);
            float currentAngle = 0;
            // float angle = 0;
            // float side = Vector3.Dot(Vector3.right, finishTransform.position - startVector);
            // if (side > 0)
            // {
            //     angle += Vector3.Angle(Vector3.forward, finishTransform.position - startVector);
            // }
            // else
            // {
            //     angle -= Vector3.Angle(Vector3.forward, finishTransform.position - startVector);
            // }
            // currentAngle += oneDegree * angle;
            for (float i = 0; i < 360 / angleRotation; i++)
            {
                Vector3 tempVector = AngleCalculationAroundY(currentAngle, startVector) + startVector;
                bool bool1 = false;
                foreach (var point in allPointList)
                {
                    if (Vector3.Distance(tempVector, point) < 1.5f)
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
                        if (Vector3.Distance(tempVector, finishTransform.position) < 2f)
                        {
                            print("1");
                            finishPoint = tempVector;
                            previousPointList.Add(startVector);
                            allPointList.Add(tempVector);
                            wayList.Add(finishTransform.position);
                            return;
                        }
                        Debug.DrawRay(startVector, tempVector - startVector, Color.blue, 100);
                        pointList.Add(tempVector);
                        previousPointList.Add(startVector);
                        allPointList.Add(tempVector);
                    }
                }
                currentAngle += oneDegree * angleRotation;
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
            // Debug.DrawRay(allPointList[index], previousPointList[index] - allPointList[index], Color.red, 100);
            SearchingNextPoint();
        }
    }
    void PathDrawing()
    {
        for (int i = 0; i < newWayList.Count - 1; i++)
        {
            Debug.DrawRay(newWayList[i], newWayList[i + 1] - newWayList[i], Color.red, 20);
        }
    }
}