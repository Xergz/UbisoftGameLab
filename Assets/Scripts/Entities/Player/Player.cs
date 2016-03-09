using UnityEngine;
using System.Collections;

public class Player : Entity {
	public PlayerController PlayerController { get; set; } // The PlayerController linked to this player

    public override void ReceiveHit() {
        PlayerController.DamagePlayer(1, 10);
    }

    // This function will probably move partially to the parent class. Probably as a template method so that all entities are affected by streams.
    // Will wait until one shade is done though since I am still not sure of the right implementation.
    private void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Fragment")) {
			other.gameObject.SetActive(false);
			PlayerController.AddFragment(other.GetComponent<Fragment>());
		}
	}

    protected override void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Stream")) {
            PlayerController.AddForce(other.GetComponent<Stream>().GetForceAtPosition(transform.position));
        }
    }
}
