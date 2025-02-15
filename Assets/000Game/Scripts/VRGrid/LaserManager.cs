using UnityEngine;

public class LaserManager : MonoBehaviour
{
	public enum LaserType
	{
		Red,
		Green,
		Blue,
		Yellow,
		Magenta
	}
	public static LaserManager Instance { get; private set; }

	[System.Serializable]
	public struct LaserMaterial
	{
		public LaserType type;
		public Material material;
		public Color color;
		public Material startGlow, startParticles;
	}

	public LaserMaterial[] laserMaterials;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// Método para obtener el material por tipo de láser
	public Material GetMaterial(LaserType type)
	{
		foreach (var laserMaterial in laserMaterials)
		{
			if (laserMaterial.type == type)
				return laserMaterial.material;
		}
		Debug.LogError("Material not found for type: " + type);
		return null;
	}

	public Material GetGlowMaterial(LaserType type)
	{
		foreach (var laserMaterial in laserMaterials)
		{
			if (laserMaterial.type == type)
				return laserMaterial.startGlow;
		}
		Debug.LogError("Material not found for type: " + type);
		return null;
	}

	public Material GetParticlesMaterial(LaserType type)
	{
		foreach (var laserMaterial in laserMaterials)
		{
			if (laserMaterial.type == type)
				return laserMaterial.startParticles;
		}
		Debug.LogError("Material not found for type: " + type);
		return null;
	}

	// Método para obtener el color por tipo de láser
	public Color GetColor(LaserType type)
	{
		foreach (var laserMaterial in laserMaterials)
		{
			if (laserMaterial.type == type)
				return laserMaterial.color;
		}
		Debug.LogError("Color not found for type: " + type);
		return Color.black;
	}
}
