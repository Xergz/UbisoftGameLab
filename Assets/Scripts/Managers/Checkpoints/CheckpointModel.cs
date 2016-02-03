using System.Collections.Generic;
using System.IO;

/// <summary>
/// This class is the data representation of the checkpoints
/// </summary>
public class CheckpointModel {
	private Stack<Checkpoint> checkpoints;
	private string FILE_EXTENSION = "chk";
	private byte[] VALID_HEADER_MAGIC = { 67, 72, 69,  75};
	private byte   VALID_HEADER_VERSION = 10;

	/// <summary>
	/// Describe the file header
	/// </summary>
	private struct FileHeader {
		public byte[] Magic;
		public byte Version;
	}

	/// <summary>
	/// Get the current checkpoint
	/// </summary>
	/// <value>The last.</value>
	public Checkpoint Current {
		get {
			return checkpoints.Peek ();
		}
	}

	/// <summary>
	/// Discard the current checkpoint
	/// </summary>
	public void Discard() {
		checkpoints.Pop ();
	}

	/// <summary>
	/// Update the checkpoint
	/// </summary>
	/// <param name="newCheckpoint">The new checkpoint.</param>
	public void Update(Checkpoint newCheckpoint) {
		checkpoints.Push (newCheckpoint);
	}

	/// <summary>
	/// Save the model into a permanent representation
	/// </summary>
	/// <param name="file">The file where to save the model.</param>
	public void SaveTo(string file) {
		// Create a temporary file where the data will be saved
		// A temporary file is used to prevent save corruption,
		// In a case where a corruption happened, the original file 
		// will still be usable
		string tempFile = Path.GetTempFileName();
		Stack<Checkpoint> lastCheckpoints = new Stack<Checkpoint> (this.checkpoints);

		using (BinaryWriter writer = new BinaryWriter(File.Open(tempFile, FileMode.Open))) {
			// Write the file header
			for (int i = 0; i < 4; i++) {
				writer.Write (VALID_HEADER_MAGIC [i]);
			}
			writer.Write (VALID_HEADER_VERSION);

			// Write
		}
			
		// TODO: Replace the current save with tempFile
	}

	/// <summary>
	/// Load the model from a permanent representation
	/// </summary>
	/// <param name="file">The file from where to load the model.</param>
	public void LoadFrom(string file) {
	}
}