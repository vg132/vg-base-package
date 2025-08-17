using System;
using System.IO;
using System.Text;
using UnityEngine;
using VGSoftware.Framework;

namespace VGSoftware.Assets.Scripts.Core.IO
{
	public static class FileManager
	{
		private static string PersistentDataPath = null;

		static FileManager()
		{
			PersistentDataPath = Application.persistentDataPath;
		}

		public static bool WriteToFile(string fileName, string fileContent)
		{
			var result = false;
			var fullPath = Path.Combine(PersistentDataPath, fileName);
			GameLog.Log($"Write File: {fileName}");
			try
			{
				var data = Encoding.Default.GetBytes(fileContent);
#if !UNITY_EDITOR
				EncryptDecryptString(data);
#endif
				File.WriteAllBytes(fullPath, data);
				result = true;
			}
			catch (Exception ex)
			{
				GameLog.LogException(ex);
			}
			return result;
		}

		public static string ReadFile(string fileName)
		{
			var result = string.Empty;
			var fullPath = Path.Combine(PersistentDataPath, fileName);
			if (!File.Exists(fullPath))
			{
				return result;
			}
			try
			{
				var data = File.ReadAllBytes(fullPath);
#if !UNITY_EDITOR
				EncryptDecryptString(data);
#endif
				result = Encoding.UTF8.GetString(data);
			}
			catch (Exception ex)
			{
				GameLog.LogException(ex);
			}
			return result ?? string.Empty;
		}

#if !UNITY_EDITOR
		private static void EncryptDecryptString(byte[] data)
		{
			byte secret = 153;
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = (byte)(data[i] ^ secret);
			}
		}
#endif
	}
}