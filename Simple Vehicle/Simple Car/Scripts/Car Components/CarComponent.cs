using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace SimpleVehicle.SimpleCar
{
    public class CarComponent : MonoBehaviour
    {
        #region Fields
        
        [Header("Car Settings")]
        [SerializeField] private float mass = 1650f;
        [SerializeField] private float wheelBase = 2.1f;

        [Space(30f)]
        [SerializeField] private ControlAssist controlAssist;

        [Space(20f, order = 0)]
        [Header("Engine", order = 1)]
        [SerializeField] private Engine engine;
        private Gearbox gearbox;
        public Gearbox Gearbox => gearbox;

        [Space(20f, order = 0)]
        [Header("Wheel Settings", order = 1)]
        [SerializeField] private List<Axle> axles;

        [Space(20f, order = 0)]
        [Header("SFX", order = 1)]
        [SerializeField] private SFXBehaviour sfxBehaviour;
        public SFXBehaviour SFXBehaviour => sfxBehaviour;

        [Space(20f, order = 0)]
        [Header("VFX", order = 1)]
        [SerializeField] private VFXBehaviour vfxBehaviour;

        public float velocityNorm { get; private set; }
        
        public ControlData controlData { get; private set; }
        private InputData playerInput;
        
        public Vector3 velocity { get; private set; }
        public Rigidbody carRb { get; private set; }
        public Collider carCollider { get; private set; }

        #endregion

        #region Car Update Methods

        public void CarAwake()
        {
            InitFields();
            InitColliders();
            InitRigidbody();
            InitWheels();
        }

        public void CarUpdate(InputData playerInput)
        {
            this.playerInput = playerInput;
            UpdateControlData();
            ShiftingGears();
            sfxBehaviour.SFX_Update();
            vfxBehaviour.VFX_Update(this);

            foreach (var axle in axles)
            {
                if (axle.axleInfo.isSteer)
                {
                    axle.Steer(controlData.steer, wheelBase);
                }
            }
        }

        public void CarFixedUpdate()
        {
            UpdateVelocity();
            
            DriveSequence();
            BrakeSequence();
            HandbrakeSequence();
            
            Vector3 averagePush = Vector3.zero;
            
            foreach (var axle in axles)
            {
                axle.AxleSequence();
                
                for (int i = 0; i < 2; i++)
                {
                    averagePush += axle.wheelHolders[i].pushDirection;
                }
                
                axle.AntiRollBar();
            }

            averagePush /= (axles.Count * 2f);
            //transform.position += averagePush;

            bool airControl = true;
            foreach (var axle in axles)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (axle.wheelHolders[i].isGrounded)
                    {
                        airControl = false;
                        break;
                    }
                }
            }

            if (airControl)
            {
                controlAssist.AirControl();
            }
        }

        #endregion
        
        #region Init Methods

        private void InitFields()
        {
            gearbox = new Gearbox();
            controlData = new ControlData();

            sfxBehaviour.SFX_Awake(this);
            
            controlAssist.ControlAssistAwake(this);
        }

        private void InitColliders()
        {
            carCollider = transform.Find("Body Collider").GetComponent<Collider>();
        }

        private void InitRigidbody()
        {
            carRb = gameObject.AddComponent<Rigidbody>();

            carRb.mass = mass;
            carRb.interpolation = RigidbodyInterpolation.Interpolate;
            carRb.collisionDetectionMode = CollisionDetectionMode.Discrete;

            Vector3 newCenterOfMass = transform.Find("Center of Mass").localPosition;
            carRb.centerOfMass = newCenterOfMass;
        }

        private void InitWheels()
        {
            foreach (var axle in axles)
            {
                axle.AxleAwake(this);
            }
        }

        #endregion

        private void UpdateControlData()
        {
            controlData.steer = playerInput.steer;
            controlData.handbrake = playerInput.handbrake > 0f;

            if (playerInput.driveAxis >= 0f)
            {
                switch (gearbox.currentGear)
                {
                    case Gearbox.Gear.FORWARD:
                    {
                        controlData.throttle = playerInput.driveAxis;
                        controlData.brake = 0f;
                        break;
                    }
                    case Gearbox.Gear.REVERSE:
                    {
                        controlData.throttle = 0f;
                        controlData.brake = playerInput.driveAxis;
                        break;
                    }
                    default:
                        print("Not implemented gear behaviour.");
                        break;
                }
            }
            else
            {
                switch (gearbox.currentGear)
                {
                    case Gearbox.Gear.FORWARD:
                    {
                        controlData.throttle = 0f;
                        controlData.brake = Mathf.Abs(playerInput.driveAxis);
                        break;
                    }
                    case Gearbox.Gear.REVERSE:
                    {
                        controlData.throttle = Mathf.Abs(playerInput.driveAxis);
                        controlData.brake = 0f;
                        break;
                    }
                    default:
                        print("Not implemented gear behaviour.");
                        break;
                }
            }
        }

        private void ShiftingGears()
        {
            if (playerInput.driveAxis >= 0f && velocity.z > -5f)
            {
                gearbox.currentGear = Gearbox.Gear.FORWARD;
            }
            else if (playerInput.driveAxis < 0f && velocity.z < 3f)
            {
                gearbox.currentGear = Gearbox.Gear.REVERSE;
            }
        }

        private void DriveSequence()
        {
            foreach (var axle in axles)
            {
                if (!axle.axleInfo.isDrive) continue;

                for (int i = 0; i < 2; i++)
                {
                    axle.wheelHolders[i].driveTorque = engine.GetTorque(Mathf.Abs(velocity.z), controlData.throttle, gearbox.currentGear);
                }
            }
        }

        private void HandbrakeSequence()
        {
            foreach (var axle in axles)
            {
                if (!axle.axleInfo.lockAvailable) continue;

                for (int i = 0; i < 2; i++)
                {
                    axle.wheelHolders[i].locked = controlData.handbrake || (Mathf.Abs(velocity.z) < 2f && controlData.throttle <= 0.01f);
                }
            }
        }

        private void BrakeSequence()
        {
            foreach (var axle in axles)
            {
                for (int i = 0; i < 2; i++)
                {
                    axle.wheelHolders[i].brake = controlData.brake > 0f;
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.impulse.magnitude >= 8000f)
            {
                sfxBehaviour.PlayCrashSound();
            }
        }

        private void UpdateVelocity()
        {
            velocity = transform.InverseTransformDirection(carRb.velocity) * AdvMath.Mps2Kph;

            switch (gearbox.currentGear)
            {
                case Gearbox.Gear.FORWARD:
                {
                    velocityNorm = Mathf.Abs(velocity.z) / engine.MaxForwardSpeed;
                    break;
                }
                case Gearbox.Gear.REVERSE:
                {
                    velocityNorm = Mathf.Abs(velocity.z) / engine.MaxReverseSpeed;
                    break;
                }
                default:
                    print("Not implemented behaviour for " + gearbox.currentGear + ".");
                    break;
            }
        }
    }
}
