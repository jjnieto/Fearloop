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
	private float maxCharge = 1.0f; // Carga m�xima
	public float minEnergyTrigger = 0.5f;
	private bool isBeingCharged = false; // Controla si est� siendo cargado
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
        
		// Asegura obtener el material del Renderer para no modificar el material compartido entre m�ltiples objetos
		material = GetComponent<Renderer>().material;

		// Establece el color de emisi�n inicial
		SetEmissionColor(LaserManager.Instance.GetColor(laserType));
		foreach(GameObject go in meters)
			go.GetComponent<Renderer>().material = meterOffMaterial;
	}

	// M�todo p�blico para cambiar el color de emisi�n
	public void SetEmissionColor(Color newColor)
	{
		Color finalColor = newColor * Mathf.LinearToGammaSpace(4);
		material.SetColor("_EmissionColor", finalColor);
		material.EnableKeyword("_EMISSION");
		// Aseg�rate de que la emisi�n se recalcula en la iluminaci�n global si es necesario
		DynamicGI.SetEmissive(GetComponent<Renderer>(), newColor);
	}
	void Update()
	{
		if (isBeingCharged)
		{
			// Reset el estado de carga para el pr�ximo frame
			isBeingCharged = false;
		}
		else if (currentCharge > 0)
		{
			// Disminuir la carga gradualmente si no est� siendo cargado
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
		// Indicar que est� siendo cargado
		isBeingCharged = true;

		// Incrementa la carga basada en el tiempo de exposici�n al l�ser
		currentCharge = Mathf.Min(maxCharge, currentCharge + deltaTime / chargeTime);

		if (currentCharge >= maxCharge && onEnergyComplete != null)
			onEnergyComplete.Invoke();
	}
}
