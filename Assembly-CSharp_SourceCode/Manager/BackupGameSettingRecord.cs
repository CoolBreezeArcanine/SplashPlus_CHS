using System.Runtime.InteropServices;

namespace Manager
{
	[StructLayout(LayoutKind.Sequential)]
	public class BackupGameSettingRecord
	{
		public bool isStandardSettingMachine;

		public int machineGroupID;

		public bool isEventMode;

		public int eventModeTrack;

		public bool isContinue;

		public int advVol;

		public bool isDressCode;

		public int bodyBrightness;

		public int backupVersion;

		public BackupGameSettingRecord()
		{
			Clear();
		}

		public void Clear()
		{
			isStandardSettingMachine = false;
			machineGroupID = 0;
			isEventMode = false;
			eventModeTrack = 0;
			isContinue = false;
			advVol = 4;
			isDressCode = false;
			backupVersion = 2;
		}
	}
}
