using System;
using Extensions;
using UnityEngine;

namespace SimpleVehicle.SimpleCar
{
	[Serializable]
	public class ControlAssist
	{
		[SerializeField] private float airControlSpeed = 3f;
		[SerializeField] private float airControlForce = 10f;
		[SerializeField] private PIDController airControllerX;
		[SerializeField] private PIDController airControllerZ;

		private CarComponent carComponent;

		public void ControlAssistAwake(CarComponent carComponent)
		{
			this.carComponent = carComponent;
		}

		public void AirControl()
		{
			// The car is not falling
			if (carComponent.carRb.velocity.y >= 0f) return;
            
			// The car is upside down
			if (Vector3.Dot(carComponent.transform.up, Vector3.up) <= 0f) return;

			Ray ray = new Ray(carComponent.transform.position, carComponent.carRb.velocity.normalized);
			if (Physics.Raycast(ray, out RaycastHit hitInfo))
			{
				Vector3 controlTorque = Vector3.zero;

				Vector3 currentAngles = carComponent.transform.up;
				
				float xTorque = airControllerX.Regulate(currentAngles.x, hitInfo.normal.x);
				float zTorque = airControllerZ.Regulate(currentAngles.z, hitInfo.normal.z);

				controlTorque.x = zTorque;
				controlTorque.z = -xTorque;
				
				carComponent.carRb.AddTorque(controlTorque * airControlForce);
			}
		}
	}
}
