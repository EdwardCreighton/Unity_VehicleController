namespace SimpleVehicle.SimpleCar
{
    public class ControlData
    {
        public float steer;
        public float currentSteerAngle;
        public float throttle;
        public float brake;
        public bool handbrake;

        public ControlData()
        {
            steer = throttle = brake = 0f;
            handbrake = false;
        }
    }
}
