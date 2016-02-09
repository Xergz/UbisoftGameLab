using UnityEngine;
using System.Collections;

public class FloatOnWave : MonoBehaviour {
	[Tooltip("Specify the wave's manager")]
	public WaveController waves;

	[Tooltip("Specify the floating line of the object")]
	[Range(-1.0f, 1.0f)]
	public float FloatingOffset = 0.0f;

	[Tooltip("Specify the surface's length")]
	public float SurfaceLength = 1.0f;

	[Tooltip("Specify the surface's width")]
	public float SurfaceWidth = 1.0f;

	private void Awake() {
		if(waves == null) {
			Debug.LogError("No WaveController is attached to the floating object");
		}
	}

	// Update is called once per frame
	void Update() {
		Vector3 pos = this.transform.position;
		WaveController.SurfaceInfo infos = this.waves.GetSurfaceAt(pos.x, pos.z, SurfaceWidth, SurfaceLength);

		pos.y = infos.Height + FloatingOffset;

		this.transform.position = pos;
		this.transform.up = infos.Normal;
	}
}
