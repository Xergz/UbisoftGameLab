using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour, GameRestorer {
	private CheckpointController checkpoints;

	/// <summary>
	/// The gameobject representing the player
	/// </summary>
	public GameObject Player = null;

	/// <summary>
	/// Gets the checkpoints controller.
	/// </summary>
	/// <value>The checkpoints controller.</value>
	public CheckpointController Checkpoints {
		get {
			return checkpoints;
		}
	}
		
	/// <summary>
	/// Restores the game state from a checkpoint.
	/// </summary>
	/// <param name="checkpoint">Checkpoint.</param>
	public void RestoreGameStateFrom(Checkpoint checkpoint) {
		if (Player != null) {
			// TODO: Restore player's state

            Player.transform.position = new Vector3 (checkpoint.Position.x, 0, checkpoint.Position.y);
            Player.transform.Rotate (0, (float)checkpoint.Orientation, 0);
		}
	}

	//
	// UNITY CALLBACKS
	//

	// Use this for initialization
	void Start () {
		this.checkpoints = new CheckpointController (this);
        this.checkpoints.SaveFile = "patate";

        try {
            this.checkpoints.LoadCheckpointsFromSaveFile ();
        }
        catch(Exception e) {
            Debug.LogError (e.Message);
        }
	}

	// Update is called once per frame
	void Update () {

	}
}
