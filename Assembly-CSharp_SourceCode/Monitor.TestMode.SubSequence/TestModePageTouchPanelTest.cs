using System;
using DB;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageTouchPanelTest : TestModePage
	{
		protected override void OnUpdateItem(Item item, int index)
		{
		}

		protected override void OnSelectItem(Item item, int index)
		{
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeTouchpanelID)Enum.Parse(typeof(TestmodeTouchpanelID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeTouchpanelID)Enum.Parse(typeof(TestmodeTouchpanelID), GetTitleName(index))).GetName();
		}
	}
}
