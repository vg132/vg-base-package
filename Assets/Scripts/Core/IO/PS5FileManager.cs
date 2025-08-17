#if UNITY_PS5
using System;
using System.Collections;
using Unity.SaveData.PS5.Core;
using Unity.SaveData.PS5.Dialog;
using Unity.SaveData.PS5.Info;
using Unity.SaveData.PS5.Mount;
using VGSoftware.Assets.Code.Scripts.Core;
using VGSoftware.Assets.Code.Scripts.Gameplay.IO;
using VGSoftware.Assets.Code.Scripts.Utility;

namespace VGSoftware.Assets.Scripts.Core.IO
{
	public static class PS5FileManager
	{
		public delegate void ErrorHandler(uint errorCode);

		private static ulong _saveBlockSize = Mounting.MountRequest.BLOCKS_MIN + (1024 * 1024 * 5 / Mounting.MountRequest.BLOCK_SIZE);
		private const string DirectoryName = "Autosave";

		//  delegate
		/// <summary>
		/// Save process states
		/// </summary>
		private enum SaveState
		{
			Begin,
			SaveFiles,
			WriteIcon,
			WriteParams,
			Unmount,
			HandleError,
			LoadFiles,
			Exit
		}

		/// <summary>
		/// Start the auto-save process as a Unity Coroutine.
		/// </summary>
		public static IEnumerator StartAutoSaveProcess(int userId, string subTitle, FileOps.FileOperationRequest fileRequest, FileOps.FileOperationResponse fileResponse, ErrorHandler errHandler)
		{
			var currentState = SaveState.Begin;
			var mountResponse = new Mounting.MountResponse();
			Mounting.MountPoint mountPoint = null;
			var saveDirectoryName = new DirName { Data = DirectoryName };
			var newItem = new Dialogs.NewItem();
			newItem.IconPath = "/app0/Media/StreamingAssets/SaveIcon.png";
			newItem.Title = "Autosave";
			var saveDataParams = new SaveDataParams();
			saveDataParams.Title = newItem.Title;
			saveDataParams.SubTitle = subTitle;
			var errorCode = 0;
			while (currentState != SaveState.Exit)
			{
				switch (currentState)
				{
					case SaveState.Begin:
						{
							var flags = Mounting.MountModeFlags.Create2 | Mounting.MountModeFlags.ReadWrite;
							var dirName = saveDirectoryName;
							errorCode = MountSaveData(userId, _saveBlockSize, mountResponse, dirName, flags);
							if (errorCode < 0)
							{
								currentState = SaveState.HandleError;
							}
							else
							{
								// Wait for save data to be mounted.
								while (mountResponse.Locked == true)
								{
									yield return null;
								}

								if (mountResponse.IsErrorCode == true)
								{
									errorCode = mountResponse.ReturnCodeValue;
									currentState = SaveState.HandleError;
								}
								else
								{
									// Save data is now mounted, so files can be saved.
									mountPoint = mountResponse.MountPoint;
									currentState = SaveState.SaveFiles;
								}
							}
						}
						break;
					case SaveState.SaveFiles:
						{
							// Do actual saving
							fileRequest.MountPointName = mountPoint.PathName;
							fileRequest.Async = true;
							fileRequest.UserId = userId;
							errorCode = FileOps.CustomFileOp(fileRequest, fileResponse);

							if (errorCode < 0)
							{
								currentState = SaveState.HandleError;
							}
							else
							{
								while (fileResponse.Locked == true)
								{
									yield return null;
								}
								// Write the icon and any detail parmas set here.
								var iconResponse = new EmptyResponse();
								errorCode = WriteIcon(userId, iconResponse, mountPoint, newItem);
								if (errorCode < 0)
								{
									currentState = SaveState.HandleError;
								}
								else
								{
									var paramsResponse = new EmptyResponse();
									errorCode = WriteParams(userId, paramsResponse, mountPoint, saveDataParams);
									if (errorCode < 0)
									{
										currentState = SaveState.HandleError;
									}
									else
									{
										// Wait for save icon to be mounted.
										while (iconResponse.Locked == true || paramsResponse.Locked == true)
										{
											yield return null;
										}
										currentState = SaveState.WriteIcon;
									}
								}
							}
						}
						break;
					case SaveState.WriteIcon:
						{
							// Write the icon and any detail parmas set here.
							var iconResponse = new EmptyResponse();
							errorCode = WriteIcon(userId, iconResponse, mountPoint, newItem);
							if (errorCode < 0)
							{
								currentState = SaveState.HandleError;
							}
							else
							{
								while (iconResponse.Locked == true)
								{
									yield return null;
								}
								currentState = SaveState.WriteParams;
							}
						}
						break;
					case SaveState.WriteParams:
						{
							var paramsResponse = new EmptyResponse();
							errorCode = WriteParams(userId, paramsResponse, mountPoint, saveDataParams);
							if (errorCode < 0)
							{
								currentState = SaveState.HandleError;
							}
							else
							{
								// Wait for save icon to be mounted.
								while (paramsResponse.Locked == true)
								{
									yield return null;
								}
								currentState = SaveState.Unmount;
							}
						}
						break;
					case SaveState.Unmount:
						{
							var unmountResponse = new EmptyResponse();
							errorCode = UnmountSaveData(userId, unmountResponse, mountPoint);
							if (errorCode < 0)
							{
								currentState = SaveState.HandleError;
							}
							else
							{
								while (unmountResponse.Locked == true)
								{
									yield return null;
								}
								currentState = SaveState.Exit;
							}
						}
						break;
					case SaveState.HandleError:
						{
							if (mountPoint != null)
							{
								var unmountResponse = new EmptyResponse();
								UnmountSaveData(userId, unmountResponse, mountPoint);
							}
							if (errHandler != null)
							{
								errHandler((uint)errorCode);
							}
						}
						currentState = SaveState.Exit;
						break;
				}
				yield return null;
				Simulation.Schedule<SaveDataCompleted>();
			}
		}

