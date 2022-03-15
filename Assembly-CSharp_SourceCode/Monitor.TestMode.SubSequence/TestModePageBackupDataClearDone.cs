using System;
using DB;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageBackupDataClearDone : TestModePage
	{
		protected override void OnSelectItem(Item item, int index)
		{
			string text = item.LabelText.text;
			if (text == "消去する")
			{
				MakeSiblingAndDie(NextPagePrefab);
			}
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeBackupclearDoneID)Enum.Parse(typeof(TestmodeBackupclearDoneID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeBackupclearDoneID)Enum.Parse(typeof(TestmodeBackupclearDoneID), GetTitleName(index))).GetName();
		}
	}
}
