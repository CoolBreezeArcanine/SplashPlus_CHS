using AMDaemon;
using MAI2;

namespace Manager
{
	public class Backup : StateMachine<Backup, Backup.EState>
	{
		public enum EState
		{
			SetupRecords,
			Ready
		}

		private enum RecordID
		{
			Bookkeep,
			GameSetting,
			SystemSetting,
			ClosingTime,
			LocalParameter,
			End
		}

		private BackupBookkeep _bookkeep = new BackupBookkeep();

		private BackupGameSetting _gameSetting = new BackupGameSetting();

		private BackupSystemSetting _systemSetting = new BackupSystemSetting();

		private BackupClosingTime _closeSetting = new BackupClosingTime();

		private BackupLocalParameter _localParameter = new BackupLocalParameter();

		private IBackup[] _backupRecordInterfaces = new IBackup[5];

		public BackupBookkeep bookkeep => _bookkeep;

		public BackupGameSetting gameSetting => _gameSetting;

		public BackupSystemSetting systemSetting => _systemSetting;

		public BackupClosingTime closeSetting
		{
			get
			{
				return _closeSetting;
			}
			set
			{
				_closeSetting = value;
			}
		}

		public BackupLocalParameter localParameter => _localParameter;

		public void initialize()
		{
			_backupRecordInterfaces[0] = _bookkeep;
			_backupRecordInterfaces[1] = _gameSetting;
			_backupRecordInterfaces[2] = _systemSetting;
			_backupRecordInterfaces[3] = _closeSetting;
			_backupRecordInterfaces[4] = _localParameter;
			setNextState(EState.SetupRecords);
		}

		public void execute()
		{
			updateState(-1f);
		}

		public void terminate()
		{
		}

		private void Enter_SetupRecords()
		{
			AMDaemon.Backup.IsAsync = true;
			BackupDevice device = BackupDevice.File;
			BackupRecord[] array = new BackupRecord[5];
			for (int i = 0; i < 5; i++)
			{
				array[i] = new BackupRecord(_backupRecordInterfaces[i].getRecord(), device);
			}
			AMDaemon.Backup.SetupRecords(array);
		}

		private void Execute_SetupRecords()
		{
			if (!AMDaemon.Backup.IsBusy)
			{
				checkOrInitializeRecord();
				setNextState(EState.Ready);
			}
		}

		private void Execute_Ready()
		{
			_bookkeep.updateDaily();
			_bookkeep.updateCredit();
			for (RecordID recordID = RecordID.Bookkeep; recordID < RecordID.End; recordID++)
			{
				if (_backupRecordInterfaces[(int)recordID].getDirty())
				{
					save(recordID);
				}
			}
		}

		private void checkOrInitializeRecord()
		{
			for (int i = 0; i < 5; i++)
			{
				RecordID recordId = (RecordID)i;
				switch (AMDaemon.Backup.GetRecordStatus(i))
				{
				case BackupRecordStatus.Valid:
					if (!_backupRecordInterfaces[i].verify())
					{
						save(recordId);
					}
					break;
				case BackupRecordStatus.DiffApp:
				case BackupRecordStatus.BrokenData:
					_backupRecordInterfaces[i].clear();
					save(recordId);
					break;
				}
			}
		}

		private void save(RecordID recordId)
		{
			AMDaemon.Backup.SaveRecord((int)recordId);
			_backupRecordInterfaces[(int)recordId].resetDirty();
		}
	}
}
