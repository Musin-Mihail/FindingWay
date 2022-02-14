using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FindingWay : MonoBehaviour
{
    public Transform startTransform;
    public Transform finishTransform;
    public List<Vector3> wayList = new List<Vector3>();
    List<Vector3> pointList = new List<Vector3>();
    List<Vector3> allPointList = new List<Vector3>();
    public Transform prefab;
    public float strideLength = 2;
    bool match = false;
    // float angle = 0;
    void Start()
    {
        pointList.Add(startTransform.position);
        allPointList.Add(startTransform.position);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SearchingAllPoint();
            if (SearchingNextPoint(startTransform.position, 1) == true)
            {
                wayList.Add(startTransform.position);
                PathDrawing();
            }
        }
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
            float angleRotation = 0;
            for (float i = 0; i < 36; i++)
            {
                Vector3 tempVector = AngleCalculationAroundY(angleRotation, startVector) + startVector;
                if (Vector3.Distance(tempVector, finishTransform.position) < 2f)
                {
                    print("1");
                    allPointList.Add(finishTransform.position);
                    return;
                }
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
                        Debug.DrawRay(startVector, tempVector - startVector, Color.blue, 100);
                        pointList.Add(tempVector);
                        allPointList.Add(tempVector);
                    }
                }
                angleRotation += 0.1745328627927352f;
            }
        }
    }
    bool SearchingNextPoint(Vector3 startVector, int value)
    {
        value++;
        if (value > 100)
        {
            return false;
        }
        float angle = 0;
        float Сторона = Vector3.Dot(Vector3.right, finishTransform.position - startVector);
        if (Сторона > 0)
        {
            angle = 0 + Vector3.Angle(Vector3.forward, finishTransform.position - startVector);
        }
        else
        {
            angle = 0 - Vector3.Angle(Vector3.forward, finishTransform.position - startVector);
        }
        float angleRotation = 0.0174532862792735f * angle;
        for (float i = 0; i < 36; i++)
        {
            if (match == false)
            {
                Vector3 tempVector = AngleCalculationAroundY(angleRotation, startVector) + startVector;
                if (Vector3.Distance(tempVector, finishTransform.position) < 2f)
                {
                    print("1");
                    match = true;
                    wayList.Add(finishTransform.position);
                    return true;
                }
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
                    Debug.DrawRay(startVector, tempVector - startVector, Color.blue, 20);
                    RaycastHit hit;
                    Ray ray = new Ray(startVector, tempVector - startVector);
                    if (Physics.Raycast(ray, out hit, strideLength))
                    {
                    }
                    else
                    {

                        allPointList.Add(tempVector);
                        if (SearchingNextPoint(tempVector, value) == true)
                        {
                            wayList.Add(tempVector);
                            return true;
                        }

                    }
                }
                angleRotation += 0.1745328627927352f;
            }
        }
        return false;
    }
    void PathDrawing()
    {
        for (int i = 0; i < wayList.Count - 1; i++)
        {
            Debug.DrawRay(wayList[i], wayList[i + 1] - wayList[i], Color.red, 20);
        }
    }
}