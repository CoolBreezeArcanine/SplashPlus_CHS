using System;
using DB;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageMonitorTest : TestModePage
	{
		protected override string GetLabelString(int index)
		{
			return ((TestmodeMonitorID)Enum.Parse(typeof(TestmodeMonitorID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeMonitorID)Enum.Parse(typeof(TestmodeMonitorID), GetTitleName(index))).GetName();
		}
	}
}
