using SimpleVehicle.SimpleCar;
using UnityEngine;

namespace SimpleVehicle.SimpleCar
{
    public class PlayerController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private CameraController cameraController;
        
        private CarComponent carComponent;
        private InputListener inputListener;

        #endregion
        
        private void Awake()
        {
            carComponent = GetComponent<CarComponent>();
            inputListener = GetComponent<InputListener>();
            
            carComponent.CarAwake();
            cameraController.CameraControllerAwake(carComponent);
        }

        private void Update()
        {
            carComponent.CarUpdate(inputListener.inputData);
        }

        private void FixedUpdate()
        {
            carComponent.CarFixedUpdate();
        }

        private void LateUpdate()
        {
            cameraController.CameraSequence();
        }
    }
}
