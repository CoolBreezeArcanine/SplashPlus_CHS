using System;
using DB;
using MAI2.Util;
using Manager;
using TMPro;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageVfd : TestModePage
	{
		protected override void OnCreate()
		{
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.Emoney.RequestCheckDisplay();
			ItemList[0].LabelText.alignment = TextAlignmentOptions.Center;
		}

		protected override void Destroy()
		{
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.Emoney.RequestCancelCheckDisplay();
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeVfdID)Enum.Parse(typeof(TestmodeVfdID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeVfdID)Enum.Parse(typeof(TestmodeVfdID), GetTitleName(index))).GetName();
		}
	}
}
