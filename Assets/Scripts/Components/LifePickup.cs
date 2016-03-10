using UnityEngine;
using System.Collections;

public class LifePickup : MonoBehaviour {
    [Tooltip("Health point value given by this pickup")]
    public int Value = 1;

    [Tooltip("The player controller assigned to this pickup. If not set, the pickup will search for the player controller on start")]
    public PlayerController Controller;

	// Use this for initialization
	void Start () {
        // If the controller is not manually selected, the pickup will try to find it
        if (Controller == null) {
            Controller = GameObject.Find ("PlayerController").GetComponent<PlayerController> ();
        }
	}
	
    void OnTriggerEnter(Collider other) {
        // If the player as picked up the health capsule
        if(other.attachedRigidbody == Controller.playerRigidbody) {
            Controller.AddLife (Value);
            this.gameObject.SetActive(false);
        }
    }
}
