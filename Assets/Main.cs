using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Main : MonoBehaviour
{
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform finishTransform;
    Way way = new Way();
    //Test test = new Test();
    Labyrinth labyrinth = new Labyrinth();
    void Start()
    {
        way.AddStartAndFinish(startTransform.position, finishTransform.position);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            labyrinth.CreateLabyrinth();
            //way.StartSearch();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //labyrinth.CreateLabyrinth();
            way.StartSearch();
        }
    }

}