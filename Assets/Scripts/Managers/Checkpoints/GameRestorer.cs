using UnityEngine;
using System.Collections;

/// <summary>
/// Define the interface of a class responsible to restore the game state from a checkpoint
/// </summary>
public interface GameRestorer {
	
	/// <summary>
	/// Restores the game state from a checkpoint.
	/// </summary>
	/// <param name="checkpoint">The checkpoint to restore the game's state from</param>
	/// <remarks>Should throw an exception on failure</remarks>
	void RestoreGameStateFrom(Checkpoint checkpoint);
}
