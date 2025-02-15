using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VRGrid
{
	public class GridGenerator : MonoBehaviour
	{
		public int rows = 10;
		public int cols = 10;
		public float density = 0.5f; // Probabilidad de que una casilla tenga bloque

		public GameObject corridorPrefab; // Prefab para ║
		public GameObject TCorridorPrefab; // Prefab para ╩
		public GameObject crossCorridorPrefab; // Prefab para ╬
		public GameObject cornerPrefab; // Prefab para ╝
		public GameObject endPrefab; // Prefab para ╥

		private char[,] maze;
		private char[] blocks = { '║', '═', '╣', '╩', '╠', '╦', '╬', '╗', '╝', '╚', '╔', '╨', '╡', '╞', '╥' };

		public class Vector4int
		{
			public int n, s, e, w;
			public Vector4int(int n, int s, int e, int w)
			{
				this.n = n;
				this.s = s;
				this.e = e;
				this.w = w;
			}
		};

		private Dictionary<char, Vector4int> connections = new Dictionary<char, Vector4int>()
		{
			{ '║', new Vector4int (1, 1, 0, 0) },
			{ '═', new Vector4int (0, 0, 1, 1) },
			{ '╣', new Vector4int (1, 1, 1, 0) },
			{ '╩', new Vector4int (1, 0, 1, 1) },
			{ '╠', new Vector4int (1, 1, 0, 1) },
			{ '╦', new Vector4int (0, 1, 1, 1) },
			{ '╬', new Vector4int (1, 1, 1, 1) },
			{ '╗', new Vector4int (0, 1, 1, 0) },
			{ '╝', new Vector4int (1, 0, 1, 0) },
			{ '╚', new Vector4int (1, 0, 0, 1) },
			{ '╔', new Vector4int (0, 1, 0, 1) },
			{ '╨', new Vector4int (1, 0, 0, 0) },
			{ '╥', new Vector4int (0, 1, 0, 0) },
			{ '╡', new Vector4int (0, 0, 1, 0) },
			{ '╞', new Vector4int (0, 0, 0, 1) },
		};

		void Start()
		{
			maze = LoadMazeFromFile("maze01");
			PrintMaze(maze);
			//InstantiateMaze(maze);
		}

		private char[,] LoadMazeFromFile(string fileName)
		{
			TextAsset file = Resources.Load<TextAsset>(fileName);
			if (file == null)
			{
				Debug.LogError("File not found: " + fileName);
				return null;
			}

			string[] rows = file.text.Split('\n');
			int rowCount = rows.Length;
			int colCount = 0;
			foreach (string row in rows)
			{
				if (row.Length > colCount)
				{
					colCount = row.Length;
				}
			}

			char[,] maze = new char[rowCount, colCount];

			for (int i = 0; i < rowCount; i++)
			{
				for (int j = 0; j < colCount; j++)
				{
					if (j < rows[i].Length)
					{
						maze[i, j] = rows[i][j];
					}
					else
					{
						maze[i, j] = ' ';
					}
				}
			}

			return maze;
		}

		private void PrintMaze(char[,] maze)
		{
			string mazeString = "";
			for (int i = 0; i < maze.GetLength(0); i++)
			{
				for (int j = 0; j < maze.GetLength(1); j++)
				{
					mazeString += maze[i, j];
				}
				mazeString += "\n";
			}
			Debug.Log(mazeString);
		}
	}
}
