using System;
using VGSoftware.Framework;

namespace Assets.VGSoftware.Framework.Audio.Events
{
  public class UpdateSoundLevels : Simulation.Event<UpdateSoundLevels>
  {
    public float Music { get; set; }
    public float SFX { get; set; }

    public static void Schedule(float musicLevel, float sfxLevel)
    {
      Simulation.CreateAndSchedule<UpdateSoundLevels>(ev =>
      {
        ev.Music = musicLevel;
        ev.SFX = sfxLevel;
      });
    }

    public static Action ScheduleAction(float musicLevel, float sfxLevel) => () => Schedule(musicLevel, sfxLevel);
  }
}