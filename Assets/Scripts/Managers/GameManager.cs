using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour, GameRestorer {
    private CheckpointController checkpoints;

    /// <summary>
    /// The gameobject parent to all fragments in the scene
    /// </summary>
    public GameObject FragmentsRoot;

    /// <summary>
    /// The player controller
    /// </summary>
    public PlayerController PlayerController = null;


    /// <summary>
    /// Returns the player's game object
    /// </summary>
    /// <value>The player.</value>
    public GameObject Player {
        get {
            return PlayerController.Player;
        }
    }

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
         
        // Iterate over every fragment gameobject
        foreach (Transform fragmentTransform in FragmentsRoot.transform) {
            GameObject fragmentObject = fragmentTransform.gameObject;
            Fragment fragment = fragmentObject.GetComponent<Fragment> ();

            // Look if the player already picked it
            System.UInt32 hashName = Backend.Core.Murmur3.Hash (System.Text.Encoding.ASCII.GetBytes (fragment.fragmentName), 0);
            if (checkpoint.Collectables.ContainsKey (hashName)) {
                fragmentObject.SetActive (!checkpoint.Collectables[hashName]);
            } else {
                fragmentObject.SetActive (true);
            }
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

    /// <summary>
    /// Counts the number of saved checkpoints.
    /// </summary>
    /// <returns>The saved checkpoints.</returns>
    public System.UInt32 CountSavedCheckpoints() {
        return this.checkpoints.Count;
    }

    /// <summary>
    /// Remove all saved checkpoints
    /// </summary>
    public void DeleteAllCheckPoints() { 
        this.checkpoints.Clear ();
    }

    /// <summary>
    /// Save a checkpoint
    /// </summary>
    /// <param name="checkpoint">The checkpoint to save</param>
    public void SaveCheckpoint(Checkpoint checkpoint) {
        this.checkpoints.SaveCheckpoint (checkpoint);
    }

    /// <summary>
    /// Loads the checkpoints from a file.
    /// </summary>
    /// <returns><c>true</c>, if checkpoint file was loaded, <c>false</c> otherwise.</returns>
	public bool LoadCheckpointFile() {
		this.checkpoints = new CheckpointController (this);
        this.checkpoints.SaveFile = "SavedGame";

        try {
            this.checkpoints.LoadCheckpointsFromSaveFile ();
            return true;
        }
        catch(Exception e) {
            Debug.Log (e.Message);
        }
        return false;
	}

    /// <summary>
    /// The game manager will search for all it's required dependencies on the scene
    /// </summary>
    public void Initialize() {
        this.FragmentsRoot = GameObject.Find ("Environment/Fragments");
        this.PlayerController = GameObject.Find ("PlayerController").GetComponent<PlayerController>();
    }
}
