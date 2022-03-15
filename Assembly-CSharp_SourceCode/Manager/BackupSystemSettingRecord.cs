using System.Runtime.InteropServices;

namespace Manager
{
	[StructLayout(LayoutKind.Sequential)]
	public class BackupSystemSettingRecord
	{
		public int touch1P;

		public int touch2P;

		public int backupVersion;

		public BackupSystemSettingRecord()
		{
			clear();
		}

		public void clear()
		{
			touch1P = 5;
			touch2P = 5;
			backupVersion = 2;
		}
	}
}
