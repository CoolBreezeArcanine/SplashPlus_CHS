using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Manager
{
	public class BackupClosingTime : IBackup
	{
		[StructLayout(LayoutKind.Sequential)]
		public class BackupClosingTimeRecord
		{
			public struct ClosingTime
			{
				public bool dontClose;

				public int hour;

				public int minutes;
			}

			private delegate void Act(ref ClosingTime cTime);

			public bool everySame = true;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public ClosingTime[] configTimes = new ClosingTime[8];

			public BackupClosingTimeRecord()
			{
				clear();
			}

			public void clear()
			{
				everySame = true;
				Act act = delegate(ref ClosingTime cTime)
				{
					cTime.dontClose = false;
					cTime.hour = 24;
					cTime.minutes = 0;
				};
				for (int i = 0; i < configTimes.Length; i++)
				{
					act(ref configTimes[i]);
				}
			}

			public int getRemainingMinutes()
			{
				DateTime now = DateTime.Now;
				DateTime dateTime = now - new TimeSpan(7, 0, 0);
				ClosingTime closingTime = ((!everySame) ? configTimes[(int)dateTime.DayOfWeek] : configTimes[7]);
				int result = 99;
				if (!closingTime.dontClose)
				{
					DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).AddHours(closingTime.hour).AddMinutes(closingTime.minutes);
					TimeSpan timeSpan = dateTime2 - now;
					result = ((!(timeSpan.TotalMinutes <= 0.0)) ? ((!(timeSpan.TotalMinutes > 99.0)) ? Mathf.Min(Mathf.CeilToInt((float)timeSpan.TotalMinutes), 99) : 99) : 0);
				}
				return result;
			}
		}

		public const int DayEndHour = 7;

		private BackupClosingTimeRecord _record = new BackupClosingTimeRecord();

		private bool _isDirty;

		public bool everySame
		{
			get
			{
				return _record.everySame;
			}
			set
			{
				_record.everySame = value;
			}
		}

		public BackupClosingTimeRecord.ClosingTime[] configTimes
		{
			get
			{
				return _record.configTimes;
			}
			set
			{
				_record.configTimes = value;
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
			_record.clear();
			_isDirty = true;
		}

		public void save()
		{
			_isDirty = true;
		}

		public int getRemainingMinutes()
		{
			return _record.getRemainingMinutes();
		}
	}
}