		/// <summary>
		/// Start the auto-save process as a Unity Coroutine.
		/// </summary>
		public static IEnumerator StartAutoSaveLoadProcess(int userId, FileOps.FileOperationRequest fileRequest, FileOps.FileOperationResponse fileResponse, ErrorHandler errHandler)
		{
			var currentState = SaveState.Begin;
			var mountResponse = new Mounting.MountResponse();
			Mounting.MountPoint mountPoint = null;
			var saveDirectoryName = new DirName { Data = DirectoryName };
			var errorCode = 0;
			while (currentState != SaveState.Exit)
			{
				switch (currentState)
				{
					case SaveState.Begin:
						{
							var flags = Mounting.MountModeFlags.ReadOnly;
							errorCode = MountSaveData(userId, 0, mountResponse, saveDirectoryName, flags);
							if (errorCode < 0)
							{
								currentState = SaveState.HandleError;
							}
							else
							{
								// Wait for save data to be mounted.
								while (mountResponse.Locked == true)
								{
									yield return null;
								}
								if (mountResponse.IsErrorCode == true)
								{
									errorCode = mountResponse.ReturnCodeValue;
									// Must handle broken save games
									//    ReturnCodes.SAVE_DATA_ERROR_BROKEN)
									currentState = SaveState.HandleError;
								}
								else
								{
									// Save data is now mounted, so files can be saved.
									mountPoint = mountResponse.MountPoint;
									currentState = SaveState.LoadFiles;
								}
							}
						}
						break;
					case SaveState.LoadFiles:
						{
							fileRequest.MountPointName = mountPoint.PathName;
							fileRequest.Async = true;
							fileRequest.UserId = userId;
							errorCode = FileOps.CustomFileOp(fileRequest, fileResponse);
							if (errorCode < 0)
							{
								currentState = SaveState.HandleError;
							}
							else
							{
								while (fileResponse.Locked == true)
								{
									yield return null;
								}
								var fileData = (PS5ReadFileResponse)fileResponse;
								SaveDataLoaded.Schedule(fileData.SaveFileData, fileData.SettingsFileData);
								currentState = SaveState.Unmount;
							}
						}
						break;
					case SaveState.Unmount:
						{
							var unmountResponse = new EmptyResponse();
							errorCode = UnmountSaveData(userId, unmountResponse, mountPoint);
							if (errorCode < 0)
							{
								currentState = SaveState.HandleError;
							}
							else
							{
								while (unmountResponse.Locked == true)
								{
									yield return null;
								}
								currentState = SaveState.Exit;
							}
						}
						break;
					case SaveState.HandleError:
						{
							if (mountPoint != null)
							{
								var unmountResponse = new EmptyResponse();
								UnmountSaveData(userId, unmountResponse, mountPoint);
							}
							if (errHandler != null)
							{
								errHandler((uint)errorCode);
							}
						}
						currentState = SaveState.Exit;
						break;
				}
				yield return null;
			}
		}

