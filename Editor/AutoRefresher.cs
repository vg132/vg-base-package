using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
	[InitializeOnLoad]
	public class AutoRefresher
	{
		private static FileSystemWatcher _watcher;
		private static bool _doReload = false;
		private static IList<string> _files = new List<string>();

		static AutoRefresher()
		{
			// FileSystemWatcher needs to be disposed after its work is done, so we disposing it before assembly reloads
			AssemblyReloadEvents.beforeAssemblyReload += CleanUp;
			EditorApplication.playModeStateChanged += OnPlayModeChanged;

			// settings for FileSystemWatcher, making shure it is capturing all changes in script files
			_watcher = new FileSystemWatcher(Path.GetFullPath(Application.dataPath));
			_watcher.Filter = "*.cs";
			_watcher.Changed += OnAssetsChanged;
			_watcher.Created += OnAssetsChanged;
			_watcher.Deleted += OnAssetsChanged;
			_watcher.Renamed += OnAssetsChanged;
			_watcher.IncludeSubdirectories = true;
			_watcher.EnableRaisingEvents = true;
		}

		private static void OnAssetsChanged(object sender, FileSystemEventArgs e)
		{
			// if any script was changed then database should be reloaded before entering play mode
			_doReload = true;
			if (!_files.Contains(e.Name))
			{
				_files.Add(e.Name);
				Debug.Log($"File change detected ({e.Name}), will refresh asset database when entering play mode.");
			}
		}

		private static void OnPlayModeChanged(PlayModeStateChange state)
		{
			switch (state)
			{
				case PlayModeStateChange.ExitingEditMode:
					OnPlayModeEntered();
					break;
			}
		}

		private static void CleanUp()
		{
			if (_watcher != null)
			{
				_watcher.Dispose();
				_files.Clear();
			}
		}

		private static void OnPlayModeEntered()
		{
			if (_doReload)
			{
				Reload();
			}
		}

		[MenuItem("File/Refresh asset database", priority = 212)]
		private static void Build()
		{
			Reload();
		}

		private static void Reload()
		{
			Debug.Log("Asset database refresh");
			_doReload = false;
			_files.Clear();
			AssetDatabase.Refresh();
		}
	}
}