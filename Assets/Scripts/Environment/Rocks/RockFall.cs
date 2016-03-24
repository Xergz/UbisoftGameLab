using UnityEngine;
using System.Collections;

public class RockFall : MonoBehaviour {

	public Transform rockTransform;
	public GameObject rockObject;
	public bool triggerFall { get; set; }
	private Vector3 position;
	private Vector3 decendre;
	// Use this for initialization
	void Start() {

		decendre = new Vector3(0f, 0.05f, 0f);
		position = new Vector3(0f, 0f, 0f);
		triggerFall = false;
		position = rockTransform.position;

	}

	// Update is called once per frame
	void Update() {
		if(triggerFall) {
			position = rockTransform.position;
			rockTransform.position = (position - decendre);
			if(position.y < -16) {
				Destroy(rockObject);
			}
		}
	}
	public void startFall() {
		triggerFall = true;
	}

}
