using UnityEngine;

namespace SimpleVehicle.SimpleCar
{
    public class Friction : Suspension
    {
        #region Fields

        private float surfaceFriction;
        
        private float lateralForce;
        private float longitudinalForce;

        protected float lateralFactor;
        private float longitudinalFactor;
        
        protected Vector3 frictionForce;

        #endregion

        protected void FrictionSequence()
        {
            ComputeVelocity();
            GetSurfaceFriction();

            VFX_VelocitySmoke(wheelVelocity.magnitude >= axleInfo.surfaceVelocityThreshold);
            float sfxFactor = Mathf.InverseLerp(0f, axleInfo.sfxMaxSurfaceVelocity, wheelVelocity.magnitude);
            SFX_Surface(sfxFactor);

            ReduceSlipping();

            ComputeLateralFactor();
            ComputeLongitudinalFactor();
            ComputeCombinedForce();
        }

        private void ComputeVelocity()
        {
            wheelVelocity = transform.InverseTransformDirection(carRb.GetPointVelocity(hitInfo.point));
        }

        private void GetSurfaceFriction()
        {
            hitInfo.transform.TryGetComponent(out GroundBehaviour groundBehaviour);

            if (groundBehaviour)
            {
                SetSurface(groundBehaviour.Type);
                surfaceFriction = groundBehaviour.surfaceFriction;
                return;
            }

            SetSurface(GroundBehaviour.GroundType.ASPHALT);
            surfaceFriction = 1f;
        }

        private void ComputeLateralFactor()
        {
            float frictionCoefficient = axleInfo.tireStiffness * surfaceFriction;
            
            lateralFactor = Mathf.Clamp(-wheelVelocity.x, -axleInfo.maxLateralVelocitySlip, axleInfo.maxLateralVelocitySlip);
            lateralFactor *= frictionCoefficient;
        }

        private void ComputeLongitudinalFactor()
        {
            float frictionCoefficient = axleInfo.tireStiffness * surfaceFriction;
            
            if (locked)
            {
                longitudinalFactor = Mathf.Clamp(-wheelVelocity.z, -axleInfo.maxLongitudinalVelocitySlip, axleInfo.maxLongitudinalVelocitySlip);
                longitudinalFactor *= frictionCoefficient;

                return;
            }

            if (brake)
            {
                longitudinalFactor = Mathf.Clamp(-wheelVelocity.z, -axleInfo.maxLongitudinalVelocitySlip, axleInfo.maxLongitudinalVelocitySlip);
                longitudinalFactor *= axleInfo.brakeFactor * frictionCoefficient;
                
                return;
            }

            if (Mathf.Approximately(driveTorque, 0f))
            {
                longitudinalFactor = Mathf.Sign(-wheelVelocity.z) * 0.015f / axleInfo.wheelRadius * frictionCoefficient;

                return;
            }
            
            longitudinalFactor = driveTorque / axleInfo.wheelRadius * frictionCoefficient;
            longitudinalFactor = Mathf.Clamp(longitudinalFactor, -axleInfo.maxDriveFactorSlip, axleInfo.maxDriveFactorSlip);
        }

        private void ComputeCombinedForce()
        {
            Vector2 totalFactor = new Vector2(lateralFactor, longitudinalFactor);

            totalFactor = Vector2.ClampMagnitude(totalFactor, 1f);
            
            Vector3 rightProject = Vector3.ProjectOnPlane(transform.right, hitInfo.normal).normalized;
            Vector3 forwardProject = Vector3.ProjectOnPlane(transform.forward, hitInfo.normal).normalized;

            rightProject *= totalFactor.x * upForce;
            forwardProject *= totalFactor.y * upForce;
            
            frictionForce = rightProject + forwardProject;

            VFX_SlipSmoke((Mathf.Abs(totalFactor.x) >= axleInfo.slipFactorThreshold 
                            && Mathf.Abs(wheelVelocity.x) >= axleInfo.velocitySlipThreshold) 
                      || (locked && Mathf.Abs(wheelVelocity.z) >= axleInfo.velocitySlipThreshold));

            float sfxFactor = locked ? wheelVelocity.magnitude : Mathf.InverseLerp(axleInfo.sfxMinVelocitySlip, axleInfo.sfxMaxVelocitySlip, Mathf.Abs(wheelVelocity.x));;
            SFX_Slip(sfxFactor);
        }
        
        private void ReduceSlipping()
        {
            float lateralThreshold = 0.1f;
            if (Mathf.Abs(wheelVelocity.x) <= lateralThreshold)
            {
                suspensionForce.x = 0f;
            }

            float longitudinalThreshold = 0.1f;
            if (Mathf.Abs(wheelVelocity.z) <= longitudinalThreshold)
            {
                suspensionForce.z = 0f;
            }
        }
    }
}
