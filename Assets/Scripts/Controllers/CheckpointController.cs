using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

/// <summary>
/// Handles checkpoints
/// </summary>
public class CheckpointController {
	private GameRestorer restorer;
	private CheckpointModel model;

	/// <summary>
	/// Initializes a new instance of the <see cref="CheckpointController"/> class.
	/// </summary>
	/// <param name="restorer">The assigned game restorer.</param>
	public CheckpointController(GameRestorer restorer) {
		this.restorer = restorer;
		this.model = new CheckpointModel ();
	}

	/// <summary>
	/// Restore the game from a checkpoint
	/// </summary>
	/// <returns><c>true</c>, if the game was restored, <c>false</c> otherwise.</returns>
	/// <param name="checkpoint">The checkpoint to restore from.</param>
	public bool RestoreFromLastCheckpoint() {
		bool restored = false;

		try {
			Checkpoint checkpoint = this.model.Current;

			this.restorer.RestoreGameStateFrom(checkpoint);

			restored = true;
		}
		catch(Exception e) {
			// Restoration failed
			// TODO: Determine what to do in case of failure
			Debug.LogError(e.Message);
		}

		return restored;
	}

	/// <summary>
	/// Discards the last checkpoint.
	/// </summary>
	/// <remarks>The last checkpoint will be replaced by the one before it</remarks>
	public void DiscardLastCheckpoint() {
		this.model.Discard ();
	}

	public void SaveCheckpoint(Checkpoint newCheckpoint) {
		this.model.Update (newCheckpoint);

		this.model.SaveTo ("test-checkpoints-1");
	}

	public void LoadCheckpointsFrom(string filename) {
		this.model.LoadFrom (filename);
		this.restorer.RestoreGameStateFrom (this.model.Current);
	}
}
