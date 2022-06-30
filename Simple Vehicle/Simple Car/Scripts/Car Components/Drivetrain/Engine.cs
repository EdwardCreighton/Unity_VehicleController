using System;
using UnityEngine;

namespace SimpleVehicle.SimpleCar
{
	[Serializable]
	public class Engine
	{
		#region Fields

		[SerializeField] private AnimationCurve engineCurve;
		[Space]
		[SerializeField] private float forwardPower = 30f;
		[SerializeField] private float reversePower = 20f;
		[Space]
		[SerializeField] private float maxForwardSpeed = 100f;
		public float MaxForwardSpeed => maxForwardSpeed;
		[SerializeField] private float maxReverseSpeed = 60f;
		public float MaxReverseSpeed => maxReverseSpeed;

		#endregion

		public float GetTorque(float currentSpeed, float throttle, Gearbox.Gear currentGear)
		{
			switch (currentGear)
			{
				case Gearbox.Gear.FORWARD:
				{
					float normVelocity = currentSpeed / maxForwardSpeed;
					return engineCurve.Evaluate(normVelocity) * forwardPower * throttle;
				}
				case Gearbox.Gear.REVERSE:
				{
					float normVelocity = currentSpeed / maxReverseSpeed;
					return engineCurve.Evaluate(normVelocity) * reversePower * throttle * -1f;
				}
				default:
					Debug.Log("Not implemented gear behaviour.");
					return 0f;
			}
		}
	}
}
