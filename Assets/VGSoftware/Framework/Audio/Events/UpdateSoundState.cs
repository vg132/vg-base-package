using System;
using VGSoftware.Framework;

namespace Assets.VGSoftware.Framework.Audio.Events
{
	public class UpdateSoundState : Simulation.Event<UpdateSoundState>
  {
		public bool EnableMusic { get; set; }
		public bool EnableSFX { get; set; }

		public static void Schedule(bool enableMusic, bool enableSFX)
		{
			Simulation.CreateAndSchedule<UpdateSoundState>(ev =>
			{
				ev.EnableMusic = enableMusic;
				ev.EnableSFX = enableSFX;
			});
		}

    public static Action ScheduleAction(bool enableMusic, bool enableSFX) => () => Schedule(enableMusic, enableSFX);
	}
}