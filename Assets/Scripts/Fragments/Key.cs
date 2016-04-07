using System.Collections;
using UnityEngine;

public class Key : MonoBehaviour {
	public RockFall rockTrigger;


	[SerializeField]
	private float moveCameraForXSeconds = 2F;

	private CameraController cameraController;


	private void Awake() {
		cameraController = FindObjectOfType<CameraController>();
	}

	private void Update() {
		transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);
	}

	private void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Player")) {
			rockTrigger.startFall();
			GetComponent<MeshRenderer>().enabled = false;
			StartCoroutine(MoveCamera());
		}
	}

	private IEnumerator MoveCamera() {
		yield return new WaitForSeconds(0.25F);

		cameraController.canMove = false;
		cameraController.SetCameraPosition(rockTrigger.transform.position);

		yield return new WaitForSeconds(moveCameraForXSeconds);

		cameraController.SetCameraPosition(PlayerController.Player.transform.position);
		cameraController.canMove = true;
		gameObject.SetActive(false);
		GetComponent<MeshRenderer>().enabled = true;
	}
}
