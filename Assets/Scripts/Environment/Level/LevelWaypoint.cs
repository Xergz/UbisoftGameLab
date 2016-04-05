using UnityEngine;
using System.Collections.Generic;

public class LevelWaypoint : MonoBehaviour {

	public EnumZone Zone { get; set; }


	[Tooltip("True if the forward of the gameobject points in the direction the player must take to enter the level")]
	public bool forwardPointsInside = true;
	public bool playerEntered = false;

	public List<LevelWaypoint> linkedWaypoints;


	private UIManager uiManager;


	private void Awake() {
		uiManager = FindObjectOfType<UIManager>();
	}

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(transform.position, (transform.forward * 5) + transform.position);
		Gizmos.DrawSphere((transform.forward * 5) + transform.position, 0.5F);
	}

	private void OnTriggerExit(Collider other) {
		if(other.CompareTag("Player")) {
			RaycastHit hit;
			if(other.Raycast(new Ray(transform.position, other.transform.position - transform.position), out hit, 25F)) { // Wrong colliders
				Vector3 normal = Vector3.Normalize(hit.normal);
				Vector3 forward = Vector3.Normalize(hit.transform.forward);
				if((normal == forward && forwardPointsInside) 
				   || (normal == -forward && !forwardPointsInside)) { // Exited the collider inside the zone
					Debug.Log("Entered Zone");
					playerEntered = true;
					linkedWaypoints.ForEach((waypoint) => waypoint.playerEntered = true);
					PlayerController.CurrentZone = Zone;
					if(uiManager != null) {
						uiManager.EnterLevel(other.transform.parent.gameObject.name);
					}
				} else { // Exited the collider outside the zone
					Debug.Log("Exited Zone");
					playerEntered = false;
					linkedWaypoints.ForEach((waypoint) => waypoint.playerEntered = false);
					PlayerController.CurrentZone = EnumZone.OPEN_WORLD;
					PlayerController.SFXEnterOpenWorld();
				}
			} else {
				Debug.LogWarning("Raycast didn't hit a collider when exiting the level waypoint collider");
			}
		}
	}
}
