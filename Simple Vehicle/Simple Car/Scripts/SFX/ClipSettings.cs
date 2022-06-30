using System;
using UnityEngine;

namespace SimpleVehicle.SimpleCar
{
	[Serializable]
	public class ClipSettings
	{
		public AudioClip clip;
		public AnimationCurve volumeCurve;
		public AnimationCurve pitchCurve;
		public float volumeLevel = 1f;
		[Space]
		public bool loop = true;
		public float spatialBlend = 1f;
		public float minDistance = 5f;
		public float maxDistance = 25f;
		public float dopplerLevel = 1f;
	}
}
