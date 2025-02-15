using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAxis : MonoBehaviour
{
	public float rotationSpeed = 10f;
	public Vector3 rotationAxis = Vector3.up;

	void Update()
	{
		transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
	}
}
