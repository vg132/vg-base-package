using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VGSoftware.Assets.Scripts.Editor
{
	public class AddNamespace : AssetPostprocessor
	{
		public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
		{
			var updated = false;
			foreach (string path in importedAssets)
			{
				var namespaceAddedOrUpdated = UpdateOrAddNamespace(path);
				if (namespaceAddedOrUpdated)
				{
					updated = true;
				}
			}
			foreach (string path in movedAssets)
			{
				var namespaceAddedOrUpdated = UpdateOrAddNamespace(path);
				if (namespaceAddedOrUpdated)
				{
					updated = true;
				}
			}
			if (updated)
			{
				AssetDatabase.Refresh();
			}
		}

		private static bool UpdateOrAddNamespace(string relativePath)
		{
			try
			{
				if (string.IsNullOrEmpty(relativePath) || !relativePath.ToLower().EndsWith(".cs"))
				{
					return false;
				}
				var index = Application.dataPath.LastIndexOf("Assets");
				var fullPath = $"{Application.dataPath.Substring(0, index)}{relativePath}";
				if (!File.Exists(fullPath))
				{
					Debug.Log("Unable to update namespace, file not created");
					return false;
				}
				var namespaceName = GenerateNamespace(relativePath);
				if (!string.IsNullOrEmpty(namespaceName))
				{
					var lines = File.ReadAllLines(fullPath);
					var newLines = new List<string>();
					var namespaceAdded = false;
					var addClosingCurlyBrace = false;
					foreach (var line in lines)
					{
						if (line.StartsWith("namespace ") && !namespaceAdded && !line.Trim().Equals(namespaceName, StringComparison.CurrentCultureIgnoreCase))
						{
							namespaceAdded = true;
							newLines.Add(namespaceName);
						}
						else if (!namespaceAdded && line.StartsWith("public class"))
						{
							namespaceAdded = true;
							addClosingCurlyBrace = true;
							newLines.Add(namespaceName);
							newLines.Add("{");
							newLines.Add(line);
						}
						else if (line.StartsWith("public class") && !namespaceAdded)
						{
							return false;
						}
						else
						{
							newLines.Add(line);
						}
					}
					if (addClosingCurlyBrace)
					{
						newLines.Add("}");
					}
					if (!namespaceAdded)
					{
						return false;
					}
					File.WriteAllLines(fullPath, newLines);
					return true;
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning($"Unable to update namespace in script file: '{ex.Message}'");
			}
			return false;
		}

		private static string GenerateNamespace(string path)
		{
			var namespaceParts = path.Substring(0, path.LastIndexOf('/')).Split('/');
			if (!namespaceParts.Any())
			{
				return null;
			}
			var namespaceName = string.Join(".", namespaceParts);
			if (!string.IsNullOrEmpty(EditorSettings.projectGenerationRootNamespace))
			{
				namespaceName = $"namespace {EditorSettings.projectGenerationRootNamespace}.{namespaceName}";
			}
			return namespaceName;
		}
	}
}