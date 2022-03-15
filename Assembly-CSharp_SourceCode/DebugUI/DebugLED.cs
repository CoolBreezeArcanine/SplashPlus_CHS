using AMDaemon;
using UnityEngine;

namespace DebugUI
{
	public static class DebugLED
	{
		private static DebugLEDCheck _instance;

		public static void SetInstance(DebugLEDCheck instance)
		{
			_instance = instance;
		}

		public static void SetAimeReaderLED(AimeLedStatus status)
		{
			Color color = Color.black;
			float offTimer = 3f;
			switch (status)
			{
			case AimeLedStatus.None:
				color = Color.black;
				offTimer = 0f;
				break;
			case AimeLedStatus.Scanning:
				color = Color.white;
				offTimer = 0f;
				break;
			case AimeLedStatus.Warning:
				color = Color.yellow;
				break;
			case AimeLedStatus.Error:
				color = Color.red;
				break;
			case AimeLedStatus.Success:
				color = Color.blue;
				break;
			}
			_instance?.SetAimeReaderLed(color, offTimer);
		}

		public static void SetAimeReaderLED(bool red, bool green, bool blue)
		{
			Color black = Color.black;
			if (red)
			{
				black += Color.red;
			}
			if (green)
			{
				black += Color.green;
			}
			if (blue)
			{
				black += Color.blue;
			}
			_instance?.SetAimeReaderLed(black, 0f);
		}

		public static void SetColorButton(int index, byte ledPos, Color32 color)
		{
			_instance?.SetColorButton(index, ledPos, color);
		}

		public static void SetColorMulti(int index, Color32 color)
		{
			_instance?.SetColorMulti(index, color);
		}

		public static void SetColorMultiFade(int index, Color32 color, byte speed)
		{
			_instance?.SetColorMultiFade(index, color, 4095f / (float)(int)speed * 8f);
		}
	}
}
