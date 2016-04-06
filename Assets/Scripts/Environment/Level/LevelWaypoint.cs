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

	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(transform.position, (transform.forward * 5) + transform.position);
		Gizmos.DrawSphere((transform.forward * 5) + transform.position, 0.5F);
	}

	private void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Player")) {
			RaycastHit hit;
			Collider collider = GetComponent<Collider>();
			if(collider.Raycast(new Ray(other.transform.position, transform.position - other.transform.position), out hit, 25F)) {
				Vector3 normal = Vector3.Normalize(hit.normal);
				Vector3 forward = Vector3.Normalize(transform.forward);
				if((normal == forward && forwardPointsInside) 
				   || (normal == -forward && !forwardPointsInside)) { // Entered the collider entering the zone
					Debug.Log("Exited Zone");
					playerEntered = false;
					linkedWaypoints.ForEach((waypoint) => waypoint.playerEntered = false);
					if(Zone != EnumZone.OPEN_WORLD || PlayerController.CurrentZone != EnumZone.OPEN_WORLD) {
						PlayerController.CurrentZone = EnumZone.OPEN_WORLD;
						PlayerController.SFXEnterOpenWorld();
					}
				} else { // Entered the collider leaving the zone
					Debug.Log("Entered Zone");
					playerEntered = true;
					linkedWaypoints.ForEach((waypoint) => waypoint.playerEntered = true);
					if(Zone != EnumZone.OPEN_WORLD || PlayerController.CurrentZone != EnumZone.OPEN_WORLD) {
						PlayerController.CurrentZone = Zone;
						if(uiManager != null) {
							uiManager.EnterLevel(other.transform.parent.gameObject.name);
						}
					}
				}
			} else {
				Debug.LogWarning("Raycast didn't hit a collider when exiting the level waypoint collider");
			}
		}
	}
}
