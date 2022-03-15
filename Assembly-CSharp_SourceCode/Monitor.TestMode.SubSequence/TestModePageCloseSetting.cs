using System;
using DB;
using MAI2.Util;
using Manager;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageCloseSetting : TestModePage
	{
		public enum MenuList
		{
			Schedule,
			DayHour,
			DayMin,
			WeekSunHour,
			WeekSunMin,
			WeekMonHour,
			WeekMonMin,
			WeekTueHour,
			WeekTueMin,
			WeekWedHour,
			WeekWedMin,
			WeekThuHour,
			WeekThuMin,
			WeekFriHour,
			WeekFriMin,
			WeekSatHour,
			WeekSatMin,
			Exit,
			Max
		}

		private delegate void Act(ref BackupClosingTime.BackupClosingTimeRecord.ClosingTime cTime);

		private readonly MenuList[] _dayItemList = new MenuList[2]
		{
			MenuList.DayHour,
			MenuList.DayMin
		};

		private readonly MenuList[] _weekItemList = new MenuList[14]
		{
			MenuList.WeekSunHour,
			MenuList.WeekSunMin,
			MenuList.WeekMonHour,
			MenuList.WeekMonMin,
			MenuList.WeekTueHour,
			MenuList.WeekTueMin,
			MenuList.WeekWedHour,
			MenuList.WeekWedMin,
			MenuList.WeekThuHour,
			MenuList.WeekThuMin,
			MenuList.WeekFriHour,
			MenuList.WeekFriMin,
			MenuList.WeekSatHour,
			MenuList.WeekSatMin
		};

		private BackupClosingTime _closingTimeData;

		private const int MinTimeHour = 18;

		private const int MaxTimeHour = 28;

		private const int MinTimeMinutes = 0;

		private const int MaxTimeMinutes = 55;

		private const int AddHourValue = 1;

		private const int AddMinutesValue = 5;

		public void InitData()
		{
			Backup backup = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup;
			if (backup != null)
			{
				_closingTimeData = backup.closeSetting;
				SwitchDispDayWeek(_closingTimeData.everySame);
			}
		}

		protected override void Destroy()
		{
			base.Destroy();
			Backup backup = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup;
			if (backup != null)
			{
				backup.closeSetting = _closingTimeData;
				backup.closeSetting.save();
			}
		}

		protected override void OnCreate()
		{
			base.OnCreate();
			Backup backup = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup;
			if (backup != null)
			{
				_closingTimeData = backup.closeSetting;
			}
			SwitchDispDayWeek(_closingTimeData.everySame);
		}

		protected override void OnUpdateItem(Item item, int index)
		{
			base.OnUpdateItem(item, index);
			Func<BackupClosingTime.BackupClosingTimeRecord.ClosingTime, string> func = delegate(BackupClosingTime.BackupClosingTimeRecord.ClosingTime cTime)
			{
				string result = cTime.hour.ToString("D2") + TestmodeCloseID.Label01.GetName();
				if (cTime.dontClose)
				{
					result = TestmodeCloseID.AllTime.GetName();
				}
				return result;
			};
			Func<BackupClosingTime.BackupClosingTimeRecord.ClosingTime, string> func2 = delegate(BackupClosingTime.BackupClosingTimeRecord.ClosingTime cTime)
			{
				if (cTime.dontClose)
				{
					item.SetState(Item.State.InvisibleTemp);
				}
				else
				{
					item.SetState(Item.State.Selectable);
				}
				return cTime.minutes.ToString("D2") + TestmodeCloseID.Label02.GetName();
			};
			if (_closingTimeData.everySame)
			{
				switch (index)
				{
				case 0:
					item.ValueText.text = (_closingTimeData.everySame ? TestmodeCloseID.Label00_00.GetName() : TestmodeCloseID.Label00_01.GetName());
					break;
				case 1:
					item.SetValueString(func(_closingTimeData.configTimes[7]));
					break;
				case 2:
					item.SetValueString(func2(_closingTimeData.configTimes[7]));
					break;
				}
				return;
			}
			switch (index)
			{
			case 0:
				item.ValueText.text = (_closingTimeData.everySame ? TestmodeCloseID.Label00_00.GetName() : TestmodeCloseID.Label00_01.GetName());
				break;
			case 3:
				item.SetValueString(func(_closingTimeData.configTimes[0]));
				break;
			case 4:
				item.SetValueString(func2(_closingTimeData.configTimes[0]));
				break;
			case 5:
				item.SetValueString(func(_closingTimeData.configTimes[1]));
				break;
			case 6:
				item.SetValueString(func2(_closingTimeData.configTimes[1]));
				break;
			case 7:
				item.SetValueString(func(_closingTimeData.configTimes[2]));
				break;
			case 8:
				item.SetValueString(func2(_closingTimeData.configTimes[2]));
				break;
			case 9:
				item.SetValueString(func(_closingTimeData.configTimes[3]));
				break;
			case 10:
				item.SetValueString(func2(_closingTimeData.configTimes[3]));
				break;
			case 11:
				item.SetValueString(func(_closingTimeData.configTimes[4]));
				break;
			case 12:
				item.SetValueString(func2(_closingTimeData.configTimes[4]));
				break;
			case 13:
				item.SetValueString(func(_closingTimeData.configTimes[5]));
				break;
			case 14:
				item.SetValueString(func2(_closingTimeData.configTimes[5]));
				break;
			case 15:
				item.SetValueString(func(_closingTimeData.configTimes[6]));
				break;
			case 16:
				item.SetValueString(func2(_closingTimeData.configTimes[6]));
				break;
			case 1:
			case 2:
				break;
			}
		}

		protected override void OnSelectItem(Item item, int index)
		{
			Act act = delegate(ref BackupClosingTime.BackupClosingTimeRecord.ClosingTime cTime)
			{
				if (cTime.dontClose)
				{
					cTime.dontClose = false;
				}
				else
				{
					cTime.hour++;
					if (cTime.hour > 28)
					{
						cTime.dontClose = true;
						cTime.hour = 18;
					}
				}
			};
			Act act2 = delegate(ref BackupClosingTime.BackupClosingTimeRecord.ClosingTime cTime)
			{
				cTime.minutes += 5;
				if (cTime.minutes > 55)
				{
					cTime.minutes = 0;
				}
			};
			_ = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup;
			switch (index)
			{
			case 0:
				SwitchDispDayWeek(!_closingTimeData.everySame);
				break;
			case 1:
				act(ref _closingTimeData.configTimes[7]);
				break;
			case 2:
				act2(ref _closingTimeData.configTimes[7]);
				break;
			case 3:
				act(ref _closingTimeData.configTimes[0]);
				break;
			case 4:
				act2(ref _closingTimeData.configTimes[0]);
				break;
			case 5:
				act(ref _closingTimeData.configTimes[1]);
				break;
			case 6:
				act2(ref _closingTimeData.configTimes[1]);
				break;
			case 7:
				act(ref _closingTimeData.configTimes[2]);
				break;
			case 8:
				act2(ref _closingTimeData.configTimes[2]);
				break;
			case 9:
				act(ref _closingTimeData.configTimes[3]);
				break;
			case 10:
				act2(ref _closingTimeData.configTimes[3]);
				break;
			case 11:
				act(ref _closingTimeData.configTimes[4]);
				break;
			case 12:
				act2(ref _closingTimeData.configTimes[4]);
				break;
			case 13:
				act(ref _closingTimeData.configTimes[5]);
				break;
			case 14:
				act2(ref _closingTimeData.configTimes[5]);
				break;
			case 15:
				act(ref _closingTimeData.configTimes[6]);
				break;
			case 16:
				act2(ref _closingTimeData.configTimes[6]);
				break;
			}
		}

		protected void SwitchDispDayWeek(bool dayWeek)
		{
			_closingTimeData.everySame = dayWeek;
			if (_closingTimeData.everySame)
			{
				for (int i = 0; i < ItemList.Count; i++)
				{
					MenuList[] weekItemList = _weekItemList;
					foreach (MenuList menuList in weekItemList)
					{
						if (i == (int)menuList)
						{
							ItemList[i].SetState(Item.State.InvisibleTemp);
							break;
						}
					}
					weekItemList = _dayItemList;
					foreach (MenuList menuList2 in weekItemList)
					{
						if (i == (int)menuList2)
						{
							ItemList[i].SetState(Item.State.Selectable);
							break;
						}
					}
				}
				return;
			}
			for (int k = 0; k < ItemList.Count; k++)
			{
				MenuList[] weekItemList = _dayItemList;
				foreach (MenuList menuList3 in weekItemList)
				{
					if (k == (int)menuList3)
					{
						ItemList[k].SetState(Item.State.InvisibleTemp);
						break;
					}
				}
				weekItemList = _weekItemList;
				foreach (MenuList menuList4 in weekItemList)
				{
					if (k == (int)menuList4)
					{
						ItemList[k].SetState(Item.State.Selectable);
						break;
					}
				}
			}
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeCloseID)Enum.Parse(typeof(TestmodeCloseID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeCloseID)Enum.Parse(typeof(TestmodeCloseID), GetTitleName(index))).GetName();
		}
	}
}
