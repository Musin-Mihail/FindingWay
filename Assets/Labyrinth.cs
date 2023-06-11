using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Labyrinth
{
    private const int SizeRoom = 10;
    private readonly List<Vector3> _allRoomWall = new();
    private readonly List<Vector3> _allRoomDoor = new();
    private GameObject _parent;

    public Vector3 CreateLabyrinth()
    {
        var lastRoom = Vector3.zero;
        _parent = new GameObject();
        const int sizeLabyrinth = 15;
        var allRoomVector3 = new List<Vector3>();
        var newVector3 = new List<Vector3> { Vector3.zero };
        for (var x = 0; x < sizeLabyrinth * SizeRoom; x += SizeRoom)
        {
            for (var y = 0; y < sizeLabyrinth * SizeRoom; y += SizeRoom)
            {
                for (var z = 0; z < sizeLabyrinth * SizeRoom; z += SizeRoom)
                {
                    lastRoom = new Vector3(x, y, z);
                    allRoomVector3.Add(lastRoom);
                }
            }
        }

        var tempRoomVector3 = new List<Vector3>(allRoomVector3);
        RandomListVector3(allRoomVector3);
        while (allRoomVector3.Count > 0)
        {
            var newVector = newVector3[0];
            newVector3.RemoveAt(0);
            for (var i = 0; i < allRoomVector3.Count; i++)
            {
                if (!(Vector3.Distance(newVector, allRoomVector3[i]) <= SizeRoom)) continue;
                //Debug.DrawLine(newVector, allRoomVector3[i], Color.red, 10);
                newVector3.Add(allRoomVector3[i]);
                CreateDoor(newVector, allRoomVector3[i]);
                allRoomVector3.RemoveAt(i);
                RandomListVector3(newVector3);
            }
        }

        foreach (var room in tempRoomVector3)
        {
            CreateRoom(room);
        }

        return lastRoom;
    }

    private void RandomListVector3(IList<Vector3> list)
    {
        for (var i = 0; i < list.Count; i++)
        {
            var temp = list[i];
            var randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void CreateDoor(Vector3 vector1, Vector3 vector2)
    {
        _allRoomDoor.Add((vector1 + vector2) / 2);
    }

    private void CreateRoom(Vector3 vector1)
    {
        var newWallVector3 = vector1 + (Vector3.down * (SizeRoom / 2));
        AddWall(newWallVector3, new Vector3(SizeRoom, 1, SizeRoom));

        newWallVector3 = vector1 + (Vector3.up * (SizeRoom / 2));
        AddWall(newWallVector3, new Vector3(SizeRoom, 1, SizeRoom));

        newWallVector3 = vector1 + (Vector3.forward * (SizeRoom / 2));
        AddWall(newWallVector3, new Vector3(SizeRoom, SizeRoom, 1));

        newWallVector3 = vector1 + (Vector3.back * (SizeRoom / 2));
        AddWall(newWallVector3, new Vector3(SizeRoom, SizeRoom, 1));

        newWallVector3 = vector1 + (Vector3.left * (SizeRoom / 2));
        AddWall(newWallVector3, new Vector3(1, SizeRoom, SizeRoom));

        newWallVector3 = vector1 + (Vector3.right * (SizeRoom / 2));
        AddWall(newWallVector3, new Vector3(1, SizeRoom, SizeRoom));
    }

    private void AddWall(Vector3 newWallVector3, Vector3 scale)
    {
        if (CheckMatch(newWallVector3))
        {
            return;
        }

        var boxDown = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boxDown.transform.position = newWallVector3;
        boxDown.transform.localScale = scale;
        boxDown.transform.parent = _parent.transform;
        _allRoomWall.Add(newWallVector3);
    }

    private bool CheckMatch(Vector3 newWallVector3)
    {
        return _allRoomDoor.Any(door => newWallVector3 == door) || _allRoomWall.Any(wall => newWallVector3 == wall);
    }
}