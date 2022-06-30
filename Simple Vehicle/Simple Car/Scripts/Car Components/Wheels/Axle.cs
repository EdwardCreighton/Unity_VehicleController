using System;
using UnityEngine;

namespace SimpleVehicle.SimpleCar
{
    [Serializable]
    public class Axle
    {
        #region Fields

        public string axleName;
        [Header("Suspension Points")]
        public Transform suspensionPointLeft;
        public Transform suspensionPointRight;
        [Space]
        public AxleInfo axleInfo;
        [Header("Anti-Roll Bar")]
        public float springRatioThreshold;
        public float antiRollForce;
        
        public Transform[] suspensionPoints { get; set; }
        public Wheel[] wheelHolders { get; set; }   // 0 - left
                                                    // 1 - right
                                                    
        private CarComponent carComponent;
        private float[] ackermannAngles;
        private float currentSteer;

        #endregion

        public void AxleAwake(CarComponent carComponent)
        {
            if (axleInfo.isSteer) ackermannAngles = new float[2];
            
            this.carComponent = carComponent;
            
            suspensionPoints = new Transform[2];
            suspensionPoints[0] = suspensionPointLeft;
            suspensionPoints[1] = suspensionPointRight;
                
            wheelHolders = new Wheel[2];
            for (int i = 0; i < 2; i++)
            {
                wheelHolders[i] = suspensionPoints[i].gameObject.AddComponent<Wheel>();
                
                wheelHolders[i].InitWheelBase(carComponent, suspensionPoints[i], axleInfo);
                wheelHolders[i].InitWheelHolder();
            }

            axleInfo.track = Vector3.Distance(suspensionPoints[0].position, suspensionPoints[1].position);
        }

        public void AxleSequence()
        {
            for (int i = 0; i < 2; i++)
            {
                wheelHolders[i].WheelSequence();
            }
        }

        public void Steer(float targetSteer, float wheelBase)
        {
            float steer;
            
            if (Mathf.Abs(targetSteer) >= 0.05f)
            {
                steer = currentSteer + axleInfo.steerSpeed * Time.deltaTime * Mathf.Sign(targetSteer);
            }
            else
            {
                if (Mathf.Abs(currentSteer) >= 0.05f)
                {
                    steer = currentSteer - axleInfo.steerSpeed * Time.deltaTime * Mathf.Sign(currentSteer);
                }
                else
                {
                    steer = 0f;
                }
            }

            currentSteer = Mathf.Clamp(steer, -1, 1);
            
            if (currentSteer > 0) // right turn
            {
                ackermannAngles[0] = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (axleInfo.turnRadius + (axleInfo.track / 2))) * currentSteer;
                ackermannAngles[1] = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (axleInfo.turnRadius - (axleInfo.track / 2))) * currentSteer;
            }
            else if (currentSteer < 0) // left turn
            {
                ackermannAngles[0] = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (axleInfo.turnRadius - (axleInfo.track / 2))) * currentSteer;
                ackermannAngles[1] = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (axleInfo.turnRadius + (axleInfo.track / 2))) * currentSteer;
            }
            else
            {
                ackermannAngles[0] = ackermannAngles[1] = 0;
            }

            for (int i = 0; i < 2; i++)
            {
                suspensionPoints[i].localRotation = Quaternion.Euler(0f, ackermannAngles[i], 0f);
            }
        }

        public void AntiRollBar()
        {
            for (int i = 0; i < 2; i++)
            {
                if (!wheelHolders[i].isGrounded) return;
            }
            
            float maxSuspensionForce = axleInfo.deltaLength * axleInfo.springStiffness;
            float ratio = (wheelHolders[0].upForce - wheelHolders[1].upForce) / maxSuspensionForce;
            
            if (Mathf.Abs(ratio) <= springRatioThreshold) return;

            if (ratio > 0f)
            {
                Vector3 position = suspensionPoints[1].position;
                Vector3 force = -suspensionPoints[1].up * (antiRollForce * Mathf.Abs(ratio));
                carComponent.carRb.AddForceAtPosition(force, position);
            }
            else if (ratio < 0f)
            {
                Vector3 position = suspensionPoints[0].position;
                Vector3 force = -suspensionPoints[0].up * (antiRollForce * Mathf.Abs(ratio));
                carComponent.carRb.AddForceAtPosition(force, position);
            }
        }

        public void AntiPenetrator(Vector3 push)
        {
            carComponent.transform.position += push;
        }
    }
}
