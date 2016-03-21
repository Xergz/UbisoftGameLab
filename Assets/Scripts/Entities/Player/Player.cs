using UnityEngine;
using System.Collections;

public class Player : Entity {
	public PlayerController PlayerController { get; set; } // The PlayerController linked to this player

    public GameObject stunStars;

	[SerializeField]
	private bool isStuned;

	[SerializeField]
	private float stunTime = 2f;
	private float beginStunTime;


	[SerializeField]
	private ParticleSystem collisionSystem;


	private void Update() {
		if(isStuned && Time.time > beginStunTime + stunTime) {
			isStuned = false;
            stunStars.SetActive(false);
            PlayerController.PlayerCanBeMoved = true;
		}
	}

	public override void ReceiveHit() {
        audioController.PlayAudio(AudioController.soundType.receiveHit);
		PlayerController.DamagePlayer(5);
	}

	public override void ReceiveStun() {
        audioController.PlayAudio(AudioController.soundType.receiveStun);
        PlayerController.PlayerCanBeMoved = false;
		isStuned = true;
        stunStars.SetActive(true);
		beginStunTime = Time.time;
	}

	private void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.CompareTag("Environment")) {
			collisionSystem.Emit(Random.Range(30, 50));
		}
	}

	private void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Fragment")) { // Picked up a fragment
			other.gameObject.SetActive(false);
			PlayerController.AddFragment(other.GetComponent<Fragment>());
			Debug.Log("Congratulations! You gained the \"" + other.GetComponent<Fragment>().fragmentName + "\" memory fragment");
		} else if(other.CompareTag("Zone")) { // Entered a zone
			PlayerController.CurrentZone = other.GetComponent<Zone>().ZoneIndex;
		} else if(other.CompareTag("Life")) {
			PlayerController.AddLife(other.GetComponent<LifePickup>().Value);
			other.gameObject.SetActive(false);
		}
	}

	protected override void OnTriggerStay(Collider other) {
		if(other.CompareTag("Stream")) { // Is inside a stream
			Vector3 force = other.GetComponent<Stream>().GetForceAtPosition(transform.position);
			PlayerController.AddForce(force, other.GetComponent<Stream>());
		}
	}

	private void OnTriggerExit(Collider other) {
		if(other.CompareTag("Zone")) { // Left the zone to enter the open world
            if (Random.Range(0.0f, 1.0f) > 0.5f)
                audioController.PlayAudio(AudioController.soundType.enterOpenWorld);

            PlayerController.CurrentZone = EnumZone.OPEN_WORLD;
		}
	}
}