		internal static int MountSaveData(int userId, UInt64 blocks, Mounting.MountResponse mountResponse, DirName dirName, Mounting.MountModeFlags flags)
		{
			var errorCode = unchecked((int)0x80B8000E);
			try
			{
				var request = new Mounting.MountRequest();
				request.UserId = userId;
				request.IgnoreCallback = true;
				request.DirName = dirName;
				request.MountMode = flags;
				if (blocks < Mounting.MountRequest.BLOCKS_MIN)
				{
					blocks = Mounting.MountRequest.BLOCKS_MIN;
				}
				request.Blocks = blocks;
				//              request.SystemBlocks = 0;     // setting to zero specifies savedata that does no support rollback. https://game.develop.playstation.net/resources/documents/sdk/latest/SaveData-Reference/0011.html
				Mounting.Mount(request, mountResponse);
				errorCode = 0;
			}
			catch
			{
				if (mountResponse.ReturnCodeValue < 0)
				{
					errorCode = mountResponse.ReturnCodeValue;
				}
			}
			return errorCode;
		}

		internal static int UnmountSaveData(int userId, EmptyResponse unmountResponse, Mounting.MountPoint mountPoint)
		{
			var errorCode = unchecked((int)0x80B8000E);
			try
			{
				var request = new Mounting.UnmountRequest();
				request.UserId = userId;
				request.MountPointName = mountPoint.PathName;
				request.IgnoreCallback = true;
				Mounting.Unmount(request, unmountResponse);
				errorCode = 0;
			}
			catch
			{
				if (unmountResponse.ReturnCodeValue < 0)
				{
					errorCode = unmountResponse.ReturnCodeValue;
				}
			}

			return errorCode;
		}

		internal static int WriteIcon(int userId, EmptyResponse iconResponse, Mounting.MountPoint mountPoint, Dialogs.NewItem newItem)
		{
			var errorCode = unchecked((int)0x80B8000E);
			try
			{
				var request = new Mounting.SaveIconRequest();
				if (mountPoint == null)
				{
					return errorCode;
				}
				request.UserId = userId;
				request.MountPointName = mountPoint.PathName;
				request.RawPNG = newItem.RawPNG;
				request.IconPath = newItem.IconPath;
				request.IgnoreCallback = true;
				Mounting.SaveIcon(request, iconResponse);
				errorCode = 0;
			}
			catch
			{
				if (iconResponse.ReturnCodeValue < 0)
				{
					errorCode = iconResponse.ReturnCodeValue;
				}
			}
			return errorCode;
		}

		internal static int WriteParams(int userId, EmptyResponse paramsResponse, Mounting.MountPoint mountPoint, SaveDataParams saveDataParams)
		{
			var errorCode = unchecked((int)0x80B8000E);

			try
			{
				var request = new Mounting.SetMountParamsRequest();

				if (mountPoint == null)
				{
					return errorCode;
				}
				request.UserId = userId;
				request.MountPointName = mountPoint.PathName;
				request.IgnoreCallback = true;
				request.Params = saveDataParams;
				Mounting.SetMountParams(request, paramsResponse);
				errorCode = 0;
			}
			catch
			{
				if (paramsResponse.ReturnCodeValue < 0)
				{
					errorCode = paramsResponse.ReturnCodeValue;
				}
			}
			return errorCode;
		}
	}
}
#endif