using System;
using AMDaemon;
using DB;
using MAI2.Util;
using Manager;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageBackupDataClearConfirm : TestModePage
	{
		private enum ItemEnum
		{
			Delete,
			Exit
		}

		protected override void OnCreate()
		{
			for (int i = 0; i < base.TitleListCount; i++)
			{
				SetTitleText(i, GetTitleString(i));
			}
		}

		protected override void OnSelectItem(Item item, int index)
		{
			if (index == 0)
			{
				SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.bookkeep.clearTotal();
				AMDaemon.Credit.ClearBackup();
				Sequence.ClearBackup();
				MakeSiblingAndDie(NextPagePrefab);
			}
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeBackupclearConfirmID)Enum.Parse(typeof(TestmodeBackupclearConfirmID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeBackupclearConfirmID)Enum.Parse(typeof(TestmodeBackupclearConfirmID), GetTitleName(index))).GetName();
		}
	}
}
