using UnityEngine;
using System.Collections;

public class Player : Entity {
	public PlayerController PlayerController { get; set; } // The PlayerController linked to this player

    public override void ReceiveHit() {
        PlayerController.DamagePlayer(1);
    }

    public override void ReceiveStun() {
        //Desactivate Player movement
        Debug.Log("Player stuned!");
    }

    private void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Fragment")) { // Picked up a fragment
			other.gameObject.SetActive(false);
			PlayerController.AddFragment(other.GetComponent<Fragment>());
		} else if(other.CompareTag("Zone")) { // Entered a zone
			PlayerController.CurrentZone = other.GetComponent<Zone>().ZoneIndex;
		}
	}

    protected override void OnTriggerStay(Collider other) {
        if (other.CompareTag("Stream")) { // Is inside a stream
            PlayerController.AddForce(other.GetComponent<Stream>().GetForceAtPosition(transform.position));
        }
    }

	private void OnTriggerExit(Collider other) {
		if(other.CompareTag("Zone")) { // Left the zone to enter the open world
			PlayerController.CurrentZone = EnumZone.OPEN_WORLD;
		}
	}
}
