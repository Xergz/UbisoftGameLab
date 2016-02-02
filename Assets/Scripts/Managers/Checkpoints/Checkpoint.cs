using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Store every data about a checkpoint
/// </summary>
public struct Checkpoint {

	/// <summary>
	/// The scene where is located the checkpoint
	/// </summary>
	int 	SceneID;

	/// <summary>
	/// The player's life
	/// </summary>
	int 	CurrentLife;

	/// <summary>
	/// Position from the top where the player should start
	/// </summary>
	Vector2 Position;

	/// <summary>
	/// The orientation from the top from which the player should start
	/// </summary>
	float 	Orientation;

	/// <summary>
	/// Enumerates every collectable the player may obtains
	/// </summary>
	/// <remarks>Must contains every collectables, even the ones the player didn't find</remarks>
	Dictionary<int, bool> Collectables;
}