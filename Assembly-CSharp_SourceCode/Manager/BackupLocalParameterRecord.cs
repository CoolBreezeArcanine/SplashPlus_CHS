using System.Runtime.InteropServices;

namespace Manager
{
	[StructLayout(LayoutKind.Sequential)]
	public class BackupLocalParameterRecord
	{
		public int playCount;

		private const int ReservedSize = 64;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public uint[] reserved = new uint[64];

		public BackupLocalParameterRecord()
		{
			clear();
		}

		public void clear()
		{
			playCount = 0;
			for (int i = 0; i < reserved.Length; i++)
			{
				reserved[i] = 0u;
			}
		}
	}
}
