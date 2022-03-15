using System;
using DB;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageCloseReset : TestModePage
	{
		protected override string GetLabelString(int index)
		{
			return ((TestmodeCloseChangedID)Enum.Parse(typeof(TestmodeCloseChangedID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeCloseChangedID)Enum.Parse(typeof(TestmodeCloseChangedID), GetTitleName(index))).GetName();
		}
	}
}
