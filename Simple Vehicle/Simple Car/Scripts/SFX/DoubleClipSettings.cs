using System;
using UnityEngine;

namespace SimpleVehicle.SimpleCar
{
	[Serializable]
	public class DoubleClipSettings
	{
		public AudioClip accelerationClip;
		public AnimationCurve accelerationVolumeCurve;
		public AnimationCurve accelerationPitchCurve;
		public float accelerationVolumeLevel = 1f;
		[Space]
		public AudioClip decelerationClip;
		public AnimationCurve decelerationVolumeCurve;
		public AnimationCurve decelerationPitchCurve;
		public float decelerationVolumeLevel = 1f;
		[Space]
		public bool loop = true;
		public float spatialBlend = 1f;
		public float minDistance = 5f;
		public float maxDistance = 25f;
		public float dopplerLevel = 1f;
	}
}
