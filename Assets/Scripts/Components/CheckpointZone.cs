using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Backend.Core;

public class CheckpointZone : MonoBehaviour {
	

	/// <summary>
	/// The global unique identifier of the checkpoint
	/// </summary>
	public string GUID;

	[SerializeField]
	private bool wasTriggered = false;


	void OnTriggerStay(Collider other) {
		if(other.CompareTag("Player") && !wasTriggered) {
			wasTriggered = true;
			Debug.Log("Saving " + GUID);
			GameManager.SaveCheckpoint(new Checkpoint(GUID));
		}
	}

	/*
	void OnTriggerExit(Collider other) {
		wasTriggered = false;
	}
	*/
}
