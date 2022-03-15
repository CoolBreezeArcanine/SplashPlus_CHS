using System;
using DB;
using MAI2.Util;
using Manager;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageCloseConfirm : TestModePage
	{
		public enum MenuList
		{
			Yes,
			No,
			Max
		}

		protected override void OnSelectItem(Item item, int index)
		{
			if (index != 0 || !(Parent != null))
			{
				return;
			}
			TestModePageCloseSetting testModePageCloseSetting = Parent as TestModePageCloseSetting;
			if (testModePageCloseSetting != null)
			{
				Backup backup = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup;
				if (backup != null)
				{
					backup.closeSetting.clear();
					backup.closeSetting.save();
					testModePageCloseSetting.InitData();
					MakeSiblingAndDie(NextPagePrefab);
				}
			}
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeCloseConfirmID)Enum.Parse(typeof(TestmodeCloseConfirmID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeCloseConfirmID)Enum.Parse(typeof(TestmodeCloseConfirmID), GetTitleName(index))).GetName();
		}
	}
}
