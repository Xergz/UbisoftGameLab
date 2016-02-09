using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Fragment")) {
			other.gameObject.SetActive(false);
			PlayerController.AddFragment(other.GetComponent<Fragment>());
		}
	}
}
