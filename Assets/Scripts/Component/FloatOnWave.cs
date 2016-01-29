using UnityEngine;
using System.Collections;

public class FloatOnWave : MonoBehaviour {
	[Tooltip("Specify the wave's manager")]
	public WaveManager waves;

	[Tooltip("Specify the floating line of the object")]
	[Range(-1.0f, 1.0f)]
	public float FloatingOffset = 0.0f;

	// Update is called once per frame
	void Update () {

		Vector3 pos = this.transform.position;
		pos.y = waves.getOceanHeightAt (pos.x, pos.z) + FloatingOffset;

		this.transform.position = pos;
		this.transform.up = waves.GetSurfaceNormalAt (pos.x, pos.z);
	}
}
