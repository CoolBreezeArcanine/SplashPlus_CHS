using System;
using DB;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageBackupDataClear : TestModePage
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
				MakeSiblingAndDie(NextPagePrefab);
			}
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeBackupclearID)Enum.Parse(typeof(TestmodeBackupclearID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeBackupclearID)Enum.Parse(typeof(TestmodeBackupclearID), GetTitleName(index))).GetName();
		}
	}
}
