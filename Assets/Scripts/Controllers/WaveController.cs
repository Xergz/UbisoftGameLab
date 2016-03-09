using UnityEngine;
using System.Collections;
using SimplexNoise;

/// <summary>
/// Manage the wave simulator.
/// </summary>
public class WaveController : MonoBehaviour {

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

	[Tooltip("Specify the wave's max/min height")]
	public float waveIntensity = 0.33f;

	[Tooltip("Specify the scale to use when accessing the noise")]
	public float noiseScale = 0.1f;

	[Tooltip("Specify the speed of waves")]
	public float waveSpeed = 0.33f;

	/// <summary>
	/// Gets the ocean height at a specific location.
	/// </summary>
	/// <returns>The height of the ocean at this location</returns>
	/// <param name="x">The X coordinate of the location</param>
	/// <param name="z">The Z coordinate of the location</param>
	public float GetOceanHeightAt(float x, float z) {
		return GetOceanHeightAt(x, z, Time.time);
	}

	/// <summary>
	/// Gets the ocean height at a specific location and a specific time point.
	/// </summary>
	/// <returns>The height of the ocean at this location</returns>
	/// <param name="x">The X coordinate of the location</param>
	/// <param name="z">The Z coordinate of the location</param>
	/// <param name="time">The time of the sampling</param>
	public float GetOceanHeightAt(float x, float z, float time) {
		float height = 0.0f;

		height = Noise.Generate(x * noiseScale, z * noiseScale, time * waveSpeed);
		height *= waveIntensity;

		return height;
	}

	/// <summary>
	/// Calculates the surface normal at one point
	/// </summary>
	/// <returns>The surface's normal.</returns>
	/// <param name="x">The X coordinate.</param>
	/// <param name="z">The Z coordinate.</param>
	public Vector3 GetSurfaceNormalAt(float x, float z, float surfaceWidth = NORMAL_PRECISION, float surfaceLength = NORMAL_PRECISION) {

		// 1. Récupérer deux points proches sur l'axe des x
		float xHeight1 = GetOceanHeightAt(x - surfaceWidth, z);
		float xHeight2 = GetOceanHeightAt(x + surfaceWidth, z);

		// 2. Récupérer deux points proches sur l'axe des z
		float zHeight1 = GetOceanHeightAt(x, z - surfaceLength);
		float zHeight2 = GetOceanHeightAt(x, z + surfaceLength);

		// On calcule la normale délimitée par le plan formé par les 4 points
		return Vector3.Cross(
			new Vector3(2 * surfaceWidth, xHeight2 - xHeight1, 0),
			new Vector3(0, zHeight1 - zHeight2, -2 * surfaceLength)
		);
	}

	/// <summary>
	/// Gets the height displacement at a specific surface point from a specific time point since elapsed time.
	/// </summary>
	/// <returns>The height displacement between the two time frame</returns>
	/// <param name="x">The x coordinate of the surface</param>
	/// <param name="z">The z coordinate of the surface</param>
	/// <param name="elaspedTime">Elapsed time.</param>
	public float GetHeightDisplacementAt(float x, float z, float elaspedTime) {
		float time = Time.time;

		float currentHeight = GetOceanHeightAt(x, z, time);
		float lastHeight = GetOceanHeightAt(x, z, time - elaspedTime);

		return currentHeight - lastHeight;
	}

	public SurfaceInfo GetSurfaceAt(float x, float z, float width = NORMAL_PRECISION, float length = NORMAL_PRECISION) {
		return new SurfaceInfo(GetSurfaceNormalAt(x, z, width, length), GetOceanHeightAt(x, z));
	}
}
