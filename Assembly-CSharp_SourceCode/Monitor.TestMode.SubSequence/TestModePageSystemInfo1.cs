using System;
using AMDaemon;
using DB;
using IO;
using MAI2.Util;
using Manager;
using TMPro;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageSystemInfo1 : TestModePage
	{
		private enum ItemEnum
		{
			SoftVersion,
			KeychipId,
			BoardId,
			Led1PBoardName,
			Led1PBoardId,
			Led1PBoardFirm,
			Led2PBoardName,
			Led2PBoardId,
			Led2PBoardFirm
		}

		protected override void OnCreate()
		{
			base.OnCreate();
			int testModeOptionPageCount = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Delivery.testModeOptionPageCount;
			SetTitleText(0, TestmodeSystemInfo1ID.Title0.GetName() + $"(1/{testModeOptionPageCount + 1})");
			TestModePageSystemInfo2.ClearCurrentPage();
		}

		protected override void OnInitializeItem(Item item, int index)
		{
			base.OnInitializeItem(item, index);
			item.ValueText.alignment = TextAlignmentOptions.TopRight;
		}

		protected override void OnUpdateItem(Item item, int index)
		{
			base.OnUpdateItem(item, index);
			switch (index)
			{
			case 0:
				item.ValueText.text = AppImage.CurrentVersion.ToString();
				break;
			case 1:
				item.ValueText.text = AMDaemon.System.KeychipId.Value;
				break;
			case 2:
				item.ValueText.text = AMDaemon.System.BoardId.Value;
				break;
			case 4:
				item.ValueText.text = MechaManager.LedIf[0].GetBoardInfo().BoardNo;
				break;
			case 5:
				item.ValueText.text = MechaManager.LedIf[0].GetBoardInfo().FirmRev.ToString("X2");
				break;
			case 7:
				item.ValueText.text = MechaManager.LedIf[1].GetBoardInfo().BoardNo;
				break;
			case 8:
				item.ValueText.text = MechaManager.LedIf[1].GetBoardInfo().FirmRev.ToString("X2");
				break;
			case 3:
			case 6:
				break;
			}
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeSystemInfo1ID)Enum.Parse(typeof(TestmodeSystemInfo1ID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeSystemInfo1ID)Enum.Parse(typeof(TestmodeSystemInfo1ID), GetTitleName(index))).GetName();
		}
	}
}
