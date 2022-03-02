using System.Collections.Generic;
using UnityEngine;
public class Test
{
    public void Testing()
    {
        Testing2 testing2 = new Testing2();
        List<Vector3> workPointList = new List<Vector3>();
        workPointList.Add(Vector3.zero);
        List<Vector3> allPointList = new List<Vector3>();
        int value = 0;
        while (value < 10000)
        {
            value++;
            if (allPointList.Count > 500)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = allPointList[0];
                allPointList.RemoveAt(0);
            }
            Vector3 startVector3 = workPointList[0];
            workPointList.RemoveAt(0);
            testing2.test2(startVector3, allPointList, workPointList);
        }
        Debug.Log("stop");
    }
}

public struct Testing2
{
    public void test2(Vector3 startVector3, List<Vector3> allPointList, List<Vector3> workPointList)
    {
        TTT ttt = new TTT();
        int angleRotation = 45;
        float oneDegree = 0.0174532862792735f;
        float currentAngle = oneDegree * angleRotation;
        for (float i = 0; i <= 360 / angleRotation; i++)
        {
            Vector3 newVector3 = ttt.FindingNearestVectors(startVector3, currentAngle);
            bool match = false;
            foreach (var point in allPointList)
            {
                if (Vector3.Distance(newVector3, point) < 2 - 0.1f)
                {
                    match = true;
                }
            }
            if (match == false)
            {
                workPointList.Add(newVector3);
                allPointList.Add(newVector3);
                Debug.DrawLine(startVector3, newVector3, Color.green, 10);
            }
            currentAngle += oneDegree * angleRotation;
        }

    }

}
public struct TTT
{
    public Vector3 FindingNearestVectors(Vector3 startVector3, float currentAngle)
    {
        float x = startVector3.x + 2 * Mathf.Sin(currentAngle);
        float z = startVector3.z - 2 * (1 - Mathf.Cos(currentAngle));
        return new Vector3(x, 0, z + 2);
    }
}
