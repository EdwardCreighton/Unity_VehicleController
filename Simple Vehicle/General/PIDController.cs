using System;
using UnityEngine;

namespace Extensions
{
	[Serializable]
	public class PIDController
	{
		#region Fields

		[SerializeField] private float kP;
		[SerializeField] private float pMin = -1f;
		[SerializeField] private float pMax = 1f;
		[Space]
		[SerializeField] private float kI;
		[SerializeField] private float iMin = -1f;
		[SerializeField] private float iMax = 1f;
		[Space]
		[SerializeField] private float kD;
		[SerializeField] private float dMin = -1f;
		[SerializeField] private float dMax = 1f;

		private float lastProportionalError;
		private float integral;

		#endregion

		public float Regulate(float currentValue, float targetValue)
		{
			float proportionalError = targetValue - currentValue;
			proportionalError = Mathf.Clamp(proportionalError, pMin, pMax);
			
			integral += proportionalError * Time.fixedDeltaTime;
			integral = Mathf.Clamp(integral, iMin, iMax);
			
			float derivative = (proportionalError - lastProportionalError) / Time.fixedDeltaTime;
			derivative = Mathf.Clamp(derivative, dMin, dMax);

			lastProportionalError = proportionalError;

			return kP * proportionalError + kI * derivative + kD * derivative;
		}
	}
}
