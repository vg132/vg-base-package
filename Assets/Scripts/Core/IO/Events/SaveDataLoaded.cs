using System;
using VGSoftware.Assets.Scripts.Core.IO.Models;
using VGSoftware.Framework;

namespace VGSoftware.Assets.Code.Scripts.Gameplay.IO
{
	public class SaveDataLoaded : Simulation.Event<SaveDataLoaded>
	{
		public SaveFileData SaveFileData { get; set; }
		public SettingsFileData SettingsFileData { get; set; }

		public static void Schedule(SaveFileData saveFileData, SettingsFileData settingsFileData)
		{
			Simulation.CreateAndSchedule<SaveDataLoaded>(ev =>
			{
				ev.SaveFileData = saveFileData;
				ev.SettingsFileData = settingsFileData;
			});
		}

		public static Action ScheduleAction(SaveFileData saveFileData, SettingsFileData settingsFileData) => () => Schedule(saveFileData, settingsFileData);

		internal override void Cleanup()
		{
			SaveFileData = null;
		}
	}
}