using System;
using UnityEngine;

namespace SimpleVehicle.SimpleCar
{
    [Serializable]
    public class CameraController
    {
        #region Fields

        [SerializeField] private Transform cameraHolderTransform;
        private Camera camera;
        [Space]
        [SerializeField] private AnimationCurve cameraFOVBehaviour;
        [SerializeField] private float minFOV = 60f;
        [SerializeField] private float maxFOV = 85f;
        [SerializeField] private float maxVelocity = 80f;
        [Space]
        [SerializeField] private float holderRotationSpeed = 0.05f;
        [SerializeField] private float rotationPause = 0.1f;
        [Space]
        [SerializeField] private float holderRotationSpeedForward = 0.1f;
        [SerializeField] private float rotationPauseForward = 0.1f;

        #endregion
        
        #region Rotation

        private Quaternion lastRotation;
        private float angularVelocity;
        private float rotationDeltaTime;

        #endregion

        private CarComponent carComponent;

        public void CameraControllerAwake(CarComponent carComponent)
        {
            this.carComponent = carComponent;

            camera = cameraHolderTransform.GetChild(0).GetComponent<Camera>();
        }

        public void CameraSequence()
        {
            UpdateVelocities();
            
            PlaceCameraHolder();
            RotateCameraHolder();
            
            ChangeFOV();
        }

        private void UpdateVelocities()
        {
            Quaternion currentRotation = carComponent.transform.rotation;
            angularVelocity = Quaternion.Angle(currentRotation, lastRotation);
            lastRotation = currentRotation;
        }

        #region Camera Holder

        private void PlaceCameraHolder()
        {
            cameraHolderTransform.position = carComponent.transform.position;
        }

        private void RotateCameraHolder()
        {
            float t;

            if (Mathf.Approximately(Mathf.Sqrt(angularVelocity), 0f))
            {
                rotationDeltaTime += Time.deltaTime;
                
                if (Mathf.Abs(carComponent.velocity.z) > 5f)
                {
                    t = rotationDeltaTime >= rotationPauseForward ? holderRotationSpeedForward : 0f;
                }
                else
                {
                    t = rotationDeltaTime >= rotationPause ? holderRotationSpeed : 0f;
                }
            }
            else
            {
                rotationDeltaTime = 0f;
                t = holderRotationSpeed * angularVelocity;
            }

            cameraHolderTransform.rotation = Quaternion.Slerp(cameraHolderTransform.rotation, carComponent.transform.rotation, t);
        }

        #endregion

        private void ChangeFOV()
        {
            float velocityFactor = Mathf.Max(carComponent.velocity.z, 0f) / maxVelocity;
            
            float newFOV = minFOV + (maxFOV - minFOV) * cameraFOVBehaviour.Evaluate(velocityFactor); 
            camera.fieldOfView = newFOV;
        }
    }
}
