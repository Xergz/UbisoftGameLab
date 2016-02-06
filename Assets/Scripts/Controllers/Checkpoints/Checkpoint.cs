using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Store every data about a checkpoint
/// </summary>
public struct Checkpoint {

	/// <summary>
	/// The scene where is located the checkpoint
	/// </summary>
	public System.UInt32 SceneID;

	/// <summary>
	/// The player's life
	/// </summary>
	public System.UInt32 CurrentLife;

	/// <summary>
	/// Position from the top where the player should start
	/// </summary>
	public Vector2 Position;

	/// <summary>
	/// The orientation from the top from which the player should start
	/// </summary>
	public System.UInt16 Orientation;

	/// <summary>
	/// Enumerates every collectable the player may obtains
	/// </summary>
	/// <remarks>Must contains every collectables, even the ones the player didn't find</remarks>
	public Dictionary<System.UInt32, bool> Collectables;
}