using UnityEngine;
using System.Collections;
using SimplexNoise;

/// <summary>
/// Manage the wave simulator.
/// </summary>
public class WaveManager : MonoBehaviour {

	/// <summary>
	/// The ocean's surface informations at a specific location
	/// </summary>
	public struct SurfaceInfo {
		public Vector3 Normal;
		public float Height;

		public SurfaceInfo(Vector3 normal, float height) {
			this.Normal = normal;
			this.Height = height;
		}
	};

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
	public Vector3 GetSurfaceNormalAt(float x, float y, float surfaceWidth = NORMAL_PRECISION, float surfaceLength = NORMAL_PRECISION) {
		
		// 1. Récupérer deux points proches sur l'axe des x
		float xHeight1 = getOceanHeightAt(x - surfaceWidth, y);
		float xHeight2 = getOceanHeightAt (x + surfaceWidth, y);

		// 2. Récupérer deux points proches sur l'axe des y
		float zHeight1 = getOceanHeightAt(x, y - surfaceLength);
		float zHeight2 = getOceanHeightAt (x, y + surfaceLength);

		// On calcule la normale délimitée par le plan formé par les 4 points
		return Vector3.Cross(
			new Vector3(2 * surfaceWidth, xHeight2 - xHeight1, 0),
			new Vector3 (0, zHeight1 - zHeight2, -2 * surfaceLength)
		);
	}

	public SurfaceInfo GetSurfaceAt(float x, float y, float width = NORMAL_PRECISION, float length = NORMAL_PRECISION) {
		return new SurfaceInfo (GetSurfaceNormalAt(x, y, width, length), getOceanHeightAt(x, y));
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
