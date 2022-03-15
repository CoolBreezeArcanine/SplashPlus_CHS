using System;
using System.Collections.Generic;
using System.Linq;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using Manager.MaiStudio;
using Monitor.Common;

namespace Monitor.Entry.Parts.Screens
{
	public class DoneEntry : EntryScreen
	{
		public enum Type
		{
			OnePlayer,
			TwoPlayer,
			Freedom
		}

		public override void Open(params object[] args)
		{
			Type num = (Type)args[0];
			int num2 = (int)args[1];
			Singleton<UserDataManager>.Instance.GetUserData(num2);
			base.Open(args);
			EntryMonitor.OpenPromotion(PromotionType.None);
			switch ((Type)(object)num)
			{
			case Type.OnePlayer:
				PlayVoice(Cue.VO_000007);
				OpenOperationInformation(OperationInformationController.InformationType.OnePlayer);
				break;
			case Type.TwoPlayer:
				PlayVoice(Cue.VO_000007);
				OpenOperationInformation(OperationInformationController.InformationType.TwoPlayer);
				break;
			}
			Delay.StartDelay(2f, delegate
			{
				EntryMonitor.ResponseYes();
			});
		}

		public static bool IsDailyBonus(UserData ud)
		{
			if (ud.IsGuest())
			{
				return false;
			}
			if (ud.IsNetMember <= 1)
			{
				return false;
			}
			string logDateString = TimeManager.GetLogDateString(TimeManager.GetNowUnixTime());
			string timeJp = (string.IsNullOrEmpty(ud.Detail.DailyBonusDate) ? TimeManager.GetLogDateString(0L) : ud.Detail.DailyBonusDate);
			long unixTime = TimeManager.GetUnixTime(logDateString);
			long unixTime2 = TimeManager.GetUnixTime(timeJp);
			return unixTime > unixTime2;
		}

		public static bool IsWeekdayBonus(UserData ud)
		{
			bool flag = false;
			bool flag2 = false;
			if (ud.IsGuest())
			{
				return false;
			}
			if (Singleton<EventManager>.Instance.IsOpenEvent(10))
			{
				flag = true;
				flag2 = false;
				DateTime baseTime = TimeManager.GetDateTime(TimeManager.PlayBaseTime);
				if (!flag2 && (baseTime.DayOfWeek == DayOfWeek.Sunday || baseTime.DayOfWeek == DayOfWeek.Saturday))
				{
					flag = false;
					flag2 = true;
				}
				DateTime dateTime = TimeManager.GetDateTime(Singleton<EventManager>.Instance.GetEventEndUnixTime(10));
				if (!flag2 && (baseTime.Hour <= 7 || baseTime.Hour >= dateTime.Hour))
				{
					flag = false;
					flag2 = true;
				}
				if (!flag2 && Singleton<DataManager>.Instance.GetHolidayes().Any((KeyValuePair<int, HolidayData> t) => baseTime.Year == t.Value.Day.Year && baseTime.Month == t.Value.Day.Month && baseTime.Day == t.Value.Day.Day))
				{
					flag = false;
					flag2 = true;
				}
			}
			else
			{
				flag = false;
			}
			return flag;
		}
	}
}
