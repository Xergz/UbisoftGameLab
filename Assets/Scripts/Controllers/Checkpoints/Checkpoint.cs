using UnityEngine;
using System.Collections.Generic;
using Backend.Core;
using System.Text;

/// <summary>
/// Store every data about a checkpoint
/// </summary>
public class Checkpoint {
	public uint GUID;

	/// <summary>
	/// The scene where is located the checkpoint
	/// </summary>
	public EnumZone Zone;

	/// <summary>
	/// Position from the top where the player should start
	/// </summary>
	public Vector2 Position;

	/// <summary>
	/// The orientation from the top from which the player should start
	/// </summary>
	public ushort Orientation;

	/// <summary>
	/// The current life value of the player
	/// </summary>
	public uint CurrentLife;

	/// <summary>
	/// Enumerates every collectable the player may obtains
	/// </summary>
	/// <remarks>Must contains every collectables, even the ones the player didn't find</remarks>
	public Dictionary<uint, bool> Collectables;

	private const uint SEED = 0;

	public Checkpoint() {
		Zone = EnumZone.OPEN_WORLD;
		CurrentLife = 0;

		Position = Vector3.zero;
		Orientation = 0;

		Collectables = new Dictionary<uint, bool>();
	}

	public Checkpoint(string sGUID) {
		GUID = Murmur3.Hash(Encoding.ASCII.GetBytes(sGUID), SEED);
		Zone = PlayerController.CurrentZone;

		Transform playerTransform = PlayerController.Player.transform;

		Position = new Vector2(playerTransform.position.x, playerTransform.position.z);
		Orientation = (ushort) playerTransform.eulerAngles.y;

		CurrentLife = (ushort) PlayerController.GetPlayerCurrentLife();

		// Save all the collected fragments
		List<Fragment> fragments = PlayerController.GetCollectedFragments();
		Collectables = new Dictionary<uint, bool>();
		foreach(Fragment frag in fragments) {
			Collectables.Add(Murmur3.Hash(Encoding.ASCII.GetBytes(frag.fragmentName), SEED), true);
		}
	}
}
