using UnityEngine;
using UnityEngine.InputSystem;

namespace SimpleVehicle.SimpleCar
{
    public class InputListener : MonoBehaviour
    {
        public InputData inputData { get; private set; }

        public void Awake()
        {
            inputData = new InputData();
        }

        public void OnSteer(InputValue value)
        {
            inputData.steer = value.Get<float>();
        }

        public void OnDrive(InputValue value)
        {
            inputData.driveAxis = value.Get<float>();
        }

        public void OnHandbrake(InputValue value)
        {
            inputData.handbrake = value.Get<float>();
        }
    }
}
