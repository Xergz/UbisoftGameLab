using UnityEngine;
using System;
using System.IO;

public class Filesystem {
	private const string GAME_DIR = "gamelab";
	private const string MAC_APPLICATION_SUPPORT = "Library/Application Support";
	private const string EDITOR_SAVE_DIR = "generated";

	/// <summary>
	/// Returns the path to the directory where save files should be located
	/// </summary>
	/// <returns>The save directory.</returns>
	/// <remarks>If the directory doesn't already exists, it is created before returning</remarks>
	public static string GetSaveDirectory() {
		string saveDir = "";

		switch (Application.platform) {
		// Directory location from Windows
		case RuntimePlatform.WindowsPlayer:
			saveDir = Path.Combine(Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), GAME_DIR);
			break;
		// Directory location from OSX
		case RuntimePlatform.OSXPlayer:
			saveDir = Path.Combine(Path.Combine(Environment.GetFolderPath (Environment.SpecialFolder.Personal), MAC_APPLICATION_SUPPORT), GAME_DIR);
			break;
		// Directory location from Linux
		case RuntimePlatform.LinuxPlayer:
			saveDir = Path.Combine(Environment.GetFolderPath (Environment.SpecialFolder.Personal), "." + GAME_DIR);
			break;
		// Directory location from the editor
		case RuntimePlatform.WindowsEditor:
		case RuntimePlatform.OSXEditor:
			saveDir = Path.Combine(UnityEngine.Application.dataPath, EDITOR_SAVE_DIR);
			break;
		}

		// If the save directory doesn't exists, create it
		if (!Directory.Exists (saveDir)) {
			Directory.CreateDirectory (saveDir);
		}

		return saveDir + "/"; 
	}
}
