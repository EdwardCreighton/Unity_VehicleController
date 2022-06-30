using UnityEditor;
using UnityEngine;

namespace SimpleVehicle.SimpleCar
{
    public class Wheel : Friction
    {
        #region Fields

        public Vector3 pushDirection;
        private Vector3 pushForce;

        #endregion
        
        public void InitWheelHolder()
        {
            InitSuspension();
        }

        public void WheelSequence()
        {
            GroundCheck();
            SuspensionSequence();

            PlaceWheel();
            
            if (isGrounded)
            {
                //PushWheel();
                FrictionSequence();
                AddForce();
            }
            else
            {
                pushDirection = Vector3.zero;
                StopAllSmoke();
            }
            
            RotateWheel();
        }

        private void AddForce()
        {
            carRb.AddForceAtPosition(suspensionForce, hitInfo.point);
            carRb.AddForceAtPosition(frictionForce, hitInfo.point);
            carRb.AddForceAtPosition(pushForce, transform.position, ForceMode.Impulse);
        }

        private void PushWheel()
        {
            refPointsHolder.transform.up = wheelTransform.TransformDirection(hitInfo.normal.normalized);
            Vector3 rotation = refPointsHolder.transform.localEulerAngles;
            rotation.y = 0f;
            refPointsHolder.transform.localEulerAngles = rotation;

            float[] dotProducts = new float[2];

            for (int i = 0; i < 2; i++)
            {
                Vector3 lhs = hitInfo.normal.normalized;
                Vector3 rhs = (refPoints[i].position - hitInfo.point).normalized;

                dotProducts[i] = Vector3.Dot(lhs, rhs);
            }

            float max = 1f;

            for (int i = 0; i < 2; i++)
            {
                if (dotProducts[i] < max) max = dotProducts[i];
            }

            float penetrationThreshold = -0.1f;
            
            if (max >= penetrationThreshold)
            {
                pushDirection = Vector3.zero;
                return;
            }

            int deepestPoint = dotProducts[0] <= dotProducts[1] ? 0 : 1;
            
            float penetrationDistance = Vector3.Dot((refPoints[deepestPoint].position - hitInfo.point), hitInfo.normal.normalized);
            pushDirection = -penetrationDistance * refPointsHolder.transform.up;

            /*Vector3 suspensionPosition = transform.position;
            Vector3 velocity = carRb.GetPointVelocity(suspensionPosition);
            Vector3 direction = -velocity.normalized;
            Vector3 force = direction * (velocity.magnitude * carRb.mass);*/
            
            //carRb.AddForce(force, ForceMode.Impulse);
            
            carRb.transform.position += pushDirection;
        }

        private void PlaceWheel()
        {
            Vector3 newPosition = transform.InverseTransformDirection(wheelTransform.up);
            newPosition *= -currentLength;
            wheelTransform.localPosition = newPosition;
        }

        private void RotateWheel()
        {
            if (locked)
            {
                wheelAngularVelocity = 0f;
            }
            else
            {
                wheelAngularVelocity = wheelVelocity.z / axleInfo.wheelRadius;
            }
            
            wheelMesh.Rotate(wheelAngularVelocity, 0f, 0f, Space.Self);
        }

        private void OnDrawGizmos()
        {
            //Handles.Label(transform.position + Vector3.up * 0.5f, suspensionForce.magnitude.ToString());

            /*print("Handles and Gizmos");
            Gizmos.color = Color.red;
            for (int i = 0; i < 2; i++)
            {
                Gizmos.DrawSphere(refPoints[i].position, 0.05f);
            }
            
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(hitInfo.point, 0.05f);*/
            
            //Handles.Label(transform.position + Vector3.up, pushForce.magnitude.ToString());
            //Handles.Label(transform.position + Vector3.up, lateralFactor.ToString());
            //Handles.Label(transform.position + Vector3.up, wheelVelocity.x.ToString());
        }
    }
}
