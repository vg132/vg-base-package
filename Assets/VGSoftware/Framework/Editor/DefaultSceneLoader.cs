using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;

namespace VGSoftware.Framework.Editor
{
	[InitializeOnLoad]
	public class DefaultSceneLoader : EditorWindow
	{
		private const string _defaultScenePath = "Assets/Scenes/Startup.unity";
		private const string _currentSceneKey = "dsl_currentScene";

		static DefaultSceneLoader()
		{
			EditorApplication.playModeStateChanged += LoadDefaultScene;
		}

		static void LoadDefaultScene(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.ExitingEditMode)
			{
				if (EditorSceneManager.GetActiveScene().path != _defaultScenePath)
				{
					PlayerPrefs.SetString(_currentSceneKey, EditorSceneManager.GetActiveScene().path);
					PlayerPrefs.Save();

					EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

					EditorApplication.delayCall += () =>
					{
						EditorSceneManager.OpenScene(_defaultScenePath);
						EditorApplication.isPlaying = true;
					};

					EditorApplication.isPlaying = false;
				}
			}

			if (state == PlayModeStateChange.EnteredEditMode)
			{
				if (PlayerPrefs.HasKey(_currentSceneKey))
				{
					EditorSceneManager.OpenScene(PlayerPrefs.GetString(_currentSceneKey));
					PlayerPrefs.DeleteKey(_currentSceneKey);
				}
			}
		}
	}
}