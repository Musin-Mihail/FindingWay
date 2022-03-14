using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Labyrinth
{
    int sizeRoom = 10;
    List<Vector3> allRoomWall = new List<Vector3>();
    List<Vector3> allRoomDoor = new List<Vector3>();
    GameObject parent;
    public Vector3 CreateLabyrinth()
    {
        Vector3 lastRoom = Vector3.zero;
        parent = new GameObject();
        int sizeLabyrinth = 15;
        List<Vector3> allRoomVector3 = new List<Vector3>();
        List<Vector3> newVector3 = new List<Vector3>();
        newVector3.Add(Vector3.zero);
        for (int x = 0; x < sizeLabyrinth * sizeRoom; x += sizeRoom)
        {
            for (int y = 0; y < sizeLabyrinth * sizeRoom; y += sizeRoom)
            {
                for (int z = 0; z < sizeLabyrinth * sizeRoom; z += sizeRoom)
                {
                    lastRoom = new Vector3(x, y, z);
                    allRoomVector3.Add(lastRoom);
                }
            }
        }
        List<Vector3> tempRoomVector3 = new List<Vector3>(allRoomVector3);
        RandomListVector3(allRoomVector3);
        while (allRoomVector3.Count > 0)
        {
            Vector3 newVector = newVector3[0];
            newVector3.RemoveAt(0);
            for (int i = 0; i < allRoomVector3.Count; i++)
            {
                if (Vector3.Distance(newVector, allRoomVector3[i]) <= sizeRoom)
                {
                    //Debug.DrawLine(newVector, allRoomVector3[i], Color.red, 10);
                    newVector3.Add(allRoomVector3[i]);
                    CreateDoor(newVector, allRoomVector3[i]);
                    allRoomVector3.RemoveAt(i);
                    RandomListVector3(newVector3);
                }
            }
        }
        foreach (var room in tempRoomVector3)
        {
            CreateRoom(room);
        }
        return lastRoom;
    }
    void RandomListVector3(List<Vector3> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Vector3 temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    void CreateDoor(Vector3 vector1, Vector3 vector2)
    {
        allRoomDoor.Add((vector1 + vector2) / 2);
    }
    void CreateRoom(Vector3 vector1)
    {
        Vector3 newWallVector3;

        newWallVector3 = vector1 + (Vector3.down * (sizeRoom / 2));
        AddWall(newWallVector3, new Vector3(sizeRoom, 1, sizeRoom));

        newWallVector3 = vector1 + (Vector3.up * (sizeRoom / 2));
        AddWall(newWallVector3, new Vector3(sizeRoom, 1, sizeRoom));

        newWallVector3 = vector1 + (Vector3.forward * (sizeRoom / 2));
        AddWall(newWallVector3, new Vector3(sizeRoom, sizeRoom, 1));

        newWallVector3 = vector1 + (Vector3.back * (sizeRoom / 2));
        AddWall(newWallVector3, new Vector3(sizeRoom, sizeRoom, 1));

        newWallVector3 = vector1 + (Vector3.left * (sizeRoom / 2));
        AddWall(newWallVector3, new Vector3(1, sizeRoom, sizeRoom));

        newWallVector3 = vector1 + (Vector3.right * (sizeRoom / 2));
        AddWall(newWallVector3, new Vector3(1, sizeRoom, sizeRoom));
    }
    void AddWall(Vector3 newWallVector3, Vector3 scale)
    {
        if (CheckMatch(newWallVector3) == true)
        {
            return;
        }
        GameObject boxDown = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boxDown.transform.position = newWallVector3;
        boxDown.transform.localScale = scale;
        boxDown.GetComponent<MeshRenderer>().material.color = new Color32(200,200,200,50);
        boxDown.transform.parent = parent.transform;
        allRoomWall.Add(newWallVector3);
    }
    bool CheckMatch(Vector3 newWallVector3)
    {
        foreach (var door in allRoomDoor)
        {
            if (newWallVector3 == door)
            {
                return true;
            }
        }
        foreach (var wall in allRoomWall)
        {
            if (newWallVector3 == wall)
            {
                return true;
            }
        }
        return false;
    }
}