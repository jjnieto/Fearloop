using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnergyReceiver : MonoBehaviour
{
	public LaserManager.LaserType laserType;
	private Material material;
	public float chargeTime = 5.0f; // Tiempo en segundos para cargar completamente
	public float dischargeTime = 2.5f; // Tiempo en segundos para cargar completamente
	public float currentCharge = 0.0f; // Carga actual
	private float maxCharge = 1.0f; // Carga máxima
	public float minEnergyTrigger = 0.5f;
	private bool isBeingCharged = false; // Controla si está siendo cargado
	public UnityEvent onEnergyComplete, onEnergyDecrease;
	public List<GameObject> meters = new List<GameObject>();
	public Material meterOffMaterial, meterOnMaterial;
	private int currentPanelsOn = 0;
	public AudioSource audioSource;
	public AudioClip stepIn, stepOut;

	void Start()
	{
        if (audioSource==null)
        {
			audioSource = GetComponent<AudioSource>();
		}
        
		// Asegura obtener el material del Renderer para no modificar el material compartido entre múltiples objetos
		material = GetComponent<Renderer>().material;

		// Establece el color de emisión inicial
		SetEmissionColor(LaserManager.Instance.GetColor(laserType));
		foreach(GameObject go in meters)
			go.GetComponent<Renderer>().material = meterOffMaterial;
	}

	// Método público para cambiar el color de emisión
	public void SetEmissionColor(Color newColor)
	{
		Color finalColor = newColor * Mathf.LinearToGammaSpace(4);
		material.SetColor("_EmissionColor", finalColor);
		material.EnableKeyword("_EMISSION");
		// Asegúrate de que la emisión se recalcula en la iluminación global si es necesario
		DynamicGI.SetEmissive(GetComponent<Renderer>(), newColor);
	}
	void Update()
	{
		if (isBeingCharged)
		{
			// Reset el estado de carga para el próximo frame
			isBeingCharged = false;
		}
		else if (currentCharge > 0)
		{
			// Disminuir la carga gradualmente si no está siendo cargado
			currentCharge = Mathf.Max(0.0f, currentCharge - Time.deltaTime / dischargeTime);
			if (currentCharge <= minEnergyTrigger && onEnergyDecrease != null)
				onEnergyDecrease.Invoke();
		}

		int totalSegments = (int)(currentCharge * (float)meters.Count / maxCharge);

		if(currentPanelsOn < totalSegments)
		{
			//Beep On
			Debug.Log("ON "+currentPanelsOn);
			if (audioSource != null)
				audioSource.PlayOneShot(stepIn);
		}
		else if(currentPanelsOn > totalSegments)
		{
			//Beel Off
			Debug.Log("OFF "+currentPanelsOn);
			if (audioSource != null)
				audioSource.PlayOneShot(stepOut);
		}
		currentPanelsOn = totalSegments;
		for (int i=0; i< meters.Count; i++)
		{
			if (i < totalSegments)
			{
				meters[i].GetComponent<Renderer>().material = meterOnMaterial;
				meters[i].GetComponent<Renderer>().material.SetColor("_EmissionColor", LaserManager.Instance.GetColor(laserType) * Mathf.LinearToGammaSpace(4));
				meters[i].GetComponent<Renderer>().material.color=LaserManager.Instance.GetColor(laserType);
			}
			else
				meters[i].GetComponent<Renderer>().material = meterOffMaterial;
		}
	}

	public void ReceiveLaser(float deltaTime, LaserManager.LaserType color)
	{
		if (color != laserType)
			return;
		// Indicar que está siendo cargado
		isBeingCharged = true;

		// Incrementa la carga basada en el tiempo de exposición al láser
		currentCharge = Mathf.Min(maxCharge, currentCharge + deltaTime / chargeTime);

		if (currentCharge >= maxCharge && onEnergyComplete != null)
			onEnergyComplete.Invoke();
	}
}
