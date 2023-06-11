using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCamera
{
    Camera pathCamera;

    public IEnumerator Move(List<Vector3> list, Camera cam)
    {
        pathCamera = cam;
        List<Vector3> path = new List<Vector3>(list);
        Debug.Log(path.Count);
        pathCamera.transform.LookAt(path[path.Count - 1]);
        while (path.Count > 0)
        {
            Quaternion quaternion = Quaternion.LookRotation(path[path.Count - 1] - pathCamera.transform.position);
            while (pathCamera.transform.rotation != quaternion)
            {
                Vector3 test = Vector3.RotateTowards(pathCamera.transform.forward, path[path.Count - 1] - pathCamera.transform.position, 1.0f * Time.deltaTime, 3);
                test += pathCamera.transform.position;
                pathCamera.transform.rotation = Quaternion.LookRotation(test - pathCamera.transform.position);
                yield return new WaitForSeconds(0.001f);
            }

            while (pathCamera.transform.position != path[path.Count - 1])
            {
                pathCamera.transform.position = Vector3.MoveTowards(pathCamera.transform.position, path[path.Count - 1], 5.0f * Time.deltaTime);
                yield return new WaitForSeconds(0.001f);
            }

            path.RemoveAt(path.Count - 1);
            yield return new WaitForSeconds(0.5f);
        }
    }
}