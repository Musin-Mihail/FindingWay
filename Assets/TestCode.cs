using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    public Transform player;
    public Transform target;
    Vector3 tempPoint1;
    Vector3 centerCollider;
    float leftDistans;
    float rightDistans;
    public List<Collider> AllCollider;
    Vector3 newDirection;

    void Start()
    {
        leftDistans = 0;
        rightDistans = 0;
        tempPoint1 = Vector3.zero;
        Application.targetFrameRate = 60;
        CreateNewDirection(target.position - player.position);
        StartCoroutine(RayLaunch());
    }

    void Update()
    {
        // Debug.DrawRay(player.position, player.forward*2, Color.green);
        player.rotation = Quaternion.LookRotation(newDirection, Vector3.forward);
        Vector3 newVector = player.position + player.forward / 10;
        newVector.z = 0;
        player.position = newVector;
    }

    IEnumerator RayLaunch()
    {
        while (true)
        {
            float randomValue = Random.Range(0.3f, 1.7f);
            Vector3 newVector = player.position - player.right + player.right * randomValue;
            RaycastHit[] test = Physics.RaycastAll(newVector, player.forward, 2);
            Debug.DrawRay(newVector, player.forward, Color.green);
            if (test.Length > 0)
            {
                foreach (var item in test)
                {
                    AllCollider.Add(item.collider);
                }

                PathChoice();
            }
            else
            {
                CreateNewDirection(target.position - player.position);
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    void CreateNewDirection(Vector3 vector)
    {
        newDirection = Vector3.RotateTowards(player.forward, vector, 0.1f, 1);
    }

    void PathChoice()
    {
        foreach (var item in AllCollider)
        {
            centerCollider = item.bounds.center;
            tempPoint1.Set(centerCollider.x + 100, centerCollider.y, centerCollider.z);
            DistanceСalculation(tempPoint1, item);
            tempPoint1.Set(centerCollider.x - 100, centerCollider.y, centerCollider.z);
            DistanceСalculation(tempPoint1, item);
            tempPoint1.Set(centerCollider.x, centerCollider.y + 100, centerCollider.z);
            DistanceСalculation(tempPoint1, item);
            tempPoint1.Set(centerCollider.x, centerCollider.y - 100, centerCollider.z);
            DistanceСalculation(tempPoint1, item);
        }

        if (leftDistans > rightDistans)
        {
            CreateNewDirection(-player.right + player.forward);
        }
        else
        {
            CreateNewDirection(player.right + player.forward);
        }

        leftDistans = 0;
        rightDistans = 0;
        AllCollider.Clear();
    }

    void DistanceСalculation(Vector3 vector, Collider collider)
    {
        Vector3 tempPoint = collider.bounds.ClosestPoint(vector);
        if (Vector3.SignedAngle(tempPoint - player.position, player.forward, Vector3.forward) < 0)
        {
            leftDistans += Vector3.Distance(player.position, tempPoint);
            leftDistans += Vector3.Distance(target.position, tempPoint);
        }
        else
        {
            rightDistans += Vector3.Distance(player.position, tempPoint);
            rightDistans += Vector3.Distance(target.position, tempPoint);
        }
    }
}