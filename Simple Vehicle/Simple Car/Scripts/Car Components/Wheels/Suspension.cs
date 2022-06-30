using UnityEngine;

namespace SimpleVehicle.SimpleCar
{
    public class Suspension : WheelBase
    {
        #region Fields

        private float minLength;
        private float maxLength;

        private float lastFrameLength;
        protected float currentLength;

        public float upForce { get; private set; }
        public Vector3 suspensionForce;

        #endregion

        protected void InitSuspension()
        {
            minLength = axleInfo.restLength - axleInfo.deltaLength;
            maxLength = axleInfo.restLength + axleInfo.deltaLength;
        }

        protected void SuspensionSequence()
        {
            ComputeSpringLength();
            ComputeSuspensionForce();
        }
        
        protected void GroundCheck()
        {
            isGrounded = wheelCast.SweepTest(-transform.up, out hitInfo, maxLength);
        }

        private void ComputeSpringLength()
        {
            if (isGrounded)
            {
                lastFrameLength = currentLength;
                currentLength = hitInfo.distance;

                currentLength = Mathf.Clamp(currentLength, minLength, maxLength);
            }
            else
            {
                lastFrameLength = currentLength;
                currentLength = maxLength;
            }
        }

        private void ComputeSuspensionForce()
        {
            if (isGrounded)
            {
                float deltaLength = axleInfo.restLength - currentLength;
                float springForce = deltaLength * axleInfo.springStiffness;
                
                float springVelocity = (lastFrameLength - currentLength) / Time.fixedDeltaTime;
                float damperForce = springVelocity * axleInfo.damperStiffness;

                float temp = springForce + damperForce;

                upForce = Mathf.Max(temp, 0f);

                suspensionForce = transform.up * upForce;
            }
            else
            {
                suspensionForce = Vector3.zero;
                upForce = 0f;
            }
        }
    }
}
