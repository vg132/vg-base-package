using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VGSoftware.Framework.Editor
{
	public class CreateTemplates
	{
		[MenuItem("Assets/C# Script/Empty MonoBehaviour Script", priority = 10)]
		public static void EmptyMonoBehaviour()
		{
			var templatePath = "Templates\\EmptyMonoBehaviourScript.cs.txt";
			var defaultScriptName = "NewEmptyMonoBehaviourScript.cs";
			CreateTemplate(templatePath, defaultScriptName);
		}

		[MenuItem("Assets/C# Script/MonoBehaviour Script", priority = 20)]
		public static void StandardMonoBehaviour()
		{
			var templatePath = "Templates\\MonoBehaviourScript.cs.txt";
			var defaultScriptName = "NewMonoBehaviourScript.cs";
			CreateTemplate(templatePath, defaultScriptName);
		}

		[MenuItem("Assets/C# Script/Global Singleton MonoBehaviour Script", priority = 30)]
		public static void SingletonMonoBehaviour()
		{
			var templatePath = "Templates\\GlobalSingletonMonoBehaviourScript.cs.txt";
			var defaultScriptName = "NewGlobalSingletonMonoBehaviourScript.cs";
			CreateTemplate(templatePath, defaultScriptName);
		}

		[MenuItem("Assets/C# Script/Scene Singleton MonoBehaviour Script", priority = 35)]
		public static void SceneSingletonMonoBehaviour()
		{
			var templatePath = "Templates\\SceneSingletonMonoBehaviourScript.cs.txt";
			var defaultScriptName = "NewSceneSingletonMonoBehaviourScript.cs";
			CreateTemplate(templatePath, defaultScriptName);
		}

		[MenuItem("Assets/C# Script/Empty Script", priority = 40)]
		public static void EmptyScript()
		{
			var templatePath = "Templates\\EmptyScript.cs.txt";
			var defaultScriptName = "EmptyScript.cs";
			CreateTemplate(templatePath, defaultScriptName);
		}

		private static void CreateTemplate(string templatePath, string defaultScriptName)
		{
			var relativePath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
			var namespaceName = GenerateNamespace(relativePath);
			var templateFile = Path.Combine(Application.dataPath, templatePath);
			if (!File.Exists(templateFile))
			{
				Debug.Log($"Template file '{templateFile}' not found");
				return;
			}
			var template = File.ReadAllText(templateFile);
			template = template.Replace("#ROOTNAMESPACEBEGIN#", $"{namespaceName}\n{{");
			template = template.Replace("#ROOTNAMESPACEEND#", "}");

			var outputFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");
			File.WriteAllText(outputFile, template);
			ProjectWindowUtil.CreateScriptAssetFromTemplateFile(outputFile, defaultScriptName);
		}

		private static string GenerateNamespace(string path)
		{
			var namespaceParts = path.Replace("\\", "/").Split('/');
			if (!namespaceParts.Any())
			{
				return null;
			}
			var namespaceName = string.Join(".", namespaceParts).Replace(" ", string.Empty).Replace("Assets.Code.", "Assets.");
			if (!string.IsNullOrEmpty(EditorSettings.projectGenerationRootNamespace))
			{
				namespaceName = $"{EditorSettings.projectGenerationRootNamespace}.{namespaceName}";
			}
			return $"namespace {namespaceName}";
		}
	}
}