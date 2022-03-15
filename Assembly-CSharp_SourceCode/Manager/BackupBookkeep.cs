using System;
using System.Collections.Generic;
using System.Diagnostics;
using AMDaemon;

namespace Manager
{
	public class BackupBookkeep : IBackup
	{
		public enum LoginType
		{
			Aime,
			Guest
		}

		public struct EntryState
		{
			public LoginType type;

			public bool entry;
		}

		private BackupBookkeepRecord _record = new BackupBookkeepRecord();

		private bool _isDirty;

		private Stopwatch[] _playTimeStopwatch = new Stopwatch[2]
		{
			new Stopwatch(),
			new Stopwatch()
		};

		public DailyLog totalLog => _record.totalIncome;

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

		public void updateRunningTime()
		{
			updateRunningTime(Sequence.Bookkeeping.TotalTime);
		}

		public void updateCredit()
		{
			updateCoinCredit(AMDaemon.Credit.Bookkeeping.CoinCredit);
			updateEmoenyCredit(AMDaemon.Credit.Bookkeeping.EMoneyCredit);
			updateServiceCredit(AMDaemon.Credit.Bookkeeping.ServiceCredit);
		}

		public void updateDaily()
		{
			bool flag = false;
			DateTime dateTime = BackUpTimeUtil.toLogDateTime(DateTime.Now, 7);
			DailyLog[] dailyIncomes = _record.dailyIncomes;
			if (dateTime != dailyIncomes[0].dateTime)
			{
				flag = true;
			}
			if (flag)
			{
				updateRunningTime();
				for (int num = dailyIncomes.Length - 1; num > 0; num--)
				{
					dailyIncomes[num].copyFrom(dailyIncomes[num - 1]);
				}
				dailyIncomes[0].clear();
				dailyIncomes[0].needReport = true;
				dailyIncomes[0].dateTime = dateTime;
				_isDirty = true;
			}
		}

		public void clearTotal()
		{
			updateRunningTime(Sequence.Bookkeeping.TotalTime);
			_record.clearTotal();
			_isDirty = true;
			AMDaemon.Credit.ClearBackup();
			Sequence.ClearBackup();
		}

		public void startPlayTime(EntryState entry, int index)
		{
			_playTimeStopwatch[index].Reset();
			_playTimeStopwatch[index].Start();
			Sequence.BeginPlay(index);
			_isDirty = true;
		}

		public void endPlayTime(EntryState[] entry, bool[] isNewFreePlay)
		{
			uint num = 0u;
			TimeSpan[] array = new TimeSpan[2];
			for (int i = 0; i < 2; i++)
			{
				_playTimeStopwatch[i].Stop();
				array[i] = _playTimeStopwatch[i].Elapsed;
				if (entry[i].entry)
				{
					switch (entry[i].type)
					{
					case LoginType.Aime:
						_record.addAimeLoginNum(1u);
						break;
					case LoginType.Guest:
						_record.addGuestLoginNum(1u);
						break;
					}
					num++;
					if (isNewFreePlay[i])
					{
						_record.addNewCardFreePlayNum(1u);
					}
					if (GameManager.TutorialPlayed != 0)
					{
						_record.addTutorialNum(1u);
					}
					if (Sequence.IsPlaying(i))
					{
						Sequence.EndPlay(i);
					}
					_record.addForAveragePlayTime(array[i]);
				}
			}
			TimeSpan add = default(TimeSpan);
			if (entry[0].entry && entry[1].entry)
			{
				_record.addPlay2PNum(1u);
				add = ((!(array[0] > array[1])) ? array[1] : array[0]);
			}
			else if (entry[0].entry || entry[1].entry)
			{
				if (GameManager.IsFreedomMode)
				{
					_record.addFreedomNum(1u);
				}
				else
				{
					_record.addPlay1PNum(1u);
				}
				add = (entry[0].entry ? array[0] : array[1]);
			}
			_record.addTotalPlayTime(add);
			_isDirty = true;
		}

		public void addCredit1P(uint add)
		{
			_record.addCredit1P(add);
			_isDirty = true;
		}

		public void addCredit2P(uint add)
		{
			_record.addCredit2P(add);
			_isDirty = true;
		}

		public void addCreditFreedom(uint add)
		{
			_record.addCreditFreedom(add);
			_isDirty = true;
		}

		public void addCreditTicket(uint add)
		{
			_record.addCreditTicket(add);
			_isDirty = true;
		}

		public void addPlay1PNum(uint add)
		{
			_record.addPlay1PNum(add);
			_isDirty = true;
		}

		public void addPlay2PNum(uint add)
		{
			_record.addPlay2PNum(add);
			_isDirty = true;
		}

		public void addFreedomNum(uint add)
		{
			_record.addFreedomNum(add);
			_isDirty = true;
		}

		public void addNewCardFreeNum(uint add)
		{
			_record.addNewCardFreePlayNum(add);
			_isDirty = true;
		}

		public void addTutorialNum(uint add)
		{
			_record.addTutorialNum(add);
			_isDirty = true;
		}

		public List<DailyLog> getDailyLogList(bool needToday, bool needReport)
		{
			return _record.getDailyLogList(needToday, needReport);
		}

		public void setDailyLogReported(DateTime dateTime)
		{
			_record.setDailyLogReported(dateTime);
			_isDirty = true;
		}

		private void updateRunningTime(TimeSpan curValue)
		{
			TimeSpan timeSpan = calcDelta(_record.preRunningTime, curValue);
			if (timeSpan != TimeSpan.Zero)
			{
				_record.addTotalRunningTime(timeSpan);
				_isDirty = true;
			}
			if (_record.preRunningTime != curValue)
			{
				_record.preRunningTime = curValue;
				_isDirty = true;
			}
		}

		private void updateCoinCredit(uint curValue)
		{
			uint num = calcDelta(_record.preCoinCredit, curValue);
			if (num != 0)
			{
				_record.addCoinCredit(num);
				_isDirty = true;
			}
			if (_record.preCoinCredit != curValue)
			{
				_record.preCoinCredit = curValue;
				_isDirty = true;
			}
		}

		private void updateEmoenyCredit(uint curValue)
		{
			uint num = calcDelta(_record.preEmoneyCredit, curValue);
			if (num != 0)
			{
				_record.addEmoneyCredit(num);
				_isDirty = true;
			}
			if (_record.preEmoneyCredit != curValue)
			{
				_record.preEmoneyCredit = curValue;
				_isDirty = true;
			}
		}

		private void updateServiceCredit(uint curValue)
		{
			uint num = calcDelta(_record.preServiceCredit, curValue);
			if (num != 0)
			{
				_record.addServiceCredit(num);
				_isDirty = true;
			}
			if (_record.preServiceCredit != curValue)
			{
				_record.preServiceCredit = curValue;
				_isDirty = true;
			}
		}

		private uint calcDelta(uint preValue, uint curValue)
		{
			uint num = 0u;
			if (curValue >= preValue)
			{
				return curValue - preValue;
			}
			return curValue;
		}

		private TimeSpan calcDelta(TimeSpan preValue, TimeSpan curValue)
		{
			TimeSpan zero = TimeSpan.Zero;
			if (curValue >= preValue)
			{
				return curValue - preValue;
			}
			return curValue;
		}
	}
}
