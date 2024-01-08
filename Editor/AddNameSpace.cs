using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
	public class AddNameSpace : AssetModificationProcessor
	{
		public static void OnWillCreateAsset(string path)
		{
			try
			{
				if (!path.EndsWith(".cs.meta"))
				{
					return;
				}
				path = path.Replace(".meta", "");
				var index = path.LastIndexOf(".");
				if (index < 0)
				{
					return;
				}
				var file = path.Substring(index);
				index = Application.dataPath.LastIndexOf("Assets");
				path = Application.dataPath.Substring(0, index) + path;
				if(!System.IO.File.Exists(path))
				{
					Debug.Log("Unable to update namespace, file not created");
					return;
				}
				file = System.IO.File.ReadAllText(path);

				var lastPart = path.Substring(path.IndexOf("Assets"));

				var namespaceParts = lastPart.Substring(0, lastPart.LastIndexOf('/')).Split('/');
				if (namespaceParts.Any())
				{
					var namespaceName = string.Join(".", namespaceParts);
					if (!string.IsNullOrEmpty(EditorSettings.projectGenerationRootNamespace))
					{
						namespaceName = $"{EditorSettings.projectGenerationRootNamespace}.{namespaceName}";
					}
					file = file.Replace("#NAMESPACE#", namespaceName);
					System.IO.File.WriteAllText(path, file);
					AssetDatabase.Refresh();
				}
			}
			catch(Exception ex)
			{
				Debug.LogWarning($"Unable to update namespace in script file: '{ex.Message}'");
			}
		}
	}
}
