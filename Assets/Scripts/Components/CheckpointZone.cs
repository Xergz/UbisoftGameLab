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


	void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Player")) {
			GameManager.SaveCheckpoint(new Checkpoint(GUID));
		}
	}
}
