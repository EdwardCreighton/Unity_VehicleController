using UnityEngine;

namespace SimpleVehicle.SimpleCar
{
    public class WheelBase : MonoBehaviour
    {
        #region Fields

        public float driveTorque;
        public bool locked;
        public bool brake;

        public bool isGrounded { get; protected set; }
        protected float wheelAngularVelocity;
        protected Vector3 wheelVelocity;

        protected AxleInfo axleInfo;
        protected RaycastHit hitInfo;

        protected Transform wheelTransform; // for moving up and down
        protected Transform wheelMesh; // for rotating
        protected Rigidbody wheelCast;
        protected Rigidbody carRb;

        protected GameObject refPointsHolder;
        protected Transform[] refPoints;

        private ParticleSystem tireSmoke;
        private ParticleSystem dustSmoke;
        private ParticleSystem dirtSmoke;

        private AudioSource wheelAudio;

        private GroundBehaviour.GroundType currentSurface;

        #endregion

        #region Init

        public void InitWheelBase(CarComponent car, Transform suspensionPoint, AxleInfo axleInfo)
        {
            this.axleInfo = axleInfo;
            locked = this.axleInfo.lockAvailable;

            wheelTransform = suspensionPoint.Find("Wheel Mesh");
            wheelMesh = wheelTransform.GetChild(0);
            
            Transform wheelCastTransform = suspensionPoint.Find("Wheel Cast");
            this.axleInfo.wheelRadius = wheelCastTransform.localScale.y / 2;
            this.axleInfo.wheelInertia = this.axleInfo.wheelMass * this.axleInfo.wheelRadius * this.axleInfo.wheelRadius / 2f;

            InitParticleSystems();

            InitPenetrationPoints();

            Collider wheelCollider = wheelCastTransform.GetComponent<Collider>();
            wheelCast = wheelCastTransform.GetComponent<Rigidbody>();

            carRb = car.carRb;
            Physics.IgnoreCollision(car.carCollider, wheelCollider, true);
            
            InitSound(car);
        }

        private void InitParticleSystems()
        {
            GameObject particleSystem;

            if (axleInfo.tireSmokePrefab != null)
            {
                particleSystem = Instantiate(axleInfo.tireSmokePrefab, Vector3.zero, Quaternion.identity, wheelTransform);
                particleSystem.transform.localPosition = Vector3.down * axleInfo.wheelRadius;
                particleSystem.transform.localEulerAngles = Vector3.zero;

                tireSmoke = particleSystem.GetComponent<ParticleSystem>();
            }

            if (axleInfo.dustSmokePrefab != null)
            {
                particleSystem = Instantiate(axleInfo.dustSmokePrefab, Vector3.zero, Quaternion.identity, wheelTransform);
                particleSystem.transform.localPosition = Vector3.down * axleInfo.wheelRadius;
                particleSystem.transform.localEulerAngles = Vector3.zero;

                dustSmoke = particleSystem.GetComponent<ParticleSystem>();
            }

            if (axleInfo.dirtSmokePrefab != null)
            {
                particleSystem = Instantiate(axleInfo.dirtSmokePrefab, Vector3.zero, Quaternion.identity, wheelTransform);
                particleSystem.transform.localPosition = Vector3.down * axleInfo.wheelRadius;
                particleSystem.transform.localEulerAngles = Vector3.zero;

                dirtSmoke = particleSystem.GetComponent<ParticleSystem>();
            }
        }

        private void InitPenetrationPoints()
        {
            refPointsHolder = new GameObject
            {
                transform =
                {
                    parent = wheelTransform,
                    localPosition = Vector3.zero
                }
            };

            refPoints = new Transform[2];
            for (int i = 0; i < 2; i++)
            {
                Vector3 newLocalPosition = new Vector3(i == 0 ? -axleInfo.wheelWidth / 2f : axleInfo.wheelWidth / 2f, -axleInfo.wheelRadius, 0f);
                
                GameObject refPoint = new GameObject
                {
                    transform =
                    {
                        parent = refPointsHolder.transform,
                        localPosition = newLocalPosition
                    }
                };

                refPoints[i] = refPoint.transform;
            }
        }

        private void InitSound(CarComponent carComponent)
        {
            wheelAudio = gameObject.AddComponent<AudioSource>();

            wheelAudio.loop = axleInfo.loop;
            wheelAudio.spatialBlend = axleInfo.spatialBlend;
            wheelAudio.minDistance = axleInfo.minDistance;
            wheelAudio.maxDistance = axleInfo.maxDistance;
            wheelAudio.dopplerLevel = axleInfo.dopplerLevel;

            wheelAudio.outputAudioMixerGroup = carComponent.SFXBehaviour.MixerGroup;
        }

        #endregion

        protected void SetSurface(GroundBehaviour.GroundType newType)
        {
            if (newType == currentSurface) return;
            
            StopAllSmoke();
            currentSurface = newType;
        }

