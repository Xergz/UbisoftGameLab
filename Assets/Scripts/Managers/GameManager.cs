using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour, GameRestorer {
	private CheckpointController checkpoints;
    public PlayerController PlayerController = null;

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
            Player.transform.position = new Vector3 (checkpoint.Position.x, 0, checkpoint.Position.y);
            Player.transform.Rotate (0, (float)checkpoint.Orientation, 0);
		}
	}

    /// <summary>
    /// Restores the game state from the last saved checkpoint.
    /// </summary>
    /// <returns><c>true</c>, if the restoration is possible, <c>false</c> otherwise.</returns>
    public bool RestoreFromLastCheckpoint() {
        return this.checkpoints.RestoreFromLastCheckpoint ();
    }

    /// <summary>
    /// Discards the last saved checkpoint.
    /// </summary>
    public void DiscardLastCheckpoint() {
        this.checkpoints.DiscardLastCheckpoint ();
    }

    public System.UInt32 CountSavedCheckpoints() {
        return this.checkpoints.Count;
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
            Debug.Log (e.Message);
        }
	}

	// Update is called once per frame
	void Update () {

	}
}
