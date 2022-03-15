using System;
using AMDaemon;
using AMDaemon.Allnet;
using DB;
using MAI2.Util;
using MAI2System;
using Manager;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageDownload : TestModePage
	{
		private enum ItemEnum
		{
			AuthStatus,
			SoftVersion,
			DownloadStatus,
			DownloadVersion,
			DownloadProgress,
			DownloadStartDate,
			DownloadReleaseDate,
			OptionStatus,
			OptionProgress,
			OptionStartDate,
			OptionReleaseDate,
			ServerDate
		}

		protected override void OnUpdateItem(Item item, int index)
		{
			base.OnUpdateItem(item, index);
			switch (index)
			{
			case 0:
			{
				string valueString = (Auth.IsGood ? ConstParameter.TestString_Good : ConstParameter.TestString_Bad);
				item.SetValueString(valueString);
				break;
			}
			case 1:
				item.SetValueString(SingletonStateMachine<AmManager, AmManager.EState>.Instance.VersionNo.versionString);
				break;
			case 3:
				if (NetDelivery.IsExistsApp)
				{
					item.SetValueString(NetDelivery.AppImageInfo.Version.ToString());
				}
				else
				{
					item.SetValueString("-");
				}
				break;
			case 4:
				if (NetDelivery.IsExistsApp)
				{
					NetDeliveryProgress progress = NetDelivery.AppImageInfo.Progress;
					item.SetValueString(progress.Current + "/" + progress.Total + "(" + (int)progress.Percentage + "％)");
				}
				else
				{
					item.SetValueString("----/----(---％)");
				}
				break;
			case 5:
				if (NetDelivery.IsExistsApp)
				{
					item.SetValueString(NetDelivery.AppTimeInfo.Order.ToString("yyyy/MM/dd HH:mm:ss"));
				}
				else
				{
					item.SetValueString("----/--/-- --:--:--");
				}
				break;
			case 6:
				if (NetDelivery.IsExistsApp)
				{
					item.SetValueString(NetDelivery.AppTimeInfo.Release.ToString("yyyy/MM/dd HH:mm:ss"));
				}
				else
				{
					item.SetValueString("----/--/-- --:--:--");
				}
				break;
			case 8:
				if (NetDelivery.IsExistsOption)
				{
					NetDeliveryProgress totalProgress = NetDelivery.OptionImageInfo.TotalProgress;
					item.SetValueString(totalProgress.Current + "/" + totalProgress.Total + "(" + (int)totalProgress.Percentage + "％)");
				}
				else
				{
					item.SetValueString("----/----(---％)");
				}
				break;
			case 9:
				if (NetDelivery.IsExistsOption)
				{
					item.SetValueString(NetDelivery.OptionTimeInfo.Order.ToString("yyyy/MM/dd HH:mm:ss"));
				}
				else
				{
					item.SetValueString("----/--/-- --:--:--");
				}
				break;
			case 10:
				if (NetDelivery.IsExistsOption)
				{
					item.SetValueString(NetDelivery.OptionTimeInfo.Release.ToString("yyyy/MM/dd HH:mm:ss"));
				}
				else
				{
					item.SetValueString("----/--/-- --:--:--");
				}
				break;
			case 11:
				item.SetValueString(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
				break;
			case 2:
			case 7:
				break;
			}
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeDownloadID)Enum.Parse(typeof(TestmodeDownloadID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeDownloadID)Enum.Parse(typeof(TestmodeDownloadID), GetTitleName(index))).GetName();
		}
	}
}
