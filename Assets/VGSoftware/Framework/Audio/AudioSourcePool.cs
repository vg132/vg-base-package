using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

namespace Assets.VGSoftware.Framework.Audio
{
	public class AudioSourcePool : MonoBehaviour
	{
		public enum MixerGroups
		{
			Music,
			SFX
		}

		[SerializeField]
		private int _poolSize = 10;

		[SerializeField]
		private AudioSource _audioSourcePrefab;

		[SerializeField]
		private AudioMixerGroup _sfxMixerGroup;

		[SerializeField]
		private AudioMixerGroup _musicMixerGroup;

		private Queue<AudioSource> _audioSourcePool;
		private GameObject _audioSoruceContainer;

		public static AudioSourcePool Instance { get; private set; }

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

				_audioSourcePool = new Queue<AudioSource>();
				_audioSoruceContainer = new GameObject("Audio Sources");
				_audioSoruceContainer.transform.parent = transform;
				InitializePool();
			}
		}

		private void InitializePool()
		{
			for (int i = 0; i < _poolSize; i++)
			{
				var newAudioSource = Instantiate(_audioSourcePrefab, _audioSoruceContainer.transform).GetComponent<AudioSource>();
				newAudioSource.gameObject.SetActive(false);
				_audioSourcePool.Enqueue(newAudioSource);
			}
		}

		public AudioSource GetAudioSource(MixerGroups mixerGroup)
		{
			AudioSource audioSource = null;
			if (_audioSourcePool.Count > 0)
			{
				audioSource = _audioSourcePool.Dequeue();
			}
			else
			{
				audioSource = Instantiate(_audioSourcePrefab, _audioSoruceContainer.transform).GetComponent<AudioSource>();
			}
			audioSource.gameObject.SetActive(true);
			audioSource.clip = null;
			audioSource.loop = false;
			audioSource.volume = 1.0f;
			audioSource.outputAudioMixerGroup = mixerGroup == MixerGroups.Music ? _musicMixerGroup : _sfxMixerGroup;
			return audioSource;
		}

		public void ReturnToPool(AudioSource audioSource)
		{
			audioSource.gameObject.SetActive(false);
			audioSource.clip = null;
			_audioSourcePool.Enqueue(audioSource);
		}
	}
}