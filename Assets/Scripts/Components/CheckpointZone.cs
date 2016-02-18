using UnityEngine;
using System.Collections;

public class CheckpointZone : MonoBehaviour {
	public GameManager game;

	/// <summary>
	/// The current scene ID
	/// </summary>
	public System.UInt32 SceneID;

	/// <summary>
	/// The global unique identifier of the checkpoint
	/// </summary>
	public string GUID;


	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject == game.Player) {
			Checkpoint checkpoint = new Checkpoint ();

			checkpoint.GUID = Backend.Core.Murmur3.Hash (System.Text.Encoding.ASCII.GetBytes(GUID), 0);
			checkpoint.SceneID = SceneID;

			checkpoint.Position = new Vector2 (this.transform.position.x, this.transform.position.z);
			checkpoint.Orientation = (System.UInt16)this.transform.eulerAngles.y;

			// Save the checkpoint or do nothing if already saved
			game.Checkpoints.SaveCheckpoint (checkpoint);
		}
	}
}
