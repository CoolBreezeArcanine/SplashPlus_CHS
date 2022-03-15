using System;

namespace Monitor.TakeOver
{
	public class TakeOverMajorVersion
	{
		public TakeOverMonitor.MajorRomVersion GetMajorRomVersion(uint rom_version)
		{
			TakeOverMonitor.MajorRomVersion result = TakeOverMonitor.MajorRomVersion.NONE;
			_ = Enum.GetNames(typeof(TakeOverMonitor.MajorRomVersion)).Length;
			foreach (object value in Enum.GetValues(typeof(TakeOverMonitor.MajorRomVersion)))
			{
				TakeOverMonitor.MajorRomVersion majorRomVersion = (TakeOverMonitor.MajorRomVersion)value;
				if (rom_version >= (uint)majorRomVersion)
				{
					result = (TakeOverMonitor.MajorRomVersion)value;
				}
			}
			return result;
		}
	}
}
