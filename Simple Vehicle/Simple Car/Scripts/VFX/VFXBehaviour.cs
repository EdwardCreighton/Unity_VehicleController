using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleVehicle.SimpleCar
{
	[Serializable]
	public class VFXBehaviour
	{
		#region Fields

		[SerializeField] private List<Light> brakeLights;
		[SerializeField] private float brakeLightsIntensity;
		[Space]
		[SerializeField] private List<Light> reverseLights;
		[SerializeField] private float reverseLightsIntensity;
		

		#endregion

		public void VFX_Update(CarComponent carComponent)
		{
			Reverse(carComponent.Gearbox.currentGear == Gearbox.Gear.REVERSE);
			Brakes(carComponent.controlData.brake > 0f);
		}

		private void Reverse(bool reverse)
		{
			if (reverseLights == null) return;
			
			foreach (var light in reverseLights)
			{
				light.intensity = reverse ? reverseLightsIntensity : 0f;
			}
		}

		private void Brakes(bool brakes)
		{
			if (brakeLights == null) return;
			
			foreach (var light in brakeLights)
			{
				light.intensity = brakes ? brakeLightsIntensity : 0f;
			}
		}
	}
}
