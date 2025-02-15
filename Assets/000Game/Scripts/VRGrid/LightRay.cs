using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LightRay : MonoBehaviour
{
	public LaserManager.LaserType laserType;
	public LayerMask reflectableLayers;
	public LayerMask notifyImpactLayers; // Nuevo LayerMask para notificar impactos
	public int maxReflections = 5;
    public float rayLength = 100f;
    public float rayWidth = 0.1f; // Nueva variable para el ancho del rayo
    public GameObject laserStart, laserEnd;
    public DotAnimatedTexture animatedTexture;
    public float animatedtextureDivisor = 10f;
    public float deltaEnd = 0.5f;
    private LineRenderer lineRenderer;
    public GameObject rotator;
	public Vector3 rotationAxis = Vector3.up;
	public float targetRotationSpeed=-450;
	public float accelerationTime = 5f; // Tiempo para alcanzar la velocidad final
	public float currentSpeed = 0f; // Velocidad actual de rotación
	public bool isActive = false; // Indica si la aceleración está activa
    private bool lighRayOn = false;
    private bool oldActive = false;
    public AudioSource audioSource;
    public AudioClip lightOn;
    public List<GameObject> materials = new List<GameObject>();
    public ParticleSystem startGlow, startParticle, startMuzzle, endGlow, endSpikes, endParticles;

	private void Start()
    {
        
        lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.material = LaserManager.Instance.GetMaterial(laserType);
		lineRenderer.positionCount = 1;
        lineRenderer.startWidth = rayWidth; // Configurar el ancho del rayo al inicio
        lineRenderer.endWidth = rayWidth; // Configurar el ancho del rayo al inicio

		startGlow.GetComponent<ParticleSystemRenderer>().sharedMaterial = LaserManager.Instance.GetGlowMaterial(laserType);
		startParticle.GetComponent<ParticleSystemRenderer>().sharedMaterial = LaserManager.Instance.GetParticlesMaterial(laserType);
		startMuzzle.GetComponent<ParticleSystemRenderer>().sharedMaterial = LaserManager.Instance.GetParticlesMaterial(laserType);
        endGlow.startColor = LaserManager.Instance.GetColor(laserType);
		endSpikes.GetComponent<ParticleSystemRenderer>().sharedMaterial = LaserManager.Instance.GetParticlesMaterial(laserType);
		endParticles.GetComponent<ParticleSystemRenderer>().sharedMaterial = LaserManager.Instance.GetParticlesMaterial(laserType);

		if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        foreach(GameObject go in materials)
        {
			Color finalColor = LaserManager.Instance.GetColor(laserType) * Mathf.LinearToGammaSpace(4);
			go.GetComponent<Renderer>().material.SetColor("_EmissionColor", finalColor);
			GetComponent<Renderer>().material.EnableKeyword("_EMISSION");


			// Asegúrate de que la emisión se recalcula en la iluminación global si es necesario
			DynamicGI.SetEmissive(GetComponent<Renderer>(), LaserManager.Instance.GetColor(laserType));
		}

    }

    private void Update()
    {
		CalculateRotation();
        if (lighRayOn)
        {            
            CastRay(transform.position, transform.forward);
            if(!laserStart.activeSelf)
                laserStart.SetActive(true);
            if(!laserEnd.activeSelf) 
                laserEnd.SetActive(true);
        }
        else
        {
			lineRenderer.positionCount = 0;
			if (laserStart.activeSelf)
				laserStart.SetActive(false);
			if (laserEnd.activeSelf)
				laserEnd.SetActive(false);
		}
       
	}

    void CastRay(Vector3 position, Vector3 direction)
    {
        Vector3 origin = position;
        Vector3 dir = direction;

        List<Vector3> positions = new List<Vector3> { origin };

        for (int i = 0; i < maxReflections; i++)
        {
            Ray ray = new Ray(origin, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayLength))
            {
                positions.Add(hit.point);

				// Notificar impacto en la nueva capa
				if ((notifyImpactLayers.value & (1 << hit.collider.gameObject.layer)) != 0)
				{
					// Llamar a una función específica para manejar la notificación, por ejemplo:
					NotifyImpact(hit.collider.gameObject);
				}

				// Verificar si el rayo se refleja en la capa impactada
				if ((reflectableLayers.value & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    origin = hit.point;
                    dir = Vector3.Reflect(dir, hit.normal);
                }
                else
                {
                    // Si no se refleja, termina el rayo en el punto de impacto
                    break;
                }
            }
            else
            {
                // Si no hay impacto, extender el rayo hasta su longitud máxima
                positions.Add(origin + dir * rayLength);
                break;
            }
        }

        Vector3[] positionsArray = positions.ToArray();
        lineRenderer.positionCount = positionsArray.Length;
        lineRenderer.SetPositions(positionsArray);
        lineRenderer.startWidth = rayWidth; // Actualizar el ancho del rayo en caso de que se cambie en tiempo de ejecución
        lineRenderer.endWidth = rayWidth; // Actualizar el ancho del rayo en caso de que se cambie en tiempo de ejecución

        direction = (positionsArray[positionsArray.Length - 2] - positionsArray[positionsArray.Length - 1]).normalized;
        Vector3 newPosition = positionsArray[positionsArray.Length - 1] + direction * deltaEnd;
        laserEnd.transform.position = newPosition;
        laserEnd.transform.rotation = Quaternion.LookRotation(-direction);
        //Debug.Log(direction + " - " + Quaternion.LookRotation(direction).eulerAngles);
    }

	private void NotifyImpact(GameObject impactedObject)
	{
		EnergyReceiver receiver = impactedObject.GetComponent<EnergyReceiver>();
		if (receiver != null)
		{
			receiver.ReceiveLaser(Time.deltaTime, laserType); // Pasa el deltaTime al método ReceiveLaser
		}
	}

	public void Activate()
	{
		isActive = true;
	}

	public void Deactivate()
	{
		isActive = false;
	}

	public void SwithActivation()
	{
        isActive = !isActive;
    }

	void CalculateRotation()
	{
        if (!oldActive && isActive) //From stoped to start
            if (audioSource)
            {
                audioSource.pitch = 0;
                audioSource.loop = true;
                audioSource.clip = lightOn;
                audioSource.Play();
            }

        
        oldActive = isActive;

        if(!isActive)
            lighRayOn = false;

        if (isActive)
		{            
            audioSource.pitch = currentSpeed / targetRotationSpeed;

            // Incrementar la velocidad gradualmente
            currentSpeed += (targetRotationSpeed / accelerationTime) * Time.deltaTime;

			// Limitar la velocidad a la velocidad objetivo
			if (Mathf.Abs(currentSpeed) >= Mathf.Abs(targetRotationSpeed))
			{
				currentSpeed = targetRotationSpeed;
                lighRayOn = true;
            }
		}
		else if (Mathf.Abs(currentSpeed) > 0f)
		{
            float oldSpeed = currentSpeed;
            audioSource.pitch = currentSpeed / targetRotationSpeed;
            // Decrementar la velocidad gradualmente
            currentSpeed -= (targetRotationSpeed / accelerationTime) * Time.deltaTime;

			// Limitar la velocidad a 0
			if ((oldSpeed >= 0 && currentSpeed < 0) || (oldSpeed <= 0 && currentSpeed > 0))
			{
				currentSpeed = 0f;
                audioSource.loop = false;
                audioSource.clip = null;
                audioSource.Stop();
            }
		}
        animatedTexture.FPS = (int) (Mathf.Abs(currentSpeed) / animatedtextureDivisor);

		rotator.transform.Rotate(rotationAxis * currentSpeed * Time.deltaTime);
	}
}