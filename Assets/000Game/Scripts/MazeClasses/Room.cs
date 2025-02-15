using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
	public class Room
	{
		public string name;
		public Connection[] connections = new Connection[6];
		public Vector2 size;

		public Room(string name, Vector2 size) {
			this.name = name;
			this.size = size;
		}
	}
}
