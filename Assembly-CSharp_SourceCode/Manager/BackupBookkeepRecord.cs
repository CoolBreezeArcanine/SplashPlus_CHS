using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Manager
{
	[StructLayout(LayoutKind.Sequential)]
	public class BackupBookkeepRecord
	{
		private const int DailyLogNum = 2;

		private const int DailyChangeHour = 7;

		private static DailyLog[] _dailyIncomes;

		private DailyLog _totalIncome = new DailyLog();

		private DailyLog _dailyIncome0 = new DailyLog();

		private DailyLog _dailyIncome1 = new DailyLog();

		private TimeSpan _preRunningTime;

		private uint _preCoinCredit;

		private uint _preEmoneyCredit;

		private uint _preServiceCredit;

		public DailyLog totalIncome => _totalIncome;

		public DailyLog writeDailyIncome => _dailyIncome0;

		public DailyLog[] dailyIncomes
		{
			get
			{
				if (_dailyIncomes == null || _dailyIncomes.Length != 2)
				{
					_dailyIncomes = new DailyLog[2];
				}
				_dailyIncomes[0] = _dailyIncome0;
				_dailyIncomes[1] = _dailyIncome1;
				return _dailyIncomes;
			}
		}

		public TimeSpan preRunningTime
		{
			get
			{
				return _preRunningTime;
			}
			set
			{
				_preRunningTime = value;
			}
		}

		public uint preCoinCredit
		{
			get
			{
				return _preCoinCredit;
			}
			set
			{
				_preCoinCredit = value;
			}
		}

		public uint preEmoneyCredit
		{
			get
			{
				return _preEmoneyCredit;
			}
			set
			{
				_preEmoneyCredit = value;
			}
		}

		public uint preServiceCredit
		{
			get
			{
				return _preServiceCredit;
			}
			set
			{
				_preServiceCredit = value;
			}
		}

		public BackupBookkeepRecord()
		{
			clear();
		}

		public void clear()
		{
			clearTotal();
			for (int i = 0; i < dailyIncomes.Length; i++)
			{
				dailyIncomes[i].clear();
			}
		}

		public void clearTotal()
		{
			totalIncome.clear();
			_preRunningTime = TimeSpan.Zero;
			_preCoinCredit = 0u;
			_preEmoneyCredit = 0u;
			_preServiceCredit = 0u;
		}

		public bool updateDaily()
		{
			bool flag = false;
			DateTime dateTime = BackUpTimeUtil.toLogDateTime(DateTime.Now, 7);
			if (dateTime != dailyIncomes[0].dateTime)
			{
				flag = true;
			}
			if (flag)
			{
				for (uint num = 1u; num != 0; num--)
				{
					dailyIncomes[num].copyFrom(dailyIncomes[num - 1]);
				}
				dailyIncomes[0].clear();
				dailyIncomes[0].needReport = true;
				dailyIncomes[0].dateTime = dateTime;
			}
			return flag;
		}

		public List<DailyLog> getDailyLogList(bool needToday, bool needReport)
		{
			List<DailyLog> list = new List<DailyLog>();
			int num = 0;
			if (!needToday)
			{
				num = 1;
			}
			for (int i = num; i < 2; i++)
			{
				DailyLog dailyLog = dailyIncomes[i];
				ulong num2 = 60uL;
				ulong num3 = (ulong)dailyLog.totalRunningTime.TotalSeconds;
				ulong num4 = (ulong)dailyLog.totalPlayTime.TotalSeconds;
				uint totalPlayNum = dailyLog.totalPlayNum;
				uint totalCredit = dailyLog.totalCredit;
				bool flag = true;
				if (dailyLog.dateTime == DateTime.MinValue)
				{
					flag = false;
				}
				if (totalCredit == 0 && num4 == 0L && totalPlayNum == 0 && num2 > num3)
				{
					flag = false;
				}
				if (!flag)
				{
					continue;
				}
				bool flag2 = true;
				if (needReport && !dailyLog.needReport)
				{
					flag2 = false;
				}
				foreach (DailyLog item in list)
				{
					if (item.dateTime == dailyLog.dateTime)
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					list.Add(dailyLog);
				}
			}
			if (list.Count != 0)
			{
				list.Sort((DailyLog a, DailyLog b) => (a.dateTime.Ticks - b.dateTime.Ticks > 0) ? 1 : ((a.dateTime.Ticks - b.dateTime.Ticks < 0) ? (-1) : 0));
			}
			return list;
		}

		public void setDailyLogReported(DateTime dateTime)
		{
			for (uint num = 0u; num < 2; num++)
			{
				DailyLog dailyLog = dailyIncomes[num];
				if (dateTime == dailyLog.dateTime)
				{
					dailyLog.needReport = false;
				}
			}
		}

		public void addCoinCredit(uint add)
		{
			totalIncome.addCoinCredit(add);
			writeDailyIncome.needReport = true;
			writeDailyIncome.addCoinCredit(add);
		}

		public void addEmoneyCredit(uint add)
		{
			totalIncome.addEmoneyCredit(add);
			writeDailyIncome.needReport = true;
			writeDailyIncome.addEmoneyCredit(add);
		}

		public void addServiceCredit(uint add)
		{
			totalIncome.addServiceCredit(add);
			writeDailyIncome.needReport = true;
			writeDailyIncome.addServiceCredit(add);
		}

		public void addCredit1P(uint add)
		{
			totalIncome.addCredit1P(add);
			writeDailyIncome.needReport = true;
			writeDailyIncome.addCredit1P(add);
		}

		public void addCredit2P(uint add)
		{
			totalIncome.addCredit2P(add);
			writeDailyIncome.needReport = true;
			writeDailyIncome.addCredit2P(add);
		}

		public void addCreditFreedom(uint add)
		{
			totalIncome.addCreditFreedom(add);
			writeDailyIncome.needReport = true;
			writeDailyIncome.addCreditFreedom(add);
		}

		public void addCreditTicket(uint add)
		{
			totalIncome.addCreditTicket(add);
			writeDailyIncome.needReport = true;
			writeDailyIncome.addCreditTicket(add);
		}

		public void addTotalRunningTime(TimeSpan add)
		{
			totalIncome.addTotalRunningTime(add);
			writeDailyIncome.needReport = true;
			writeDailyIncome.addTotalRunningTime(add);
		}

		public void addAimeLoginNum(uint add)
		{
			totalIncome.addAimeLoginNum(add);
			writeDailyIncome.needReport = true;
			writeDailyIncome.addAimeLoginNum(add);
		}

		public void addGuestLoginNum(uint add)
		{
			totalIncome.addGuestLoginNum(add);
			writeDailyIncome.needReport = true;
			writeDailyIncome.addGuestLoginNum(add);
		}

		public void addPlay1PNum(uint add)
		{
			totalIncome.addPlay1PNum(add);
			writeDailyIncome.needReport = true;
			writeDailyIncome.addPlay1PNum(add);
		}

		public void addPlay2PNum(uint add)
		{
			totalIncome.addPlay2PNum(add);
			writeDailyIncome.needReport = true;
			writeDailyIncome.addPlay2PNum(add);
		}

		public void addFreedomNum(uint add)
		{
			totalIncome.addFreedomNum(add);
			writeDailyIncome.needReport = true;
			writeDailyIncome.addFreedomNum(add);
		}

		public void addNewCardFreePlayNum(uint add)
		{
			totalIncome.addNewCardFreePlayNum(add);
			writeDailyIncome.needReport = true;
			writeDailyIncome.addNewCardFreePlayNum(add);
		}

		public void addTutorialNum(uint add)
		{
			totalIncome.addTutorialNum(add);
			writeDailyIncome.needReport = true;
			writeDailyIncome.addTutorialNum(add);
		}

		public void addTotalPlayTime(TimeSpan add)
		{
			totalIncome.addTotalPlayTime(add);
			writeDailyIncome.needReport = true;
			writeDailyIncome.addTotalPlayTime(add);
		}

		public void addForAveragePlayTime(TimeSpan add)
		{
			totalIncome.addForAveragePlayTime(add);
			writeDailyIncome.needReport = true;
			writeDailyIncome.addForAveragePlayTime(add);
		}
	}
}
