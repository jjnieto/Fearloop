using Maze;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public RoomDatabase roomDatabase;

    public void CreateRoom(Room room, int family, Transform parent, RoomDatabase.BuildingSet buildingSet = null)
    {
        Vector3 pos= Vector3.zero;

		RoomDatabase.BuildingSet bSet= new RoomDatabase.BuildingSet();

		bSet = buildingSet;
		if (buildingSet == null)
		{
			bSet.wallIndex = 0;
			bSet.cornerIndex = 0;
			bSet.doorIndex = 0;
			bSet.floorIndex = 0;
			bSet.ceilingIndex = 0;
		}

        int doorIndex = 0;
		int wallIndex = 0;
		int cornerIndex = 0;
		int floorIndex = 0;
		int ceilingIndex = 0;
		//Puerta norte
		int northDoor = Random.Range(0, (int)(room.size.x)-2)+1;
		//Puerta sur
		int southDoor = Random.Range(0, (int)(room.size.x) - 2)+1;
		//Instantiate walls
		for (int x = 1; x < room.size.x - 1; x++)
		{
			doorIndex = bSet.doorIndex != -1 ? bSet.doorIndex : Random.Range(0, roomDatabase.families[family].doorPrefabs.Count);
			wallIndex = bSet.wallIndex != -1 ? bSet.wallIndex : Random.Range(0, roomDatabase.families[family].wallPrefabs.Count);
			//MURO SUR
			if (room.connections[(int)Connection.connections.S]!=null && x == southDoor)
				Instantiate(roomDatabase.families[family].doorPrefabs[doorIndex], new Vector3(x * 5, 0, 0), Quaternion.Euler(0, 270, 0), parent);
			else
				Instantiate(roomDatabase.families[family].wallPrefabs[wallIndex], new Vector3(x * 5, 0, 0), Quaternion.Euler(0, 270, 0), parent);
			//MURO NORTE
			if (room.connections[(int)Connection.connections.N] != null && x == northDoor)
				Instantiate(roomDatabase.families[family].doorPrefabs[doorIndex], new Vector3((x + 1) * 5, 0, room.size.y * 5), Quaternion.Euler(0, 90, 0), parent);
			else
				Instantiate(roomDatabase.families[family].wallPrefabs[wallIndex], new Vector3((x + 1) * 5, 0, room.size.y * 5), Quaternion.Euler(0, 90, 0), parent);
		}

		//Puerta oeste
		int westDoor = Random.Range(0, (int)(room.size.y) - 2)+1;
		//Puerta sur
		int eastDoor = Random.Range(0, (int)(room.size.y) - 2)+1;

		Debug.Log(room.connections[(int)Connection.connections.S]);
		Debug.Log(room.connections[(int)Connection.connections.E]+ " - "+ eastDoor);

		for (int z = 1; z < room.size.y - 1; z++) {

			doorIndex = bSet.doorIndex != -1 ? bSet.doorIndex : Random.Range(0, roomDatabase.families[family].doorPrefabs.Count);
			wallIndex = bSet.wallIndex != -1 ? bSet.wallIndex : Random.Range(0, roomDatabase.families[family].wallPrefabs.Count);
			//MURO OESTE
			if (room.connections[(int)Connection.connections.W] != null && z == westDoor)
				Instantiate(roomDatabase.families[family].doorPrefabs[doorIndex], new Vector3(0, 0, (z + 1) * 5), Quaternion.Euler(0, 0, 0), parent);
			else
				Instantiate(roomDatabase.families[family].wallPrefabs[wallIndex], new Vector3(0, 0, (z+1)*5), Quaternion.Euler(0, 0, 0), parent);

			//MUTO ESTE
			if (room.connections[(int)Connection.connections.E] != null && z == eastDoor)
				Instantiate(roomDatabase.families[family].doorPrefabs[doorIndex], new Vector3(room.size.x * 5, 0, z * 5), Quaternion.Euler(0, 180, 0), parent);
			else
				Instantiate(roomDatabase.families[family].wallPrefabs[wallIndex], new Vector3(room.size.x * 5, 0, z * 5), Quaternion.Euler(0, 180, 0), parent);
		}
		//Corners
		cornerIndex = bSet.cornerIndex != -1 ? bSet.cornerIndex : Random.Range(0, roomDatabase.families[family].cornerPrefabs.Count);
		Instantiate(roomDatabase.families[family].cornerPrefabs[cornerIndex], new Vector3(0, 0, 5), Quaternion.Euler(0, 0, 0), parent);
		cornerIndex = bSet.cornerIndex != -1 ? bSet.cornerIndex : Random.Range(0, roomDatabase.families[family].cornerPrefabs.Count);
		Instantiate(roomDatabase.families[family].cornerPrefabs[cornerIndex], new Vector3(5, 0, room.size.y * 5), Quaternion.Euler(0, 90, 0), parent);
		cornerIndex = bSet.cornerIndex != -1 ? bSet.cornerIndex : Random.Range(0, roomDatabase.families[family].cornerPrefabs.Count);
		Instantiate(roomDatabase.families[family].cornerPrefabs[cornerIndex], new Vector3((room.size.x-1) * 5, 0,0), Quaternion.Euler(0, 270, 0), parent);
		cornerIndex = bSet.cornerIndex != -1 ? bSet.cornerIndex : Random.Range(0, roomDatabase.families[family].cornerPrefabs.Count);
		Instantiate(roomDatabase.families[family].cornerPrefabs[cornerIndex], new Vector3(room.size.x * 5, 0, (room.size.y-1) * 5), Quaternion.Euler(0, 180, 0), parent);


		//Floor
		for (int x = 1; x <= room.size.x; x++)
			for (int z = 1; z <= room.size.y; z++)
			{
				floorIndex = bSet.floorIndex != -1 ? bSet.floorIndex : Random.Range(0, roomDatabase.families[family].floorPrefabs.Count);
				Instantiate(roomDatabase.families[family].floorPrefabs[floorIndex], new Vector3(x * 5, 0, z * 5), Quaternion.Euler(0, 0, 0), parent);
			}
		//Ceiling
		for (int x = 0; x < room.size.x; x++)
			for (int z = 1; z <= room.size.y; z++)
			{
				ceilingIndex = bSet.ceilingIndex != -1 ? bSet.ceilingIndex : Random.Range(0, roomDatabase.families[family].ceilingPrefabs.Count);
				Instantiate(roomDatabase.families[family].ceilingPrefabs[ceilingIndex], new Vector3(x * 5, roomDatabase.heightsPerFamily[family], z * 5), Quaternion.Euler(0, 0, 180), parent);
			}
	}
}
