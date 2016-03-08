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

    public System.UInt32 Count {
        get {
            return model.Count;
        }
    }

	/// <summary>
	/// The save file's name
	/// </summary>
	/// <value>The save file.</value>
	public string SaveFile {
		get;
		set;
	}

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
        this.model.SaveTo (SaveFile);
	}

	public bool SaveCheckpoint(Checkpoint newCheckpoint) {
		bool added = false;

		if (!this.model.ContainsGUID (newCheckpoint.GUID)) {
			this.model.Update (newCheckpoint);

			this.model.SaveTo (SaveFile);

			added = true;
		}

		return added;
	}

	public void LoadCheckpointsFromSaveFile() {
		LoadCheckpointsFrom (SaveFile);
	}

	public void LoadCheckpointsFrom(string filename) {
		this.model.LoadFrom (filename);
		this.restorer.RestoreGameStateFrom (this.model.Current);
	}

    public void Clear() {
        this.model.Clear (this.SaveFile);
    }
}
