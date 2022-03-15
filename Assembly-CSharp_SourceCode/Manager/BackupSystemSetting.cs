namespace Manager
{
	public class BackupSystemSetting : IBackup
	{
		public const int BackupVersion = 2;

		private BackupSystemSettingRecord _record = new BackupSystemSettingRecord();

		private bool _isDirty;

		public int touchSens1P
		{
			get
			{
				return _record.touch1P;
			}
			set
			{
				_record.touch1P = value;
				_isDirty = true;
			}
		}

		public int touchSens2P
		{
			get
			{
				return _record.touch2P;
			}
			set
			{
				_record.touch2P = value;
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
				_record.backupVersion = 2;
			}
			return true;
		}

		public void clear()
		{
			_record.clear();
			_isDirty = true;
		}
	}
}
