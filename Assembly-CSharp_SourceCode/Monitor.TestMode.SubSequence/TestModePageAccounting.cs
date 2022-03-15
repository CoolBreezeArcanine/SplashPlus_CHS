using System;
using AMDaemon;
using AMDaemon.Allnet;
using DB;
using MAI2System;
using TMPro;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageAccounting : TestModePage
	{
		private enum ItemEnum
		{
			AccountInfo,
			AccountStatus,
			PlayLog,
			Log1,
			Log2,
			Log3,
			CloseInfo,
			CloseLog1,
			CloseLog2,
			ErrorDisp,
			AllNetWarrning,
			End
		}

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
			switch (index)
			{
			case 0:
				switch (Accounting.Mode)
				{
				case AccountingMode.A:
					item.SetValueString("A");
					break;
				case AccountingMode.B1:
					item.SetValueString("B1");
					break;
				case AccountingMode.B2:
					item.SetValueString("B2");
					break;
				}
				break;
			case 1:
				if (!Accounting.IsLogFull)
				{
					item.SetValueString(ConstParameter.TestString_Good);
				}
				else
				{
					item.SetValueString(ConstParameter.TestString_Bad);
				}
				break;
			case 3:
				if (!AMDaemon.Error.IsOccurred)
				{
					AccountingPlayCountItem playCountItem3 = Accounting.GetPlayCountItem(AccountingPlayCountMonth.Current);
					if (playCountItem3.IsValid)
					{
						item.SetLabel(playCountItem3.Month.Year + "/" + playCountItem3.Month.Month.ToString("D2"));
						item.SetValueInt(playCountItem3.Count);
					}
					else
					{
						item.SetLabel("----/--");
						item.SetValueString("----");
					}
				}
				else
				{
					item.SetLabel("");
					item.SetValueString("");
				}
				break;
			case 4:
				if (!AMDaemon.Error.IsOccurred)
				{
					AccountingPlayCountItem playCountItem2 = Accounting.GetPlayCountItem(AccountingPlayCountMonth.Last);
					if (playCountItem2.IsValid)
					{
						item.SetLabel(playCountItem2.Month.Year + "/" + playCountItem2.Month.Month.ToString("D2"));
						item.SetValueInt(playCountItem2.Count);
					}
					else
					{
						item.SetLabel("----/--");
						item.SetValueString("----");
					}
				}
				else
				{
					item.SetLabel("");
					item.SetValueString("");
				}
				break;
			case 5:
				if (!AMDaemon.Error.IsOccurred)
				{
					AccountingPlayCountItem playCountItem = Accounting.GetPlayCountItem(AccountingPlayCountMonth.BeforeLast);
					if (playCountItem.IsValid)
					{
						item.SetLabel(playCountItem.Month.Year + "/" + playCountItem.Month.Month.ToString("D2"));
						item.SetValueInt(playCountItem.Count);
					}
					else
					{
						item.SetLabel("----/--");
						item.SetValueString("----");
					}
				}
				else
				{
					item.SetLabel("");
					item.SetValueString("");
				}
				break;
			case 7:
				if (!AMDaemon.Error.IsOccurred)
				{
					DateTime reportTime = Accounting.ReportTime;
					if (reportTime != DateTime.MinValue)
					{
						item.SetValueString(reportTime.ToString("yyyy/MM/dd HH:mm:ss"));
					}
					else
					{
						item.SetValueString(reportTime.ToString("----/--/-- --:--:--"));
					}
				}
				else
				{
					item.SetLabel("");
					item.SetValueString("");
				}
				break;
			case 8:
				if (!AMDaemon.Error.IsOccurred)
				{
					DateTime backgroundReportTime = Accounting.BackgroundReportTime;
					if (backgroundReportTime != DateTime.MinValue)
					{
						item.SetValueString(backgroundReportTime.ToString("yyyy/MM/dd HH:mm:ss"));
					}
					else
					{
						item.SetValueString(backgroundReportTime.ToString("----/--/-- --:--:--"));
					}
				}
				else
				{
					item.SetLabel("");
					item.SetValueString("");
				}
				break;
			case 10:
			{
				string valueString = (Accounting.IsNearFullEnabled ? "ON" : "OFF");
				item.SetValueString(valueString);
				break;
			}
			case 9:
			{
				if (!AMDaemon.Error.IsOccurred)
				{
					item.SetLabel("");
					break;
				}
				string text = AMDaemon.Error.Number.ToString();
				string message = AMDaemon.Error.Message;
				item.SetLabel(text + "  " + message);
				item.LabelText.alignment = TextAlignmentOptions.Top;
				break;
			}
			case 2:
			case 6:
				break;
			}
		}

		protected override void OnSelectItem(Item item, int index)
		{
			base.OnSelectItem(item, index);
			if (index == 10)
			{
				Accounting.SetNearFullEnabled(!Accounting.IsNearFullEnabled);
			}
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeAccountingID)Enum.Parse(typeof(TestmodeAccountingID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeAccountingID)Enum.Parse(typeof(TestmodeAccountingID), GetTitleName(index))).GetName();
		}
	}
}
