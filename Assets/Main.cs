using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform finishTransform;
    private readonly Way _way = new();
    private readonly Labyrinth _labyrinth = new();
    private readonly PathCamera _pathCamera = new();
    private List<Vector3> _path = new();
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            finishTransform.position = _labyrinth.CreateLabyrinth();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _way.AddStartAndFinish(startTransform.position, finishTransform.position);
            _path = _way.StartSearch();
            _camera.transform.LookAt(_path[^1]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(_pathCamera.Move(_path, _camera));
        }
    }
}