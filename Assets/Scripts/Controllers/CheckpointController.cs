using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

/// <summary>
/// Handles checkpoints
/// </summary>
public class CheckpointController {
	private static CheckpointModel model;

	static CheckpointController() {
		model = new CheckpointModel();
	}

	public static uint Count {
		get {
			return model.Count;
		}
	}

	/// <summary>
	/// The save file's name
	/// </summary>
	/// <value>The save file.</value>
	public static string SaveFile { get; set; }

	/// <summary>
	/// Restore the game from a checkpoint
	/// </summary>
	/// <returns><c>true</c>, if the game was restored, <c>false</c> otherwise.</returns>
	/// <param name="checkpoint">The checkpoint to restore from.</param>
	public static bool RestoreFromLastCheckpoint() {
		try {
			GameManager.RestoreGameStateFrom(model.Current);

			return true;
		} catch(Exception e) {
			// Restoration failed
			// TODO: Determine what to do in case of failure
			Debug.LogError(e.Message);
		}

		return false;
	}

	/// <summary>
	/// Discards the last checkpoint.
	/// </summary>
	/// <remarks>The last checkpoint will be replaced by the one before it</remarks>
	public static void DiscardLastCheckpoint() {
		model.Discard();
		model.SaveTo(SaveFile);
	}

	public static bool SaveCheckpoint(Checkpoint newCheckpoint) {
		if(!model.ContainsGUID(newCheckpoint.GUID)) {
			model.Update(newCheckpoint);
			model.SaveTo(SaveFile);

			return true;
		}

		return false;
	}

	public static void LoadCheckpointsFromSaveFile(bool restoreFromLastCheckpoint) {
		LoadCheckpointsFrom(SaveFile, restoreFromLastCheckpoint);
	}

	public static void LoadCheckpointsFrom(string filename, bool restoreFromLastCheckpoint) {
		model.LoadFrom(filename);
		if(restoreFromLastCheckpoint) GameManager.RestoreGameStateFrom(model.Current);
	}

	public static void Clear() {
		model.Clear(SaveFile);
	}
}
