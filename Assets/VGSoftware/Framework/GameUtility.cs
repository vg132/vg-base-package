using System.Collections;
using UnityEngine;

namespace VGSoftware.Framework
{
	public static class GameUtility
	{
		public static IEnumerator ExecuteAfterDelay(float delaySeconds, System.Action action)
		{
			yield return new WaitForSeconds(delaySeconds);
			action?.Invoke();
		}

		public static string FormatNumber(string number) => FormatNumber(int.Parse(number));
		public static string FormatNumber(int number) => number.ToString("#,0").Replace(",", " ");

		public static string FormatTime(float time, bool showMilliseconds = true)
		{
			var minutes = Mathf.FloorToInt(time / 60);
			var seconds = Mathf.FloorToInt(time % 60);
			if (!showMilliseconds)
			{
				return $"{minutes}m{seconds:00}s";
			}
			var milliseconds = Mathf.FloorToInt((time % 1) * 1000);
			return $"{minutes}:{seconds:00}.{milliseconds:000}";
		}
	}
}