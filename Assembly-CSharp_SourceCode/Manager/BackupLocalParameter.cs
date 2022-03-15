namespace Manager
{
	public class BackupLocalParameter : IBackup
	{
		public const int BackupVersion = 1;

		private BackupLocalParameterRecord _record = new BackupLocalParameterRecord();

		private bool _isDirty;

		public int playCount
		{
			get
			{
				return _record.playCount;
			}
			set
			{
				_record.playCount = value;
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
			return true;
		}

		public void clear()
		{
			_isDirty = true;
		}

		public void AddPlayCount()
		{
			_record.playCount++;
			_isDirty = true;
		}
	}
}
