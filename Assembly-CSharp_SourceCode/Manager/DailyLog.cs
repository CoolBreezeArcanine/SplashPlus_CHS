using System;
using System.Runtime.InteropServices;

namespace Manager
{
	[StructLayout(LayoutKind.Sequential)]
	public class DailyLog
	{
		public enum ReserveKind
		{
			CreditsTicket
		}

		public bool needReport;

		public long dateTimeTicks;

		public uint coinCredit;

		public uint emoneyCredit;

		public uint serviceCredit;

		public uint credits1P;

		public uint credits2P;

		public uint creditsFreedom;

		public TimeSpan totalRunningTime;

		public TimeSpan totalPlayTime;

		public TimeSpan forAveragePlayTime;

		public uint aimeLoginNum;

		public uint guestLoginNum;

		public uint play1PNum;

		public uint play2PNum;

		public uint playFreedomNum;

		public uint newCardFreePlayNum;

		public uint tutorialNum;

		private const int ReservedSize = 64;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public uint[] reserved = new uint[64];

		public DateTime dateTime
		{
			get
			{
				return new DateTime(dateTimeTicks);
			}
			set
			{
				dateTimeTicks = value.Ticks;
			}
		}

		public uint totalCredit
		{
			get
			{
				ulong val = (ulong)((long)coinCredit + (long)emoneyCredit + serviceCredit);
				val = Math.Min(4294967295uL, val);
				return (uint)val;
			}
		}

		public TimeSpan averagePlayTime
		{
			get
			{
				long ticks = 0L;
				if (totalPlayNum != 0)
				{
					ticks = forAveragePlayTime.Ticks / (long)totalPlayNum;
				}
				return new TimeSpan(ticks);
			}
		}

		public uint totalLoginNum
		{
			get
			{
				ulong val = (ulong)aimeLoginNum + (ulong)guestLoginNum;
				val = Math.Min(4294967295uL, val);
				return (uint)val;
			}
		}

		public uint playRatio
		{
			get
			{
				if (totalRunningTime.Ticks == 0L)
				{
					return 0u;
				}
				return (uint)((ulong)(totalPlayTime.Ticks * 10000) / (ulong)totalRunningTime.Ticks);
			}
		}

		public uint totalPlayNum
		{
			get
			{
				ulong val = (ulong)aimeLoginNum + (ulong)guestLoginNum;
				val = Math.Min(4294967295uL, val);
				return (uint)val;
			}
		}

		public static uint safeAdd(uint a, uint b)
		{
			if ((uint)(-1 - (int)a) < b)
			{
				return uint.MaxValue;
			}
			return a + b;
		}

		public static uint safeMul(uint a, uint b)
		{
			ulong val = (ulong)a * (ulong)b;
			val = Math.Min(4294967295uL, val);
			return (uint)val;
		}

		public static TimeSpan safeTimeSpanAdd(TimeSpan a, TimeSpan b)
		{
			if (TimeSpan.MaxValue - a < b)
			{
				return TimeSpan.MaxValue;
			}
			return a + b;
		}

		public void addCoinCredit(uint add)
		{
			coinCredit = safeAdd(coinCredit, add);
		}

		public void addEmoneyCredit(uint add)
		{
			emoneyCredit = safeAdd(emoneyCredit, add);
		}

		public void addServiceCredit(uint add)
		{
			serviceCredit = safeAdd(serviceCredit, add);
		}

		public void addCredit1P(uint add)
		{
			credits1P = safeAdd(credits1P, add);
		}

		public void addCredit2P(uint add)
		{
			credits2P = safeAdd(credits2P, add);
		}

		public void addCreditFreedom(uint add)
		{
			creditsFreedom = safeAdd(creditsFreedom, add);
		}

		public void addCreditTicket(uint add)
		{
			reserved[0] = safeAdd(reserved[0], add);
		}

		public void addTotalRunningTime(TimeSpan add)
		{
			totalRunningTime = safeTimeSpanAdd(totalRunningTime, add);
		}

		public void addAimeLoginNum(uint add)
		{
			aimeLoginNum = safeAdd(aimeLoginNum, add);
		}

		public void addGuestLoginNum(uint add)
		{
			guestLoginNum = safeAdd(guestLoginNum, add);
		}

		public void addPlay1PNum(uint add)
		{
			play1PNum = safeAdd(play1PNum, add);
		}

		public void addPlay2PNum(uint add)
		{
			play2PNum = safeAdd(play2PNum, add);
		}

		public void addFreedomNum(uint add)
		{
			playFreedomNum = safeAdd(playFreedomNum, add);
		}

		public void addNewCardFreePlayNum(uint add)
		{
			newCardFreePlayNum = safeAdd(newCardFreePlayNum, add);
		}

		public void addTutorialNum(uint add)
		{
			tutorialNum = safeAdd(tutorialNum, add);
		}

		public void addTotalPlayTime(TimeSpan add)
		{
			totalPlayTime = safeTimeSpanAdd(totalPlayTime, add);
		}

		public void addForAveragePlayTime(TimeSpan add)
		{
			forAveragePlayTime = safeTimeSpanAdd(forAveragePlayTime, add);
		}

		public DailyLog()
		{
			clear();
		}

		public void clear()
		{
			needReport = false;
			dateTimeTicks = DateTime.MinValue.Ticks;
			coinCredit = 0u;
			emoneyCredit = 0u;
			serviceCredit = 0u;
			credits1P = 0u;
			credits2P = 0u;
			creditsFreedom = 0u;
			totalRunningTime = TimeSpan.Zero;
			totalPlayTime = TimeSpan.Zero;
			forAveragePlayTime = TimeSpan.Zero;
			aimeLoginNum = 0u;
			guestLoginNum = 0u;
			play1PNum = 0u;
			play2PNum = 0u;
			playFreedomNum = 0u;
			newCardFreePlayNum = 0u;
			tutorialNum = 0u;
			for (int i = 0; i < reserved.Length; i++)
			{
				reserved[i] = 0u;
			}
		}

		public void copyFrom(DailyLog src)
		{
			needReport = src.needReport;
			dateTimeTicks = src.dateTimeTicks;
			coinCredit = src.coinCredit;
			emoneyCredit = src.emoneyCredit;
			serviceCredit = src.serviceCredit;
			credits1P = src.credits1P;
			credits2P = src.credits2P;
			creditsFreedom = src.creditsFreedom;
			totalRunningTime = src.totalRunningTime;
			forAveragePlayTime = src.forAveragePlayTime;
			totalPlayTime = src.totalPlayTime;
			aimeLoginNum = src.aimeLoginNum;
			guestLoginNum = src.guestLoginNum;
			play1PNum = src.play1PNum;
			play2PNum = src.play2PNum;
			playFreedomNum = src.playFreedomNum;
			newCardFreePlayNum = src.newCardFreePlayNum;
			tutorialNum = src.tutorialNum;
			for (int i = 0; i < reserved.Length; i++)
			{
				reserved[i] = src.reserved[i];
			}
		}
	}
}
