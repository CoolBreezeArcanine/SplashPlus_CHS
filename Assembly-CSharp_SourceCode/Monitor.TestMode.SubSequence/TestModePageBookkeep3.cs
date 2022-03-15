using System;
using System.Text;
using DB;
using MAI2.Util;
using MAI2System;
using Manager;
using TMPro;
using UnityEngine;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageBookkeep3 : TestModePage
	{
		private enum ItemEnum
		{
			RunningTime,
			TotalPlayTime,
			AveragePlayTime
		}

		private static readonly int DayMax = Mathf.Min(9999, TimeSpan.MaxValue.Days);

		private static readonly TimeSpan DisplayTimeMax = new TimeSpan(DayMax, 23, 59, 59, 999);

		protected override void OnInitializeItem(Item item, int index)
		{
			base.OnInitializeItem(item, index);
			if (item.ValueText != null)
			{
				item.ValueText.alignment = TextAlignmentOptions.TopRight;
			}
		}

		protected override void OnUpdateItem(Item item, int index)
		{
			base.OnUpdateItem(item, index);
			DailyLog totalLog = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.bookkeep.totalLog;
			switch (index)
			{
			case 0:
				item.ValueText.text = formatTime(totalLog.totalRunningTime, noDays: false);
				break;
			case 1:
				item.ValueText.text = formatTime(totalLog.totalPlayTime, noDays: false);
				break;
			case 2:
				item.ValueText.text = formatTime(totalLog.averagePlayTime, noDays: true);
				break;
			}
		}

		private string formatTime(TimeSpan timeSpan, bool noDays)
		{
			StringBuilder stringBuilder = Singleton<SystemConfig>.Instance.getStringBuilder();
			if (noDays)
			{
				stringBuilder.AppendFormat(TestmodeBookkeep3ID.TimeStr.GetName(), timeSpan.Hours + timeSpan.Days * 24, timeSpan.Minutes, timeSpan.Seconds);
			}
			else
			{
				if (timeSpan > DisplayTimeMax)
				{
					timeSpan = DisplayTimeMax;
				}
				stringBuilder.AppendFormat(TestmodeBookkeep3ID.DateTimeStr.GetName(), timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			}
			return stringBuilder.ToString().Replace(" ", "  ");
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeBookkeep3ID)Enum.Parse(typeof(TestmodeBookkeep3ID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeBookkeep3ID)Enum.Parse(typeof(TestmodeBookkeep3ID), GetTitleName(index))).GetName();
		}
	}
}
