using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
	public class RoomDatabase : MonoBehaviour
	{
		[System.Serializable]
		public class Family
		{
			public List<GameObject> wallPrefabs;
			public List<GameObject> cornerPrefabs;
			public List<GameObject> doorPrefabs;
			public List<GameObject> floorPrefabs;
			public List<GameObject> ceilingPrefabs;
		}

		public List<Family> families;

		public List<float> heightsPerFamily;

		public class BuildingSet
		{
			public int wallIndex;
			public int cornerIndex;
			public int doorIndex;
			public int floorIndex;
			public int ceilingIndex;

			public static BuildingSet AllRandom
			{
				get
				{
					return new BuildingSet
					{
						wallIndex = -1,
						cornerIndex = -1,
						doorIndex = -1,
						floorIndex = -1,
						ceilingIndex = -1
					};
				}
			}
		}
	}
}
