using UnityEngine;

namespace VGSoftware.Framework
{
	public static class GameLog
	{
		[System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEBUG")]
		public static void Log(string message, Object context = null)
		{
#if UNITY_PS5
			System.Console.WriteLine($"[Log] {message}");
#else
			Debug.Log(message, context);
#endif
		}

		[System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEBUG")]
		public static void LogWarning(string message, Object context = null)
		{
#if UNITY_PS5
			System.Console.WriteLine($"[Warning] {message}");
#else
			Debug.LogWarning(message, context);
#endif
		}

		[System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEBUG")]
		public static void LogError(string message, Object context = null)
		{
#if UNITY_PS5
			System.Console.WriteLine($"[Error] {message}");
#else
			Debug.LogError(message, context);
#endif
		}

		[System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEBUG")]
		public static void LogException(System.Exception ex, Object context = null)
		{
#if UNITY_PS5
			System.Console.WriteLine($"[Exception] {ex?.Message}");
#else
			Debug.LogException(ex, context);
#endif
		}
	}
}