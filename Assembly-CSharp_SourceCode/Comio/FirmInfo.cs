namespace Comio
{
	public class FirmInfo
	{
		public bool FirmAppli;

		public byte Revision;

		public byte Major;

		public byte Minor;

		public ushort Sum;

		public void Clear()
		{
			FirmAppli = false;
			Revision = 0;
			Major = 0;
			Minor = 0;
			Sum = 0;
		}

		public bool IsAppliMode()
		{
			return FirmAppli;
		}

		public bool CheckFirmVersion(byte version)
		{
			bool result = true;
			if (version != 0 && version > Revision)
			{
				result = false;
			}
			return result;
		}

		public bool CheckFirmVersionSame(byte version)
		{
			bool result = true;
			if (version != 0 && version != Revision)
			{
				result = false;
			}
			return result;
		}
	}
}
