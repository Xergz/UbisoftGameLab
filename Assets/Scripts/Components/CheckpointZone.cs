using UnityEngine;
using System.Collections;

public class CheckpointZone : MonoBehaviour {
    private const uint SEED = 0;
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

            checkpoint.GUID = Backend.Core.Murmur3.Hash (System.Text.Encoding.ASCII.GetBytes(GUID), SEED);
			checkpoint.SceneID = SceneID;

			checkpoint.Position = new Vector2 (this.transform.position.x, this.transform.position.z);
			checkpoint.Orientation = (System.UInt16)this.transform.eulerAngles.y;

            checkpoint.CurrentLife = (System.UInt32)game.PlayerController.GetPlayerCurrentLife ();

            // Save all the collected checkpoints
            System.Collections.Generic.List<Fragment> fragments = game.PlayerController.GetFragments ();
            foreach (Fragment frag in fragments) {
                checkpoint.Collectables.Add(Backend.Core.Murmur3.Hash(System.Text.Encoding.ASCII.GetBytes(frag.fragmentName), SEED), true);
            }


            game.SaveCheckpoint (checkpoint);
		}
	}
}
