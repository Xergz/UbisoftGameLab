using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Store every data about a checkpoint
/// </summary>
public class Checkpoint {
	public System.UInt32 GUID;

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
	public System.UInt16 Orientation;

	/// <summary>
	/// The current life value of the player
	/// </summary>
	public System.UInt32 CurrentLife;

	/// <summary>
	/// Enumerates every collectable the player may obtains
	/// </summary>
	/// <remarks>Must contains every collectables, even the ones the player didn't find</remarks>
	public Dictionary<System.UInt32, bool> Collectables;

	public Checkpoint() {
		this.Zone = EnumZone.OPEN_WORLD;

		this.Position = new Vector2 (0, 0);
		this.Orientation = 0;

		this.Collectables = new Dictionary<System.UInt32, bool> ();
		this.CurrentLife = 0;
	}

	public Checkpoint(EnumZone zone, System.UInt32 life, Vector2 pos, System.UInt16 orientation) {
		this.Zone = zone;
		this.CurrentLife = life;

		this.Position = pos;
		this.Orientation = orientation;

		this.Collectables = new Dictionary<System.UInt32, bool> ();
	}

}
