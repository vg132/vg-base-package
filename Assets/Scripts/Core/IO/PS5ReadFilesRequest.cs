#if UNITY_PS5
using Newtonsoft.Json;
using System;
using System.IO;
using Unity.SaveData.PS5.Info;
using Unity.SaveData.PS5.Mount;
using VGSoftware.Assets.Code.Scripts.IO.Models;
using VGSoftware.Assets.Code.Scripts.Utility;

namespace VGSoftware.Assets.Scripts.Core.IO
{
	public class PS5ReadFileRequest : FileOps.FileOperationRequest
	{
		public override void DoFileOperations(Mounting.MountPoint mountPoint, FileOps.FileOperationResponse response)
		{
			var fileResponse = response as PS5ReadFileResponse;

			// Read save data
			outpath = mountPoint.PathName.Data + "/savedata.json";
			if (File.Exists(outpath))
			{
				var jsonData = File.ReadAllText(outpath);
				fileResponse.SaveFileData = JsonConvert.DeserializeObject<SaveFileData>(jsonData);
				GameLog.Log($"Game file found: {jsonData}");
			}
			else
			{
				GameLog.Log("Game file not found");
				fileResponse.SaveFileData = new SaveFileData();
			}
			outpath = mountPoint.PathName.Data + "/settingsdata.json";
			if (File.Exists(outpath))
			{
				var jsonData = File.ReadAllText(outpath);
				fileResponse.SettingsFileData = JsonConvert.DeserializeObject<SettingsFileData>(jsonData);
				GameLog.Log($"Game file found: {jsonData}");
			}
			else
			{
				GameLog.Log("Game file not found");
				fileResponse.SettingsFileData = new SettingsFileData();
			}
		}
	}

	public class PS5ReadFileResponse : FileOps.FileOperationResponse
	{
		public SaveFileData SaveFileData { get; set; }
		public SettingsFileData SettingsFileData { get; set; }
	}
}
#endif