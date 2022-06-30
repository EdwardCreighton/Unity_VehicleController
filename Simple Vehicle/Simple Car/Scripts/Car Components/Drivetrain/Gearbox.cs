namespace SimpleVehicle.SimpleCar
{
	public class Gearbox
	{
		public enum Gear
		{
			FORWARD = 1,
			REVERSE = -1
		}

		#region Fields

		public Gear currentGear;

		#endregion

		public Gearbox()
		{
			currentGear = Gear.FORWARD;
		}
	}
}
