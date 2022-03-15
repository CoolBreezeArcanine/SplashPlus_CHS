using System;
using DB;
using MAI2.Util;
using Manager;
using TMPro;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageBookkeep2 : TestModePage
	{
		private enum ItemEnum
		{
			Credit1GameStart,
			Credit2GameStart,
			CreditFreedomStart,
			CreditTicketUse,
			TotalPlayNum,
			AimePlayNum,
			GuestPlayNum,
			Play1PNum,
			Play2PNum,
			PlayFreedomNum,
			NewCardFreePlayNum
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
				item.ValueText.text = totalLog.credits1P.ToString();
				break;
			case 1:
				item.ValueText.text = totalLog.credits2P.ToString();
				break;
			case 2:
				item.ValueText.text = totalLog.creditsFreedom.ToString();
				break;
			case 3:
				item.ValueText.text = totalLog.reserved[0].ToString();
				break;
			case 4:
				item.ValueText.text = totalLog.totalPlayNum.ToString();
				break;
			case 5:
				item.ValueText.text = totalLog.aimeLoginNum.ToString();
				break;
			case 6:
				item.ValueText.text = totalLog.guestLoginNum.ToString();
				break;
			case 7:
				item.ValueText.text = totalLog.play1PNum.ToString();
				break;
			case 8:
				item.ValueText.text = totalLog.play2PNum.ToString();
				break;
			case 9:
				item.ValueText.text = totalLog.playFreedomNum.ToString();
				break;
			case 10:
				item.ValueText.text = totalLog.newCardFreePlayNum.ToString();
				break;
			}
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeBookkeep2ID)Enum.Parse(typeof(TestmodeBookkeep2ID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeBookkeep2ID)Enum.Parse(typeof(TestmodeBookkeep2ID), GetTitleName(index))).GetName();
		}
	}
}
