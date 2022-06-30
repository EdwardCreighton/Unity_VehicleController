using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace SimpleVehicle.SimpleCar
{
	[Serializable]
	public class SFXBehaviour
	{
		#region Fields

		[Header("General")]
		[SerializeField] private AudioMixerGroup mixerGroup;
		public AudioMixerGroup MixerGroup => mixerGroup;
		[Header("Engine")]
		[SerializeField] private DoubleClipSettings engineClipSettings;

		[Header("Brakes")]
		[SerializeField] private ClipSettings brakesClipSettings;

		[Header("Crash")]
		[SerializeField] private List<ClipSettings> crashClipSettings;

		private AudioSource engineSource;
		private AudioSource brakesSource;
		private AudioSource oneShotSfxSource;

		private CarComponent carComponent;

		#endregion

		public void SFX_Awake(CarComponent carComponent)
		{
			this.carComponent = carComponent;
			
			InitEngine();
			InitBrakes();
			InitOneShotSfx();
		}

		public void SFX_Update()
		{
			SetEngineSound();
			Brakes();
		}

		public void PlayCrashSound()
		{
			if (crashClipSettings.Count == 0) return;
			
			int index = Random.Range(0, crashClipSettings.Count);
			
			oneShotSfxSource.PlayOneShot(crashClipSettings[index].clip, crashClipSettings[index].volumeLevel);
		}

		#region Inits

		private void InitEngine()
		{
			if (!engineClipSettings.accelerationClip) return;
			
			engineSource = carComponent.gameObject.AddComponent<AudioSource>();

			engineSource.loop = engineClipSettings.loop;
			engineSource.spatialBlend = engineClipSettings.spatialBlend;
			engineSource.minDistance = engineClipSettings.minDistance;
			engineSource.maxDistance = engineClipSettings.maxDistance;
			engineSource.dopplerLevel = engineClipSettings.dopplerLevel;

			engineSource.outputAudioMixerGroup = mixerGroup;

			if (!engineClipSettings.decelerationClip)
			{
				engineClipSettings.decelerationClip = engineClipSettings.accelerationClip;
				engineClipSettings.decelerationVolumeCurve = engineClipSettings.accelerationVolumeCurve;
				engineClipSettings.decelerationPitchCurve = engineClipSettings.accelerationPitchCurve;
			}
		}

		private void InitBrakes()
		{
			if (!brakesClipSettings.clip) return;
			
			brakesSource = carComponent.gameObject.AddComponent<AudioSource>();

			brakesSource.clip = brakesClipSettings.clip;
			brakesSource.pitch = 1f;
			brakesSource.loop = brakesClipSettings.loop;
			brakesSource.spatialBlend = brakesClipSettings.spatialBlend;
			brakesSource.minDistance = brakesClipSettings.minDistance;
			brakesSource.maxDistance = brakesClipSettings.maxDistance;
			brakesSource.dopplerLevel = brakesClipSettings.dopplerLevel;

			brakesSource.outputAudioMixerGroup = mixerGroup;
		}

		private void InitOneShotSfx()
		{
			if (crashClipSettings.Count == 0) return;
			
			oneShotSfxSource = carComponent.gameObject.AddComponent<AudioSource>();
			
			oneShotSfxSource.loop = false;
			oneShotSfxSource.volume = 1f;
			oneShotSfxSource.spatialBlend = 1f;
			oneShotSfxSource.minDistance = 5f;
			oneShotSfxSource.maxDistance = 10f;
			oneShotSfxSource.dopplerLevel = 0f;

			oneShotSfxSource.outputAudioMixerGroup = mixerGroup;
		}

		#endregion

		private void SetEngineSound()
		{
			bool throttle = carComponent.controlData.throttle > 0f;
			float velocityNorm = carComponent.velocityNorm;
			
			if (throttle)
			{
				if (engineSource.clip != engineClipSettings.accelerationClip)
				{
					engineSource.clip = engineClipSettings.accelerationClip;
					engineSource.Play();
				}
				
				engineSource.volume = engineClipSettings.accelerationVolumeCurve.Evaluate(velocityNorm) * engineClipSettings.accelerationVolumeLevel;
				engineSource.pitch = engineClipSettings.accelerationPitchCurve.Evaluate(velocityNorm);
				
				return;
			}

			if (engineSource.clip != engineClipSettings.decelerationClip)
			{
				engineSource.clip = engineClipSettings.decelerationClip;
				engineSource.Play();
			}
			
			engineSource.volume = engineClipSettings.decelerationVolumeCurve.Evaluate(velocityNorm) * engineClipSettings.decelerationVolumeLevel;
			engineSource.pitch = engineClipSettings.decelerationPitchCurve.Evaluate(velocityNorm);
		}

		private void Brakes()
		{
			if (!brakesClipSettings.clip) return;
			
			brakesSource.volume = carComponent.controlData.brake * brakesClipSettings.volumeCurve.Evaluate(
				Mathf.Abs(carComponent.velocity.z)) * brakesClipSettings.volumeLevel;

			if (brakesSource.volume >= 0.01f && !brakesSource.isPlaying)
			{
				brakesSource.Play();
			}
			else if (brakesSource.volume < 0.01f && brakesSource.isPlaying)
			{
				brakesSource.Stop();
			}
		}
	}
}
