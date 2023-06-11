using System.Collections.Generic;
using UnityEngine;
public class Main : MonoBehaviour
{
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform finishTransform;
    Way way = new Way();
    Labyrinth labyrinth = new Labyrinth();
    List<Vector3> path = new List<Vector3>();
    PathCamera pathCamera = new PathCamera();
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var time = Time.realtimeSinceStartup;
            finishTransform.position = labyrinth.CreateLabyrinth();
            Debug.Log("Время создания лабиринта - " + (Time.realtimeSinceStartup - time));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            way.AddStartAndFinish(startTransform.position, finishTransform.position);
            var time = Time.realtimeSinceStartup;
            path = way.StartSearch();
            Camera.main.transform.LookAt(path[path.Count - 1]);
            Debug.Log("Время поиска маршрута - " + (Time.realtimeSinceStartup - time));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(pathCamera.Move(path, Camera.main));
        }
    }
}