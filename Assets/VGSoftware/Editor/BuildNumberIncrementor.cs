using UnityEditor;
using UnityEngine;
using System;

namespace VGSoftware.Framework.Editor
{
	[InitializeOnLoad]
	public class BuildNumberIncrementor : EditorWindow
	{
		static BuildNumberIncrementor()
		{
			EditorApplication.playModeStateChanged += LoadDefaultScene;
		}

		static void LoadDefaultScene(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.ExitingEditMode)
			{
				var newBuildNumber = int.Parse(PlayerSettings.macOS.buildNumber) + 1;
				PlayerSettings.macOS.buildNumber = newBuildNumber.ToString();

				var buildNumberRepository = ScriptableObject.CreateInstance<BuildNumberRepository>();
				buildNumberRepository.BuildNumber = newBuildNumber;
				buildNumberRepository.DateString = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
				AssetDatabase.DeleteAsset("Assets/Resources/BuildNumberRepository.asset");
				AssetDatabase.CreateAsset(buildNumberRepository, "Assets/Resources/BuildNumberRepository.asset");
				AssetDatabase.SaveAssets();
			}
		}
	}
}