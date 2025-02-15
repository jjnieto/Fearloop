using Maze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection
{
	public enum connections
	{
		N, S, E, W, U, D
	}

	public Room A, B;

	public connections conA, conB;
}
