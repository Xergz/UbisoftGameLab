using UnityEngine;
using System.Collections;
using System;
using System.Text;
using Backend.Core;

public class GameManager : MonoBehaviour {

	private static CameraController cameraController;

	/// <summary>
	/// Restores the game state from a checkpoint.
	/// </summary>
	/// <param name="checkpoint">The checkpoint to restore the game's state from</param>
	/// <remarks>Should throw an exception on failure</remarks>
	public static void RestoreGameStateFrom(Checkpoint checkpoint) {
		// Iterate over every fragment gameobject
		foreach(Transform fragmentTransform in PlayerController.fragmentsList) {
			GameObject fragmentObject = fragmentTransform.gameObject;
			Fragment fragment = fragmentObject.GetComponent<Fragment>();

			// Look if the player already picked it
			uint hashName = Murmur3.Hash(Encoding.ASCII.GetBytes(fragment.fragmentName), 0);
			if(checkpoint.Collectables.ContainsKey(hashName)) {
				bool obtained = checkpoint.Collectables[hashName];
				fragmentObject.SetActive(!obtained);
				if(obtained) {
					PlayerController.AddFragment(fragment);
				}
			} else {
				fragmentObject.SetActive(true);
			}
		}

		if(PlayerController.Player != null) {
			PlayerController.Player.transform.position = new Vector3(checkpoint.Position.x, 0, checkpoint.Position.y);
			PlayerController.Player.transform.Rotate(0, checkpoint.Orientation, 0);
			PlayerController.SetPlayerCurrentLife((int) checkpoint.CurrentLife);

			PlayerController.CurrentZone = checkpoint.Zone;

			if(cameraController == null) { // Try to find it
				cameraController = FindObjectOfType<CameraController>();
			}

			if(cameraController != null) { // It exists
				cameraController.SetCameraPosition(PlayerController.Player.transform.position);
			}
		}
	}

	/// <summary>
	/// Restores the game state from the last saved checkpoint.
	/// </summary>
	/// <returns><c>true</c>, if the restoration is possible, <c>false</c> otherwise.</returns>
	public static bool RestoreFromLastCheckpoint() {
		return CheckpointController.RestoreFromLastCheckpoint();
	}

	/// <summary>
	/// Discards the last saved checkpoint.
	/// </summary>
	public static void DiscardLastCheckpoint() {
		CheckpointController.DiscardLastCheckpoint();
	}

	/// <summary>
	/// Counts the number of saved checkpoints.
	/// </summary>
	/// <returns>The saved checkpoints.</returns>
	public static uint CountSavedCheckpoints() {
		return CheckpointController.Count;
	}

	/// <summary>
	/// Remove all saved checkpoints
	/// </summary>
	public static void DeleteAllCheckPoints() {
		CheckpointController.Clear();
	}

	/// <summary>
	/// Save a checkpoint
	/// </summary>
	/// <param name="checkpoint">The checkpoint to save</param>
	public static void SaveCheckpoint(Checkpoint checkpoint) {
		CheckpointController.SaveCheckpoint(checkpoint);
	}

	/// <summary>
	/// Loads the checkpoints from a file.
	/// </summary>
	/// <returns><c>true</c>, if checkpoint file was loaded, <c>false</c> otherwise.</returns>
	public static bool LoadCheckpointFile(bool restoreFromLastCheckpoint) {
		CheckpointController.SaveFile = "SavedGame";

		try {
			CheckpointController.LoadCheckpointsFromSaveFile(restoreFromLastCheckpoint);
			return true;
		} catch(Exception e) {
			Debug.Log(e.Message);
		}
		return false;
	}
}
