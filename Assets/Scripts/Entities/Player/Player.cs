using UnityEngine;
using System.Collections;

public class Player : Entity {
	public PlayerController PlayerController { get; set; } // The PlayerController linked to this player

    public override void ReceiveHit() {
        PlayerController.DamagePlayer(1);
    }

    private void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Fragment")) {
			other.gameObject.SetActive(false);
			PlayerController.AddFragment(other.GetComponent<Fragment>());
		} else if(other.CompareTag("Zone")) {
			PlayerController.CurrentZone = other.GetComponent<Zone>().ZoneIndex;
		}
	}

    protected override void OnTriggerStay(Collider other) {
        if (other.CompareTag("Stream")) {
            PlayerController.AddForce(other.GetComponent<Stream>().GetForceAtPosition(transform.position));
        }
    }
}
