using UnityEngine;
using System.Collections;

public class FloatOnWave : MonoBehaviour {
	[Tooltip("Specify the wave's manager")]
	public WaveManager waves;

	// Update is called once per frame
	void Update () {

		Vector3 pos = this.transform.position;
		pos.y = waves.getOceanHeightAt (pos.x, pos.z);

		this.transform.position = pos;
	}
}
