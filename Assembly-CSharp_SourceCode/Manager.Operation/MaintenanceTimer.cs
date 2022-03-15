using System;
using UnityEngine;

namespace Manager.Operation
{
	public class MaintenanceTimer
	{
		private const int RebootForceLogoutMin = 25;

		private TimeSpan _startTimeSetting;

		private TimeSpan _endTimeSetting;

		private DateTime _startTime;

		private DateTime _endTime;

		private DateTime _rebootTime;

		private DateTime _rebootLogoutTime;

		private int _secServerMaintenance;

		private int _secRebootLogout;

		private bool _afterRebootTimeNow;

		private bool _afterRebootTimeOld;

		private bool _needReboot;

		private int RemainingMinutes => (_secServerMaintenance + 59) / 60;

		public MaintenanceTimer()
		{
			_startTimeSetting = new TimeSpan(4, 0, 0);
			_endTimeSetting = new TimeSpan(7, 0, 0);
			_startTime = DateTime.MinValue;
			_endTime = DateTime.MinValue;
			_rebootTime = DateTime.MinValue;
			_rebootLogoutTime = DateTime.MinValue;
			_secServerMaintenance = int.MaxValue;
			_secRebootLogout = int.MaxValue;
			_afterRebootTimeNow = false;
			_afterRebootTimeOld = false;
			_needReboot = false;
		}

		public void Initialize()
		{
			UpdateTimes();
			_afterRebootTimeNow = DateTime.Now > _rebootTime;
		}

		public void Execute()
		{
			UpdateTimes();
			DateTime now = DateTime.Now;
			_afterRebootTimeOld = _afterRebootTimeNow;
			_afterRebootTimeNow = now > _rebootTime;
			if (_afterRebootTimeNow && !_afterRebootTimeOld)
			{
				_needReboot = true;
			}
			_secRebootLogout = (int)(_rebootLogoutTime - now).TotalSeconds;
			if (_secRebootLogout < 0)
			{
				if (_needReboot)
				{
					_secRebootLogout = 0;
				}
				else
				{
					_secRebootLogout += 86400;
				}
			}
			_secServerMaintenance = (int)(_startTime - now).TotalSeconds;
			if (_secServerMaintenance < 0)
			{
				if (now <= _endTime)
				{
					_secServerMaintenance = 0;
				}
				else
				{
					_secServerMaintenance += 86400;
				}
			}
		}

		public void Terminate()
		{
		}

		public void SetMaintenanceTime(DateTime startTime, DateTime endTime)
		{
			_startTimeSetting = new TimeSpan(startTime.Hour, startTime.Minute, 0);
			_endTimeSetting = new TimeSpan(endTime.Hour, endTime.Minute, 0);
			_afterRebootTimeNow = DateTime.Now > _rebootTime;
		}

		public bool IsUnderServerMaintenance()
		{
			return false;
		}

		public int GetServerMaintenanceSec()
		{
			return _secServerMaintenance;
		}

		public int GetAutoRebootSec()
		{
			return _secRebootLogout;
		}

		public bool IsAutoRebootNeeded()
		{
			return false;
		}

		public int GetRemainingMinutes()
		{
			return Mathf.Max(0, RemainingMinutes - 15);
		}

		public bool IsShowRemainingMinutes()
		{
			return false;
		}

		public bool IsClosed()
		{
			return false;
		}

		public bool IsCoinAcceptable()
		{
			return !IsClosed();
		}

		private void UpdateTimes()
		{
			DateTime today = DateTime.Today;
			DateTime now = DateTime.Now;
			_startTime = today + _startTimeSetting;
			_endTime = today + _endTimeSetting;
			if (_endTime < _startTime)
			{
				_endTime = _endTime.AddDays(1.0);
			}
			if ((_endTime - now).TotalHours > 24.0)
			{
				_startTime = _startTime.AddDays(-1.0);
				_endTime = _endTime.AddDays(-1.0);
			}
			_rebootTime = today + _endTimeSetting;
			if ((DateTime.Now - _rebootTime).TotalHours > 12.0)
			{
				_rebootTime = _rebootTime.AddDays(1.0);
			}
			_rebootLogoutTime = _rebootTime.AddMinutes(25.0);
		}
	}
}
