using UnityEditor;
using UnityEngine;
using System;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace VGSoftware.Framework.Editor
{
	public class BuildDateIncrementor : IPreprocessBuildWithReport
	{
		public int callbackOrder => 1;

		public void OnPreprocessBuild(BuildReport report)
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