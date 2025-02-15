using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MoveAlongPath : MonoBehaviour
{

	public List<Transform> pathPoints; // Lista de puntos que definen el camino
	[Range(0, 1)]
	public float avance = 0f; // Valor de avance de 0 a 1

	public Transform targetTransform; // Transform a rotar
	public float rotationDuration = 1f; // Duración de la rotación en segundos

	private bool isRotating = false; // Indica si el objeto está rotando

	void Update()
	{
		if (pathPoints.Count >= 2)
		{
			Vector3 newPosition = CalculatePositionOnPath(avance);
			transform.position = newPosition;
		}
	}

	public void IncreasePosition(float increasing)
	{
		avance += increasing;
		avance %= 1;
	}

	Vector3 CalculatePositionOnPath(float t)
	{
		t %= 1;
		int segmentCount = pathPoints.Count;
		float segmentLength = 1f / segmentCount;
		int currentSegment = Mathf.FloorToInt(t / segmentLength);
		
		float localT = (t - (currentSegment * segmentLength)) / segmentLength;

		Vector3 start = pathPoints[currentSegment].position;
		Vector3 end = pathPoints[(currentSegment + 1) % segmentCount].position;

		return Vector3.Lerp(start, end, localT);
	}

	public void RotateTarget()
	{
		if (!isRotating)
		{
			StartCoroutine(RotateCoroutine());
		}
	}

	private IEnumerator RotateCoroutine()
	{
		isRotating = true;

		Quaternion startRotation = targetTransform.rotation;
		Quaternion endRotation = startRotation * Quaternion.Euler(0, 90, 0);
		float elapsedTime = 0f;

		while (elapsedTime < rotationDuration)
		{
			Debug.Log(targetTransform.rotation);
			targetTransform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / rotationDuration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		targetTransform.rotation = endRotation;
		isRotating = false;
	}
}
