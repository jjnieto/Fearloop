using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public class LightRayStrip : MonoBehaviour
{
	public LayerMask reflectableLayers;
	public int maxReflections = 5;
	public float rayLength = 100f;
	public float rayWidth = 0.1f; // Nueva variable para el ancho del rayo

	public VolumetricLineStripBehavior lightRay;


	private void Start()
	{
		
	}

	private void Update()
	{
		CastRay(transform.position, transform.forward);
	}

	void CastRay(Vector3 position, Vector3 direction)
	{
		Vector3 origin = position;
		Vector3 dir = direction;

		List<Vector3> positions = new List<Vector3> { origin - position };

		for (int i = 0; i < maxReflections; i++)
		{
			Ray ray = new Ray(origin, dir);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, rayLength, reflectableLayers))
			{
				positions.Add(hit.point - position);
				origin = hit.point;
				dir = Vector3.Reflect(dir, hit.normal);
			}
			else
			{
				positions.Add(origin - position + dir * rayLength);
				break;
			}
		}

		Vector3[] positionsArray = positions.ToArray();
		
		lightRay.UpdateLineVertices(positionsArray);
	}
}
