using UnityEngine;
using System.Collections;
using SimplexNoise;

/// <summary>
/// Manage the wave simulator.
/// </summary>
public class WaveManager : MonoBehaviour {
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
