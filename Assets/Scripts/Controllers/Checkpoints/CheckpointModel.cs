﻿using System.Collections.Generic;
using System.IO;

/// <summary>
/// This class is the data representation of the checkpoints
/// </summary>
public class CheckpointModel {
	private Stack<Checkpoint> checkpoints = new Stack<Checkpoint>();
	private string FILE_EXTENSION = "chk";
	private byte[] VALID_HEADER_MAGIC = { 67, 72, 69,  75};
	private byte   VALID_HEADER_VERSION = 13;
	private List<System.UInt32> guids = new List<System.UInt32>();

	/// <summary>
	/// Describe the file header
	/// </summary>
	private struct FileHeader {
		public byte[] Magic;
		public byte Version;
	}

	/// <summary>
	/// The number of saved checkpoints
	/// </summary>
	/// <value>The number of checkpoints</value>
	public System.UInt32 Count {
		get {
			return (System.UInt32)this.checkpoints.Count;
		}
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

    private static System.UInt32 ZoneToUint(EnumZone zone) {
        System.UInt32 id = 0;

        switch (zone) {
        case EnumZone.LEVEL_1:
            id = 1;
            break;
        case EnumZone.LEVEL_2:
            id = 2;
            break;
        case EnumZone.LEVEL_3:
            id = 3;
            break;
        case EnumZone.LEVEL_4:
            id = 4;
            break;
        }

        return id;
    }

    private static EnumZone UintToZone(System.UInt32 zoneID) {
        EnumZone zone = EnumZone.OPEN_WORLD;

        switch (zoneID) {
        case 1:
            zone = EnumZone.LEVEL_1;
            break;
        case 2:
            zone = EnumZone.LEVEL_2;
            break;
        case 3:
            zone = EnumZone.LEVEL_3;
            break;
        case 4:
            zone = EnumZone.LEVEL_4;
            break;
        }

        return zone;
    }

	private void saveCheckpointTo(BinaryWriter writer, Checkpoint checkpoint) {
		// Writing guid
		writer.Write (checkpoint.GUID);

		// Writing scene ID
        writer.Write (ZoneToUint(checkpoint.Zone));

		// Writing the position
		writer.Write ((System.Single)checkpoint.Position.x);
		writer.Write ((System.Single)checkpoint.Position.y);

		// Writing the orientation
		writer.Write(checkpoint.Orientation);

		// Writing the current life value of the player
		writer.Write (checkpoint.CurrentLife);

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
        checkpoint.Zone = UintToZone(reader.ReadUInt32 ());

		// Reading the position
		checkpoint.Position.x = reader.ReadSingle();
		checkpoint.Position.y = reader.ReadSingle ();

		// Reading the orientation
		checkpoint.Orientation = reader.ReadUInt16 ();

		checkpoint.CurrentLife = reader.ReadUInt32 ();

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
					throw new InvalidFileFormatException();
				}
			}

			byte version = reader.ReadByte ();

			if (version != VALID_HEADER_VERSION) {
				throw new InvalidVersionException (version, VALID_HEADER_VERSION);
			}

			// We move at the end of the file to read the checkpoints table offset
			reader.BaseStream.Seek (-4, SeekOrigin.End);
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

			// Load every checkpoints and store them in the stack
			for (int i = 0; i < checkpointOffsets.Length; i++) {
				reader.BaseStream.Seek (checkpointOffsets[i], SeekOrigin.Begin);

				Checkpoint loadedCheckpoint = new Checkpoint ();

				LoadCheckpointFrom (reader, ref loadedCheckpoint);

				checkpoints.Push (loadedCheckpoint);
			}
		}
	}

	public void Clear(string file) {
		string saveFile =  Filesystem.GetSaveDirectory () + Path.GetFileNameWithoutExtension(file) + "." + FILE_EXTENSION;

		if (File.Exists (saveFile)) {
			File.Delete (saveFile);
		}

		this.checkpoints.Clear ();
	}
}
