using System;
using UnityEngine;

namespace SimpleVehicle.SimpleCar
{
	[Serializable]
	public class SingleClipSettings
	{
		public AudioClip clip;
		public AnimationCurve volumeCurve;
		public AnimationCurve pitchCurve;
		public float volumeLevel = 1f;
	}
}
