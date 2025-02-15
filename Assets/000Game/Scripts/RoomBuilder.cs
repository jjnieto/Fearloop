using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
	[Header("Prefabs")]
	public List<GameObject> wall = new List<GameObject>();
	public List<GameObject> corner = new List<GameObject>();
	public List<GameObject> door = new List<GameObject>();
	public List<GameObject> floor = new List<GameObject>();
	public List<GameObject> ceiling = new List<GameObject>();

	[Header("Dimensiones")]
	public float wallLength = 5;
	public float floorLength = 5;
	public float ceilingLength = 5;
	public float ceilingHeigth = 4;
	public int length = 10;
	public int width = 10;

	[Header("Ajustes de esquina")]
	public float cornerOffsetY = 4f; // Altura a la que se colocan las esquinas
	public float cornerGap = 3f;    // Hueco que se deja en cada esquina para no solapar la pared

	[Header("Aspecto visual")]
	public int wallIndex = 0;
	public int floorIndex = 0;
	public int ceilingIndex = 0;

	public bool DoorLeft = false;
	public bool DoorRight = false;
	public bool DoorTop = false;
	public bool DoorBottom = false;

	void Start()
	{
		CreateWalls();
		CreateFloor();
		CreateCeiling();
	}

	void CreateWalls()
	{
		// Verificamos que existan prefabs de pared y esquina
		if (wall.Count <= wallIndex || corner.Count <= wallIndex || door.Count <= wallIndex)
		{
			Debug.LogWarning("No hay prefabs suficientes en 'wall' o 'corner'.");
			return;
		}

		// 1. Calcula la dimensión total en X y Z, teniendo en cuenta el número de paredes
		//    y el hueco en cada esquina (cornerGap).
		float totalX = length * wallLength;
		float totalZ = width * wallLength;

		float halfGap = cornerGap / 2f;
		float halfWallLength = wallLength / 2f;

		float halfX = totalX / 2f;
		float halfZ = totalZ / 2f;

		// 2. Creamos un GameObject padre para las esquinas (opcional, por organización)
		GameObject cornersParent = new GameObject("Corners");
		cornersParent.transform.SetParent(transform);

		// Posiciones de las 4 esquinas
		Vector3 bottomLeft = new Vector3(-halfX, cornerOffsetY, -halfZ);
		Vector3 bottomRight = new Vector3(halfX, cornerOffsetY, -halfZ);
		Vector3 topRight = new Vector3(halfX, cornerOffsetY, halfZ);
		Vector3 topLeft = new Vector3(-halfX, cornerOffsetY, halfZ);

		// Instanciamos esquinas (ajusta rotaciones según necesites)
		Instantiate(corner[wallIndex], bottomLeft, Quaternion.identity, cornersParent.transform).name = "CornerBottomLeft";
		Instantiate(corner[wallIndex], bottomRight, Quaternion.Euler(0, 270, 0), cornersParent.transform).name = "CornerBottomRight";
		Instantiate(corner[wallIndex], topRight, Quaternion.Euler(0, 180, 0), cornersParent.transform).name = "CornerTopRight";
		Instantiate(corner[wallIndex], topLeft, Quaternion.Euler(0, 90, 0), cornersParent.transform).name = "CornerTopLeft";

		// 3. Creamos un GameObject para cada lado, y dentro instanciamos sus paredes.

		// --- LADO INFERIOR ---
		GameObject bottomParent = new GameObject("BottomWalls");
		bottomParent.transform.SetParent(transform);

		for (int i = 0; i < width; i++)
		{
			float zPos = -halfZ +(i * wallLength);
			Vector3 pos = new Vector3(-halfX - halfGap, 0, zPos);
			if (DoorBottom && i == width/2)
			{
				GameObject w = Instantiate(door[wallIndex], pos, Quaternion.Euler(0, 270, 0), bottomParent.transform);
				w.name = "BottomWall_" + i;
			}
			else
			{
				GameObject w = Instantiate(wall[wallIndex], pos, Quaternion.Euler(0, 270, 0), bottomParent.transform);
				w.name = "BottomWall_" + i;
			}
			
		}		

		// --- LADO SUPERIOR ---
		GameObject topParent = new GameObject("TopWalls");
		topParent.transform.SetParent(transform);

		// Rotación: Y=270
		for (int i = 0; i < width; i++)
		{
			float zPos = halfZ - (i * wallLength);
			Vector3 pos = new Vector3(halfX + halfGap, 0, zPos);
			if (DoorTop && i == width/2)
			{
				GameObject w = Instantiate(door[wallIndex], pos, Quaternion.Euler(0, 90, 0), topParent.transform);
				w.name = "TopWall_" + i;
			}
			else
			{
				GameObject w = Instantiate(wall[wallIndex], pos, Quaternion.Euler(0, 90, 0), topParent.transform);
				w.name = "TopWall_" + i;
			}
				
		}

		// --- LADO IZQUIERDO ---
		GameObject leftParent = new GameObject("LeftWalls");
		leftParent.transform.SetParent(transform);

		// Rotación: Y=180
		for (int i = 0; i < length; i++)
		{
			float xPos = -halfX +(i * wallLength);
			Vector3 pos = new Vector3(xPos, 0, halfZ + halfGap);
			if (DoorLeft && i == length/2)
			{
				GameObject w = Instantiate(door[wallIndex], pos, Quaternion.Euler(0, 0, 0), leftParent.transform);
				w.name = "LeftWall_" + i;
			}
			else
			{
				GameObject w = Instantiate(wall[wallIndex], pos, Quaternion.Euler(0, 0, 0), leftParent.transform);
				w.name = "LeftWall_" + i;
			}
		}
		
		// --- LADO DERECHO ---
		GameObject rightParent = new GameObject("RightWalls");
		rightParent.transform.SetParent(transform);

		// Rotación: Y=0
		for (int i = 0; i < length; i++)
		{
			float xPos = halfX -(i * wallLength);
			Vector3 pos = new Vector3(xPos, 0, -halfZ- halfGap);
			if (DoorRight && i == length/2)
			{
				GameObject w = Instantiate(door[wallIndex], pos, Quaternion.Euler(0, 180, 0), rightParent.transform);
				w.name = "RightWall_" + i;
			}
			else
			{
				GameObject w = Instantiate(wall[wallIndex], pos, Quaternion.Euler(0, 180, 0), rightParent.transform);
				w.name = "RightWall_" + i;
			}
		}
	}

	void CreateFloor()
	{
		// Verificamos que existan prefabs de pared y esquina
		if (floor.Count <= floorIndex)
		{
			Debug.LogWarning("No hay prefabs suficientes en 'floor'.");
			return;
		}

		// 1. Calcula la dimensión total en X y Z, teniendo en cuenta el número de paredes
		//    y el hueco en cada esquina (cornerGap).
		float totalX = length * wallLength;
		float totalZ = width * wallLength;

		float halfGap = cornerGap / 2f;
		float halfWallLength = wallLength / 2f;

		float halfX = totalX / 2f;
		float halfZ = totalZ / 2f;


		// --- LADO INFERIOR ---
		GameObject bottomParent = new GameObject("Floor");
		bottomParent.transform.SetParent(transform);

		for (int i = 0; i <= length; i++)
			for (int j = 0; j <= width; j++)
			{
				float xPos = -halfX + (i * wallLength);
				float zPos = -halfZ + (j * wallLength);
				Vector3 pos = new Vector3(xPos, 0, zPos);
				GameObject w = Instantiate(floor[floorIndex], pos, Quaternion.Euler(0, 0, 0), bottomParent.transform);
				w.name = "Floor_" + ((i*length)+j);
			}
	}

	void CreateCeiling()
	{
		// Verificamos que existan prefabs de pared y esquina
		if (ceiling.Count <= ceilingIndex)
		{
			Debug.LogWarning("No hay prefabs suficientes en 'ceiling'.");
			return;
		}

		// 1. Calcula la dimensión total en X y Z, teniendo en cuenta el número de paredes
		//    y el hueco en cada esquina (cornerGap).
		float totalX = length * wallLength;
		float totalZ = width * wallLength;

		float halfGap = cornerGap / 2f;
		float halfWallLength = wallLength / 2f;

		float halfX = totalX / 2f;
		float halfZ = totalZ / 2f;


		// --- LADO INFERIOR ---
		GameObject bottomParent = new GameObject("Ceiling");
		bottomParent.transform.SetParent(transform);

		for (int i = 0; i <= length; i++)
			for (int j = 0; j <= width; j++)
			{
				float xPos = -halfX + (i * wallLength);
				float zPos = -halfZ + (j * wallLength);
				Vector3 pos = new Vector3(xPos, ceilingHeigth, zPos);
				GameObject w = Instantiate(ceiling[ceilingIndex], pos, Quaternion.Euler(0, 0, 0), bottomParent.transform);
				w.name = "Ceiling_" + ((i * length) + j);
			}
	}
}
