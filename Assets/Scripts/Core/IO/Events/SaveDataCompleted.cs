using System;
using VGSoftware.Framework;

namespace VGSoftware.Assets.Code.Scripts.Gameplay.IO
{
	public class SaveDataCompleted : Simulation.Event<SaveDataCompleted>
	{
		public static void Schedule() => Simulation.Schedule<SaveDataCompleted>();
		public static Action ScheduleAction() => () => Schedule();
	}
}