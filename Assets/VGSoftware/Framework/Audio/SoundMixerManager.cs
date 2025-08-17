using Assets.Scripts.Core;
using UnityEngine.Audio;
using UnityEngine;
using VGSoftware.Framework;
using Assets.VGSoftware.Framework.Audio.Events;

namespace Assets.VGSoftware.Framework.Audio
{
	public class SoundMixerManager : MonoBehaviour
	{
		[SerializeField]
		private AudioMixer _audioMixer;

		private void OnEnable()
		{
			Simulation.Event<UpdateSoundLevels>.OnExecute += OnUpdateSoundLevels;
			Simulation.Event<UpdateSoundState>.OnExecute += OnUpdateSoundState;
		}

		private void OnDisable()
		{
			Simulation.Event<UpdateSoundLevels>.OnExecute -= OnUpdateSoundLevels;
			Simulation.Event<UpdateSoundState>.OnExecute -= OnUpdateSoundState;
		}

		private void OnUpdateSoundState(UpdateSoundState state)
		{
			_audioMixer.SetFloat(Constants.Audio.MixerMusicVolume, state.EnableMusic ? 0 : -80);
			_audioMixer.SetFloat(Constants.Audio.MixerSoundSFVolume, state.EnableSFX ? 0 : -80);
		}

		private void OnUpdateSoundLevels(UpdateSoundLevels levels)
		{
			_audioMixer.SetFloat(Constants.Audio.MixerMusicVolume, levels.Music);
			_audioMixer.SetFloat(Constants.Audio.MixerSoundSFVolume, levels.SFX);
		}
	}
}