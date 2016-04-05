using UnityEngine;
using System.Collections;

public class LevelWaypoint : MonoBehaviour {
	public LevelWaypoint otherWaypoint;

	public bool playerEntered = false;

	public EnumZone Zone { get; set; }

	private UIManager uiManager;


	private void Awake() {
		if(otherWaypoint == null) {
			otherWaypoint = this;
		}

		uiManager = FindObjectOfType<UIManager>();
	}

	private void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Player")) {
			if(!playerEntered) {
				playerEntered = true;
				otherWaypoint.playerEntered = true;
				PlayerController.CurrentZone = Zone;
				if(uiManager != null) {
					uiManager.EnterLevel(other.transform.parent.gameObject.name);
				}
			}
		}
	}

	private void OnTriggerExit(Collider other) {
		if(other.CompareTag("Player")) {
			if(playerEntered) {
				playerEntered = false;
				otherWaypoint.playerEntered = false;
				PlayerController.CurrentZone = EnumZone.OPEN_WORLD;
				PlayerController.SFXEnterOpenWorld();
			}
		}
	}
}
