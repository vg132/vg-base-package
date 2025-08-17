using Newtonsoft.Json;
using System;
using System.Collections;
using System.Threading.Tasks;

#if UNITY_PS5
using Unity.SaveData.PS5.Core;
using Unity.SaveData.PS5.Dialog;
using Unity.SaveData.PS5.Info;
using Unity.SaveData.PS5.Mount;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
using VGSoftware.Assets.Scripts.Core.IO.Models;
using VGSoftware.Assets.Code.Scripts.Gameplay.IO;
using VGSoftware.Framework;

namespace VGSoftware.Assets.Scripts.Core.IO
{
	public class GameDataHandler : MonoBehaviour
	{
		public static GameDataHandler Instance { get; private set; }

		public SaveFileData SaveData { get; private set; }
		public SettingsFileData SettingsData { get; private set; }
		public bool IsLoaded { get; private set; }

		private bool _saveDataQueued = false;

#if UNITY_EDITOR
		private static string SaveDataFileName = "savedata_dev.dat";
		private static string SettingsDataFileName = "settingsdata_dev.dat";
#else
		private static string SaveDataFileName = "savedata.dat";
		private static string SettingsDataFileName = "settingsdata.dat";
#endif

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				DontDestroyOnLoad(this);
				Instance = this;
			}
		}

		private void Start()
		{
			Load();
		}

		private void OnEnable()
		{
			Simulation.Event<SaveDataLoaded>.OnExecute += OnSaveDataLoaded;
			Simulation.Event<SaveDataCompleted>.OnExecute += OnSaveDataCompleted;
		}

		private void OnDisable()
		{
			Simulation.Event<SaveDataLoaded>.OnExecute -= OnSaveDataLoaded;
			Simulation.Event<SaveDataCompleted>.OnExecute -= OnSaveDataCompleted;
		}

		private void OnSaveDataCompleted(SaveDataCompleted completed)
		{
			_saveDataQueued = false;
		}

		private void OnSaveDataLoaded(SaveDataLoaded loaded)
		{
			SaveData = loaded.SaveFileData;
			SettingsData = loaded.SettingsFileData;
			IsLoaded = true;
		}

		public void Save()
		{
			if (!_saveDataQueued)
			{
				_saveDataQueued = true;
#if UNITY_PS5
				StartCoroutine(PS5SaveData());
#else
				StartCoroutine(PCSaveData());
#endif
			}
			else
			{
				GameLog.Log("Call in progress");
			}
		}

		private void Load()
		{
#if UNITY_PS5
			PS5LoadData();
#else
			PCLoadData();
#endif
		}

#if UNITY_PS5
		private IEnumerator PS5SaveData()
		{
			// Wait for 120 frames until we execute the save function
			int count = 0;
			while (count < 120)
			{
				count++;
				yield return null;
			}
			var userId = GameStateManager.Instance.CurrentUser.UserId;

			var fileRequest = new PS5WriteFileRequest();
			fileRequest.SaveFileData = SaveData;

			fileRequest.IgnoreCallback = false;
			var fileResponse = new PS5WriteFileResponse();
			var subTitle = "Save File Sub Title";

			StartCoroutine(PS5FileManager.StartAutoSaveProcess(userId, subTitle, fileRequest, fileResponse, HandleAutoSaveError));
		}

		private void PS5LoadData()
		{
			// Get the user id for the saves
			var userId = GameStateManager.Instance.CurrentUser.UserId;
			var fileRequest = new PS5ReadFileRequest();
			fileRequest.IgnoreCallback = false;
			var fileResponse = new PS5ReadFileResponse();
			StartCoroutine(PS5FileManager.StartAutoSaveLoadProcess(userId, fileRequest, fileResponse, HandleAutoSaveError));
		}

		private void HandleAutoSaveError(uint errorCode)
		{
			if (errorCode == Constants.ErrorCodes.IO.SaveDataNotFound)
			{
				GameLog.Log("*** Save data not found, create new save data");
				SaveDataLoaded.Schedule(new SaveFileData());
			}
			else if (errorCode == (uint)ReturnCodes.DATA_ERROR_NO_SPACE_FS)
			{
				GameLog.Log($"*** Save data error: {errorCode} (DATA_ERROR_NO_SPACE_FS)");
			}
			else if (errorCode == (uint)ReturnCodes.SAVE_DATA_ERROR_BROKEN)
			{
				GameLog.Log($"*** Save data error: {errorCode} (SAVE_DATA_ERROR_BROKEN)");
			}
			else
			{
				GameLog.Log($"*** Save File Error: {errorCode}");
				Sony.PS5.Dialog.Common.ShowErrorMessage(errorCode, GameStateManager.Instance.CurrentUser.UserId);
			}
		}
#else
		private IEnumerator PCSaveData()
		{
			// Wait for 120 frames until we execute the save function
			int count = 0;
			while (count < 120)
			{
				count++;
				yield return null;
			}
			new Task(() =>
			{
				var jsonData = JsonConvert.SerializeObject(SaveData, Formatting.None);
				FileManager.WriteToFile(SaveDataFileName, jsonData);
				jsonData = JsonConvert.SerializeObject(SettingsData, Formatting.None);
				FileManager.WriteToFile(SettingsDataFileName, jsonData);
				SaveDataCompleted.Schedule();
			}).Start();
		}

		private void PCLoadData()
		{
			var jsonData = FileManager.ReadFile(SaveDataFileName);
			var saveFileData = JsonConvert.DeserializeObject<SaveFileData>(jsonData) ?? new SaveFileData();
			jsonData = FileManager.ReadFile(SettingsDataFileName);
			var settingsFileData = JsonConvert.DeserializeObject<SettingsFileData>(jsonData) ?? new SettingsFileData();

			SaveDataLoaded.Schedule(saveFileData, settingsFileData);
		}
#endif
	}
}