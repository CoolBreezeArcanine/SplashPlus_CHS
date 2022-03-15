using DB;

namespace Manager
{
	public class BackupGameSetting : IBackup
	{
		public const int BackupVersion = 2;

		private readonly BackupGameSettingRecord _record = new BackupGameSettingRecord();

		private bool _isDirty;

		public bool IsStandardSettingMachine
		{
			get
			{
				return _record.isStandardSettingMachine;
			}
			set
			{
				_record.isStandardSettingMachine = value;
				_isDirty = true;
			}
		}

		public MachineGroupID MachineGroupID
		{
			get
			{
				return (MachineGroupID)_record.machineGroupID;
			}
			set
			{
				_record.machineGroupID = value.GetEnumValue();
				_isDirty = true;
			}
		}

		public bool IsEventMode
		{
			get
			{
				return _record.isEventMode;
			}
			set
			{
				_record.isEventMode = value;
				_isDirty = true;
			}
		}

		public EventModeMusicCountID EventModeTrack
		{
			get
			{
				return (EventModeMusicCountID)_record.eventModeTrack;
			}
			set
			{
				_record.eventModeTrack = value.GetEnumValue();
				_isDirty = true;
			}
		}

		public bool IsContinue
		{
			get
			{
				return _record.isContinue;
			}
			set
			{
				_record.isContinue = value;
				_isDirty = true;
			}
		}

		public AdvertiseVolumeID AdvVol
		{
			get
			{
				return (AdvertiseVolumeID)_record.advVol;
			}
			set
			{
				_record.advVol = value.GetEnumValue();
				_isDirty = true;
			}
		}

		public bool IsDressCode
		{
			get
			{
				return _record.isDressCode;
			}
			set
			{
				_record.isDressCode = value;
				_isDirty = true;
			}
		}

		public object getRecord()
		{
			return _record;
		}

		public bool getDirty()
		{
			return _isDirty;
		}

		public void resetDirty()
		{
			_isDirty = false;
		}

		public bool verify()
		{
			if (2 != _record.backupVersion)
			{
				_record.isDressCode = false;
				_record.bodyBrightness = 0;
				_record.backupVersion = 2;
			}
			return true;
		}

		public void clear()
		{
			_record.Clear();
			_isDirty = true;
		}
	}
}