        #region VFX

        protected void VFX_VelocitySmoke(bool play)
        {
            switch (currentSurface)
            {
                case GroundBehaviour.GroundType.ASPHALT:
                {
                    break;
                }
                case GroundBehaviour.GroundType.DIRT:
                {
                    if (!dirtSmoke) break;
                    
                    PlayStopPS(dirtSmoke, play);

                    //float factor = Mathf.InverseLerp(0f, axleInfo.sfxMaxSurfaceVelocity, wheelVelocity.magnitude);
                    //SetClip(axleInfo.dirtSkid, factor);
                    //AudioOnOff(play);
                    
                    break;
                }
                case GroundBehaviour.GroundType.SAND:
                {
                    if (!dustSmoke) break;
                    
                    PlayStopPS(dustSmoke, play);
                    
                    //float factor = Mathf.InverseLerp(0f, axleInfo.sfxMaxSurfaceVelocity, wheelVelocity.magnitude);
                    //SetClip(axleInfo.sandSkid, factor);
                    //AudioOnOff(play);
                    
                    break;
                }
                default:
                    print("No effect implementation for " + currentSurface + " ground type.");
                    break;
            }
        }

        protected void VFX_SlipSmoke(bool play)
        {
            switch (currentSurface)
            {
                case GroundBehaviour.GroundType.ASPHALT:
                {
                    if (!tireSmoke) break;
                    
                    PlayStopPS(tireSmoke, play);
                    
                    //float factor = locked ? 1f : Mathf.InverseLerp(axleInfo.sfxMinVelocitySlip, axleInfo.sfxMaxVelocitySlip, wheelVelocity.x);
                    //SetClip(axleInfo.asphaltSkid, factor);
                    //AudioOnOff(play);
                    
                    break;
                }
                case GroundBehaviour.GroundType.DIRT:
                {
                    break;
                }
                case GroundBehaviour.GroundType.SAND:
                {
                    break;
                }
                default:
                    print("No effect implementation for " + currentSurface + " ground type.");
                    break;
            }
        }
        
        protected void StopAllSmoke()
        {
            if (tireSmoke) PlayStopPS(tireSmoke, false);
            
            if (dustSmoke) PlayStopPS(dustSmoke, false);
            
            if (dirtSmoke) PlayStopPS(dirtSmoke, false);
        }
        
        private void PlayStopPS(ParticleSystem effect, bool param)
        {
            if (param && !effect.isPlaying)
            {
                effect.Play();
            }
            else if (!param && effect.isPlaying)
            {
                effect.Stop();
            }
        }

        #endregion

        #region SFX

        protected void SFX_Surface(float velocityFactor)
        {
            switch (currentSurface)
            {
                case GroundBehaviour.GroundType.ASPHALT:
                {
                    break;
                }
                case GroundBehaviour.GroundType.DIRT:
                {
                    if (axleInfo.dirtSkid == null) break;
                    SetClip(axleInfo.dirtSkid, velocityFactor);
                    break;
                }
                case GroundBehaviour.GroundType.SAND:
                {
                    if (axleInfo.sandSkid == null) break;
                    SetClip(axleInfo.sandSkid, velocityFactor);
                    break;
                }
                default:
                    print("No sound implementation for " + currentSurface + " ground type.");
                    break;
            }
        }

        protected void SFX_Slip(float slipFactor)
        {
            switch (currentSurface)
            {
                case GroundBehaviour.GroundType.ASPHALT:
                {
                    if (axleInfo.asphaltSkid == null) break;
                    
                    SetClip(axleInfo.asphaltSkid, slipFactor);
                    break;
                }
                case GroundBehaviour.GroundType.DIRT:
                {
                    break;
                }
                case GroundBehaviour.GroundType.SAND:
                {
                    break;
                }
                default:
                    print("No sound implementation for " + currentSurface + " ground type.");
                    break;
            }
        }

        private void SetClip(SingleClipSettings clipSettings, float factor)
        {
            if (clipSettings.clip != wheelAudio.clip)
            {
                wheelAudio.clip = clipSettings.clip;
                wheelAudio.Play();
            }

            if (!wheelAudio.isPlaying)
            {
                wheelAudio.Play();
            }

            wheelAudio.volume = clipSettings.volumeCurve.Evaluate(factor) * clipSettings.volumeLevel;
            wheelAudio.pitch = clipSettings.pitchCurve.Evaluate(factor);

            if (wheelAudio.volume < 0.01f)
            {
                wheelAudio.Stop();
            }
        }

        private void AudioOnOff(bool state)
        {
            if (state && !wheelAudio.isPlaying)
            {
                wheelAudio.Play();
            }
            else if (!state && wheelAudio.isPlaying)
            {
                wheelAudio.Stop();
            }
        }

        #endregion
    }
}
