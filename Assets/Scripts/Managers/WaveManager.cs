using UnityEngine;
using System.Collections;
using SimplexNoise;

/// <summary>
/// Manage the wave simulator.
/// </summary>
public class WaveManager : MonoBehaviour {
	private const float NORMAL_PRECISION = 1.0f;

	[Tooltip("Specify the position in the world")]
	public Transform worldTransform;

	[Tooltip("Specify the surface to deform")]
	public MeshFilter surfaceMeshFilter;

	[Tooltip("Specify the wave's max/min height")]
	public float waveIntensity = 0.33f;

	[Tooltip("Specify the scale to use when accessing the noise")]
	public float noiseScale = 0.1f;

	[Tooltip("Specify the speed of waves")]
	public float waveSpeed = 0.33f;

	private Mesh surfaceMesh;

	/// <summary>
	/// Gets the ocean height at a specific location.
	/// </summary>
	/// <returns>The <see cref="System.Single"/>.</returns>
	/// <param name="position">The position in the ocean from the top view.</param>
	public float getOceanHeightAt(float x, float y) {
		float height = 0.0f;

		height = Noise.Generate (x * noiseScale, y * noiseScale, Time.realtimeSinceStartup * waveSpeed);
		height *= waveIntensity;

		return height;
	}

	/// <summary>
	/// Calculates the surface normal at one point
	/// </summary>
	/// <returns>The surface's normal.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public Vector3 GetSurfaceNormalAt(float x, float y) {
		Vector3 normal = Vector3.up;


		// 1. Récupérer deux points proches sur l'axe des x
		//float xHeight1, xHeight2;
		float xHeight1 = getOceanHeightAt(x - NORMAL_PRECISION, y);
		float xHeight2 = getOceanHeightAt (x + NORMAL_PRECISION, y);
		Vector3 xHeight = new Vector3 (x + NORMAL_PRECISION, xHeight2, 0) - new Vector3(x - NORMAL_PRECISION, xHeight1, 0);

		// 2. Récupérer deux points proches sur l'axe des y
		float zHeight1 = getOceanHeightAt(x, y - NORMAL_PRECISION);
		float zHeight2 = getOceanHeightAt (x, y + NORMAL_PRECISION);
		Vector3 zHeight = new Vector3 (0, zHeight1, y - NORMAL_PRECISION) - new Vector3 (0, zHeight2, y + NORMAL_PRECISION);

		//normal = (xHeight + zHeight).normalized;

		normal = Vector3.Cross (xHeight, zHeight);

		return normal;
	}

	//
	// UNITY CALLBACKS
	//

	void Start() {
		surfaceMesh = surfaceMeshFilter.mesh;
	}

	void Update() {
		Vector3[] vertices = surfaceMesh.vertices;
		Vector3 worldPos;

		for (int i = 0; i < vertices.Length; i++) {
			worldPos = worldTransform.TransformPoint (vertices [i]);

			vertices[i].y = getOceanHeightAt (worldPos.x, worldPos.z);
		}

		surfaceMesh.vertices = vertices;
	}
}
