using System;
using DB;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageSystemTest : TestModePage
	{
		private enum ItemEnum
		{
			GotoSystemTest,
			Exit
		}

		protected override void OnSelectItem(Item item, int index)
		{
			if (index == 0)
			{
				Finish(Result.GoToSystemTest);
			}
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeSystemtestID)Enum.Parse(typeof(TestmodeSystemtestID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeSystemtestID)Enum.Parse(typeof(TestmodeSystemtestID), GetTitleName(index))).GetName();
		}
	}
}
