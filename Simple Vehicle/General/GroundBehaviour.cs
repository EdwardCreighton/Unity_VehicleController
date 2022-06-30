using UnityEngine;

namespace SimpleVehicle
{
	public class GroundBehaviour : MonoBehaviour
	{
		public enum GroundType
		{
			ASPHALT,
			DIRT,
			SAND
		}

		[SerializeField] private GroundType type;
		public GroundType Type => type;

		public float surfaceFriction { get; private set; }

		private void Awake()
		{
			switch (type)
			{
				case GroundType.ASPHALT:
					surfaceFriction = 1f;
					break;
				case GroundType.DIRT:
					surfaceFriction = 0.8f;
					break;
				case GroundType.SAND:
					surfaceFriction = 0.5f;
					break;
			}
		}
	}
}
