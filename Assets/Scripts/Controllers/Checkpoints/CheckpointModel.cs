﻿using System.Collections.Generic;
using System.IO;

/// <summary>
/// This class is the data representation of the checkpoints
/// </summary>
public class CheckpointModel {
	private Stack<Checkpoint> checkpoints = new Stack<Checkpoint>();
	private string FILE_EXTENSION = "chk";
	private byte[] VALID_HEADER_MAGIC = { 67, 72, 69,  75};
	private byte   VALID_HEADER_VERSION = 10;
	private List<System.UInt32> guids = new List<System.UInt32>();

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

	public bool ContainsGUID(System.UInt32 guid) {
		return guids.Contains (guid);
	}

	private void saveCheckpointTo(BinaryWriter writer, Checkpoint checkpoint) {
		// Writing guid
		writer.Write (checkpoint.GUID);

		// Writing scene ID
		writer.Write (checkpoint.SceneID);

		// Writing current life
		writer.Write (checkpoint.CurrentLife);

		// Writing the position
        writer.Write ((System.Single)checkpoint.Position.x);
        writer.Write ((System.Single)checkpoint.Position.y);

		// Writing the orientation
		writer.Write(checkpoint.Orientation);

		// Writing collectables
		writer.Write ((System.UInt32)checkpoint.Collectables.Count);
		foreach (KeyValuePair<System.UInt32, bool> pair in checkpoint.Collectables) {
			// 31 bits for collectable id + 1 bit for associated boolean value
			System.UInt32 collectable = (System.UInt32)(pair.Key << 1);

			if (pair.Value) {
				collectable |= 1;
			}

			writer.Write (collectable);
		}
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
		string saveFile =  Filesystem.GetSaveDirectory () + Path.GetFileNameWithoutExtension(file) + "." + FILE_EXTENSION;

		//Stack<Checkpoint> lastCheckpoints = new Stack<Checkpoint> (this.checkpoints);
		Stack<Checkpoint> reverseCheckpoints = new Stack<Checkpoint> (this.checkpoints);

		long[] checkPointOffsets = new long[reverseCheckpoints.Count];

		using (BinaryWriter writer = new BinaryWriter(File.Open(tempFile, FileMode.Open))) {
			// Write the file header
			for (int i = 0; i < 4; i++) {
				writer.Write (VALID_HEADER_MAGIC [i]);
			}
			writer.Write (VALID_HEADER_VERSION);

			// Write checkpoints
			long checkPointIndex = 0;
			while (reverseCheckpoints.Count > 0) {
				Checkpoint checkpoint = reverseCheckpoints.Pop ();

				checkPointOffsets [checkPointIndex] = writer.BaseStream.Position;

				saveCheckpointTo (writer, checkpoint);

				checkPointIndex++;
			}

			long tableOffset = writer.BaseStream.Position;

			// Write the number of entries in the table
			writer.Write ((System.UInt32)checkPointOffsets.Length);

			// Write checkpoint table
			for (int i = 0; i < checkPointOffsets.Length; i++) {
				writer.Write ((System.UInt32)checkPointOffsets[i]);
			}

			// Write the table's offset
			writer.Write((System.UInt32)tableOffset);
		}

		// On supprime l'ancienne sauvegarde
		if (File.Exists (saveFile)) {
			File.Delete (saveFile);
		}

		// On déplace le fichier temporaire à la place du fichier de sauvegarde
		File.Move (tempFile, saveFile);
	}
		

	private void LoadCheckpointFrom(BinaryReader reader, ref Checkpoint checkpoint) {
		// Reading GUID
		checkpoint.GUID = reader.ReadUInt32 ();
		guids.Add (checkpoint.GUID);

		// Reading scene ID
		checkpoint.SceneID = reader.ReadUInt32 ();

		// Reading current life
		checkpoint.CurrentLife = reader.ReadUInt32 ();

		// Reading the position
		checkpoint.Position.x = reader.ReadSingle();
		checkpoint.Position.y = reader.ReadSingle ();

		// Reading the orientation
		checkpoint.Orientation = reader.ReadUInt16 ();

		// Reading collectables
		System.UInt32 collectableCount = reader.ReadUInt32();

		for (System.UInt32 i = 0; i < collectableCount; i++) {
			System.UInt32 collectable = reader.ReadUInt32 ();

			bool collected = (collectable & 1) != 0;
			System.UInt32 collectableID = (collectable >> 1);

			checkpoint.Collectables.Add (collectableID, collected);
		}
	}

	/// <summary>
	/// Load the model from a permanent representation
	/// </summary>
	/// <param name="file">The file from where to load the model.</param>
	public void LoadFrom(string file) {
		// Clear the current state of the stack
		checkpoints.Clear ();
		guids.Clear ();

		string saveFile =  Filesystem.GetSaveDirectory () + Path.GetFileNameWithoutExtension(file) + "." + FILE_EXTENSION;

		using (BinaryReader reader = new BinaryReader (File.Open(saveFile, FileMode.Open))) {
			// Read the 32 bit magic number
			for (int i = 0; i < VALID_HEADER_MAGIC.Length; i++) {
				char magic = reader.ReadChar ();

				if (magic != VALID_HEADER_MAGIC [i]) {
					// TODO: Throw an exception
					// The header magic number is invalid
				}
			}

			byte version = reader.ReadByte ();

			if (version != VALID_HEADER_VERSION) {
				// TODO: Throw an exception
				// Unsupported version
			}

			// We move at the end of the file to read the checkpoints table offset
			reader.BaseStream.Seek (-4, SeekOrigin.End);

			System.UInt32 endOfTable = (System.UInt32)reader.BaseStream.Position;
			System.UInt32 tableOffset = reader.ReadUInt32 ();

			// We move at the beginning of the checkpoints table
			reader.BaseStream.Seek (tableOffset, SeekOrigin.Begin);

			// We read the number of checkpoints
			System.UInt32 checkpointsCount = reader.ReadUInt32 ();
			System.UInt32[] checkpointOffsets = new System.UInt32[checkpointsCount];

			// Read every entries in the table
			for (int i = 0; i < checkpointsCount; i++) {
				checkpointOffsets [i] = reader.ReadUInt32 ();
			}


            //Stack<Checkpoint> reverseCheckpointsStack = new Stack<Checkpoint> ();

			// Load every checkpoints and store them in the stack
			for (int i = 0; i < checkpointOffsets.Length; i++) {
				reader.BaseStream.Seek (checkpointOffsets[i], SeekOrigin.Begin);

				Checkpoint loadedCheckpoint = new Checkpoint ();

				LoadCheckpointFrom (reader, ref loadedCheckpoint);

                checkpoints.Push (loadedCheckpoint);
			}

            //this.checkpoints = new Stack<Checkpoint> (reverseCheckpointsStack);

		}
	}
}