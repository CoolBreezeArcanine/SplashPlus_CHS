using System;
using DB;
using MAI2.Util;
using MAI2System;
using Manager;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageTouchPanel2PTest : TestModePageTouchPanel1PTest
	{
		protected override void OnCreate()
		{
			base.OnCreate();
			PlayerIndex = 1;
		}

		protected override int GetTouchPanelLevel()
		{
			return SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.systemSetting.touchSens2P;
		}

		protected override void SetTouchPanelLevel(int level)
		{
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.systemSetting.touchSens2P = level;
		}

		protected override ConstParameter.ErrorID GetTouchPanelOpenErrorID()
		{
			return ConstParameter.ErrorID.TouchPanel_Right_OpenError;
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeTouchpanel2pID)Enum.Parse(typeof(TestmodeTouchpanel2pID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeTouchpanel2pID)Enum.Parse(typeof(TestmodeTouchpanel2pID), GetTitleName(index))).GetName();
		}
	}
}
