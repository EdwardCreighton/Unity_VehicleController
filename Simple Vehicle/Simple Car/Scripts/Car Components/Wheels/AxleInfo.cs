using UnityEngine;
using System;

namespace SimpleVehicle.SimpleCar
{
    [Serializable]
    public class AxleInfo
    {
        [Header("Axle")]
        public bool isDrive;
        public bool isSteer;
        public bool lockAvailable;
        public float turnRadius = 8f;
        public float steerSpeed = 5f;
        
        [Header("Suspension")]
        public float springStiffness = 70000f;
        public float damperStiffness = 5000f;
        public float restLength = 1f;
        public float deltaLength = 0.15f;

        [Header("Friction")]
        public float tireStiffness = 1f;
        public float brakeFactor = 0.5f;
        [Space]
        public float maxLateralVelocitySlip = 1f;
        public float maxLongitudinalVelocitySlip = 1f;
        [Space]
        public float maxDriveFactorSlip = 1f;

        [Header("Wheel")]
        public float wheelMass = 15f;
        public float wheelWidth = 0.25f;

        [Header("VFX")]
        public GameObject tireSmokePrefab;
        public float slipFactorThreshold = 0.5f;
        public float velocitySlipThreshold = 5f;
        [Space]
        public GameObject dustSmokePrefab;
        public GameObject dirtSmokePrefab;
        [Space]
        public float surfaceVelocityThreshold = 5f;

        [Header("SFX")]
        public SingleClipSettings asphaltSkid;
        public SingleClipSettings dirtSkid;
        public SingleClipSettings sandSkid;
        [Space]
        public float sfxMinVelocitySlip;
        public float sfxMaxVelocitySlip;
        public float sfxMaxSurfaceVelocity;
        [Space]
        public bool loop = true;
        public float spatialBlend = 1f;
        public float minDistance = 5f;
        public float maxDistance = 25f;
        public float dopplerLevel = 1f;

        [HideInInspector] public float wheelRadius;
        [HideInInspector] public float wheelInertia;
        [HideInInspector] public float track;
    }
}
