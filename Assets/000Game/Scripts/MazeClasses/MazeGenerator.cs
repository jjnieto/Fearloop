using UnityEngine;
using Maze;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{

	public Maze.Maze maze;
	public RoomGenerator roomGenerator;

	private void Start()
	{
		Room roomA = new Room("Room A", new Vector2(3,3));
		roomA.name = "room A";

		Room roomB = new Room("Room B", new Vector2(3, 4));

		Room roomC = new Room("Room C", new Vector2(6, 4));

		maze.AddRoom(roomA);
		maze.AddRoom(roomB);
		maze.AddRoom(roomC);

		maze.ConnectRooms(roomA, roomB, Connection.connections.N, Connection.connections.S);
		maze.ConnectRooms(roomA, roomC, Connection.connections.S, Connection.connections.S);
		maze.ConnectRooms(roomB, roomC, Connection.connections.N, Connection.connections.E);
		maze.ConnectRooms(roomA, roomA, Connection.connections.W, Connection.connections.E);

		maze.CheckIntegrity(true);

		GameObject allRooms = new GameObject("allRooms");
		GameObject RoomA = new GameObject("Room A");
		RoomA.transform.parent = allRooms.transform;

		roomGenerator.CreateRoom(roomC, 2, RoomA.transform, RoomDatabase.BuildingSet.AllRandom);

	}
}
