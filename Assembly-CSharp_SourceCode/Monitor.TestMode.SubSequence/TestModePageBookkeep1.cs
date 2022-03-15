using System;
using AMDaemon;
using DB;
using MAI2.Util;
using Manager;
using TMPro;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageBookkeep1 : TestModePage
	{
		private enum ItemEnum
		{
			Coin1,
			Coin2,
			TotalCoin,
			EMoneyCoin,
			CoinCredit,
			EMoneyCredit,
			ServiceCredit,
			TotalCredit
		}

		protected override void OnInitializeItem(Item item, int index)
		{
			base.OnInitializeItem(item, index);
			item.ValueText.alignment = TextAlignmentOptions.TopRight;
		}

		protected override void OnUpdateItem(Item item, int index)
		{
			base.OnUpdateItem(item, index);
			DailyLog totalLog = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.bookkeep.totalLog;
			switch (index)
			{
			case 0:
				item.ValueText.text = AMDaemon.Credit.Bookkeeping.Coins[0].ToString();
				break;
			case 1:
				item.ValueText.text = AMDaemon.Credit.Bookkeeping.Coins[1].ToString();
				break;
			case 3:
				item.ValueText.text = AMDaemon.Credit.Bookkeeping.EMoneyCoin.ToString();
				break;
			case 2:
				item.ValueText.text = AMDaemon.Credit.Bookkeeping.TotalCoin.ToString();
				break;
			case 4:
				item.ValueText.text = totalLog.coinCredit.ToString();
				break;
			case 5:
				item.ValueText.text = totalLog.emoneyCredit.ToString();
				break;
			case 6:
				item.ValueText.text = totalLog.serviceCredit.ToString();
				break;
			case 7:
				item.ValueText.text = totalLog.totalCredit.ToString();
				break;
			}
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeBookkeep1ID)Enum.Parse(typeof(TestmodeBookkeep1ID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeBookkeep1ID)Enum.Parse(typeof(TestmodeBookkeep1ID), GetTitleName(index))).GetName();
		}
	}
}
