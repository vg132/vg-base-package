#if UNITY_PS5
using Newtonsoft.Json;
using System;
using System.IO;
using Unity.SaveData.PS5.Info;
using Unity.SaveData.PS5.Mount;
using VGSoftware.Assets.Code.Scripts.IO.Models;

namespace VGSoftware.Assets.Scripts.Core.IO
{
	public class PS5WriteFileRequest : FileOps.FileOperationRequest
	{
		public SettingsData SettingsData { get; set; }
		public StatisticsData StatisticsData { get; set; }
		public SaveFileData SaveFileData { get; set; }

		public override void DoFileOperations(Mounting.MountPoint mountPoint, FileOps.FileOperationResponse response)
		{
			var fileResponse = response as PS5WriteFileResponse;

			// Write Save Data
			outpath = mountPoint.PathName.Data + "/savedata.json";
			jsonData = JsonConvert.SerializeObject(SaveFileData, Formatting.None);
			File.WriteAllText(outpath, jsonData);

			// Write Settings Data
			outpath = mountPoint.PathName.Data + "/settingsdata.json";
			jsonData = JsonConvert.SerializeObject(SettingsData, Formatting.None);
			File.WriteAllText(outpath, jsonData);

			// Read the info about the file just written and set this on the custom response object.
			var info = new FileInfo(outpath);
			fileResponse.lastWriteTime = info.LastWriteTime;
			fileResponse.totalFileSizeWritten = info.Length;
		}
	}

	public class PS5WriteFileResponse : FileOps.FileOperationResponse
	{
		public DateTime lastWriteTime;
		public long totalFileSizeWritten;
	}
}
#endif