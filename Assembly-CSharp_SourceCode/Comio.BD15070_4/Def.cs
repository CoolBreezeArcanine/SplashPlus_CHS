namespace Comio.BD15070_4
{
	public class Def
	{
		public enum LedIndex
		{
			Button1,
			Button2,
			Button3,
			Button4,
			Button5,
			Button6,
			Button7,
			Button8,
			Body,
			Circle,
			Side,
			End
		}

		public const byte NodeId = 17;

		public const byte LedCtrlUnitMax = 11;

		public const ushort LedCtrlIntervalMin = 33;

		public const int BaudRate = 115200;

		public const int SendTimeoutDef = 180;

		public const byte ColorElementMax = 3;
	}
}
