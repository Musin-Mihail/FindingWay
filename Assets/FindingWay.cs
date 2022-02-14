using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class FindingWay : MonoBehaviour
{
    public Transform startTransform;
    public Transform finishTransform;
    public List<Vector3> wayList = new List<Vector3>();
    List<Vector3> pointList = new List<Vector3>();
    List<Vector3> allPointList = new List<Vector3>();
    List<Vector3> newAllPointList = new List<Vector3>();
    public float strideLength = 2;
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
            Vector3 tempVector = allPointList[0];
            allPointList.RemoveAt(0);
            // newAllPointList = new List<Vector3>(allPointList);
            newAllPointList = allPointList.OrderBy(x => Vector3.Distance(x, finishTransform.position)).ToList();
            if (SearchingNextPoint(tempVector) == true)
            {
                wayList.Add(startTransform.position);
                PathDrawing();
            }
            else
            {
                print("3");
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
                            allPointList.Add(tempVector);
                            return;
                        }
                        Debug.DrawRay(startVector, tempVector - startVector, Color.blue, 100);
                        pointList.Add(tempVector);
                        allPointList.Add(tempVector);
                    }
                }
                angleRotation += 0.1745328627927352f;
            }
        }
    }
    bool SearchingNextPoint(Vector3 startVector)
    {
        for (int i = 0; i < newAllPointList.Count; i++)
        {
            if (Vector3.Distance(startVector, newAllPointList[i]) <= 2.0f)
            {
                if (Vector3.Distance(newAllPointList[i], finishTransform.position) < 2.1f)
                {
                    // Debug.DrawRay(newAllPointList[i], finishTransform.position - newAllPointList[i], Color.red, 20);
                    print("2");
                    wayList.Add(finishTransform.position);
                    wayList.Add(newAllPointList[i]);
                    return true;
                }
                Vector3 tempVector = newAllPointList[i];
                newAllPointList.RemoveAt(i);
                if (SearchingNextPoint(tempVector) == true)
                {
                    // print("4");
                    wayList.Add(tempVector);
                    return true;
                }
                else
                {
                    // print("5");
                }
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