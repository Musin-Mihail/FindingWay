using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCamera
{
    private Camera _pathCamera;

    public IEnumerator Move(IEnumerable<Vector3> list, Camera cam)
    {
        _pathCamera = cam;
        var path = new List<Vector3>(list);
        _pathCamera.transform.LookAt(path[^1]);
        while (path.Count > 0)
        {
            Quaternion quaternion = Quaternion.LookRotation(path[^1] - _pathCamera.transform.position);
            while (_pathCamera.transform.rotation != quaternion)
            {
                var transform = _pathCamera.transform;
                var position = transform.position;
                var test = Vector3.RotateTowards(transform.forward, path[^1] - position, 1.0f * Time.deltaTime, 3);
                test += position;
                _pathCamera.transform.rotation = Quaternion.LookRotation(test - position);
                yield return new WaitForSeconds(0.001f);
            }

            while (_pathCamera.transform.position != path[^1])
            {
                _pathCamera.transform.position = Vector3.MoveTowards(_pathCamera.transform.position, path[^1], 5.0f * Time.deltaTime);
                yield return new WaitForSeconds(0.001f);
            }

            path.RemoveAt(path.Count - 1);
            yield return new WaitForSeconds(0.5f);
        }
    }
}