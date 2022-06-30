using UnityEngine;

namespace SimpleVehicle.SimpleCar
{
	public class DemoSceneManager : MonoBehaviour
	{
		[SerializeField] private GameObject car;
		[SerializeField] private Vector3 spawnPosition;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.R))
			{
				ResetCar();
			}
		}

		private void ResetCar()
		{
			car.transform.position = spawnPosition;
				
			car.transform.eulerAngles = Vector3.zero;
                
			Rigidbody carRb = car.GetComponent<Rigidbody>();
			carRb.velocity = Vector3.zero;
			carRb.angularVelocity = Vector3.zero;
		}
	}
}
