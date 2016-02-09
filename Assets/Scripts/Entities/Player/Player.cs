using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public PlayerController PlayerController { get; set; } // The PlayerController linked to this player


	private void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.CompareTag("Rock")) {
			PlayerController.DamagePlayer(1);
		}
	}

	private void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Fragment")) {
			other.gameObject.SetActive(false);
			PlayerController.AddFragment(other.GetComponent<Fragment>());
		} else if(other.gameObject.CompareTag("Stream")) {
			PlayerController.AddForce(other.gameObject.GetComponent<Stream>().GetForceAtPosition(transform.position));
		}
	}
}
