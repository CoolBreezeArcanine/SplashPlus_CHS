using System;
using AMDaemon;
using DB;
using MAI2.Util;
using Manager;
using Process;
using UnityEngine.UI;
using Util;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageTop : TestModePage
	{
		private enum ItemEnum
		{
			Bookkeep,
			Input,
			Output,
			MonitorTest,
			TouchPanel,
			Camera,
			GameSetting,
			GameSystem,
			Aime,
			Close,
			Network,
			Emoney,
			Vfd,
			Accounting,
			DeliveryLog,
			BackupClear,
			SystemTest,
			DeliveryServer,
			Debug_SoundTest,
			Debug_IniSet,
			Debug_EventSet,
			Debug_Led,
			Exit
		}

		private TestModeMonitor _parentMonitor;

		private new void Awake()
		{
			base.Awake();
			base.OnCreate();
			OnFinishCallBack = TestModeExit;
			RawImage rawImage = Utility.findChildRecursive<RawImage>(base.transform, "RawImage");
			if (null != rawImage)
			{
				rawImage.texture = TestModeProcess.TestModeRenderTexture;
			}
		}

		protected override void OnSelectItem(Item item, int index)
		{
			if (index == 16)
			{
				Finish(Result.GoToSystemTest);
			}
		}

		protected override void OnInitializeItem(Item item, int index)
		{
			switch (index)
			{
			case 11:
				if (!SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute)
				{
					item.SetState(Item.State.InvisibleTemp);
				}
				break;
			case 14:
				if (!LanInstall.IsServer)
				{
					item.SetState(Item.State.InvisibleTemp);
				}
				break;
			case 18:
			case 19:
			case 21:
				item.SetState(Item.State.InvisibleTemp);
				break;
			case 20:
				item.SetState(Item.State.InvisibleTemp);
				break;
			case 17:
				item.ValueText.text = (LanInstall.IsServer ? TestmodeRootID.Deliver00.GetName() : TestmodeRootID.Deliver01.GetName());
				break;
			case 12:
			case 13:
			case 15:
			case 16:
				break;
			}
		}

		private void TestModeExit(Result result)
		{
			_parentMonitor.IsExit = true;
			_parentMonitor.Result = result;
		}

		public void SetParent(TestModeMonitor parentMonitor)
		{
			_parentMonitor = parentMonitor;
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeRootID)Enum.Parse(typeof(TestmodeRootID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeRootID)Enum.Parse(typeof(TestmodeRootID), GetTitleName(index))).GetName();
		}
	}
}
