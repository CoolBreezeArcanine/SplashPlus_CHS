using System;
using System.Diagnostics;
using AMDaemon;
using AMDaemon.Allnet;
using DB;
using MAI2.Util;
using MAI2System;
using Manager;
using Util;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageNetwork : TestModePage
	{
		private enum State
		{
			TestAmDaemon,
			TestTitleServer,
			Done
		}

		private enum ItemEnum
		{
			IpAddress,
			Gateway,
			LocalDns,
			HopNum,
			LineType,
			AllnetAuth,
			Aime,
			EMoney,
			TitleServer,
			Max
		}

		private readonly Stopwatch _timer = new Stopwatch();

		private Mode<TestModePageNetwork, State> _mode;

		protected override void OnCreate()
		{
			base.OnCreate();
			_mode = new Mode<TestModePageNetwork, State>(this);
			_timer.Restart();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			_mode.update();
			NetworkTestInfo testInfo = AMDaemon.Network.TestInfo;
			int num = 0;
			string text = "";
			foreach (NetworkTestItem availableItem in testInfo.AvailableItems)
			{
				if (!testInfo.IsBusy(availableItem) && !testInfo.IsDone(availableItem))
				{
					text = "";
				}
				else if (testInfo.IsBusy(availableItem))
				{
					text = (Utility.isBlinkDisp(_timer) ? ConstParameter.TestString_Check : "");
				}
				else if (!testInfo.IsBusy(availableItem) && testInfo.IsDone(availableItem))
				{
					switch (testInfo.GetResult(availableItem))
					{
					case TestResult.Good:
						text = ((availableItem == NetworkTestItem.Hops) ? testInfo.Hops.ToString() : ConstParameter.TestString_Good);
						break;
					case TestResult.NA:
						text = ConstParameter.TestString_NA;
						break;
					case TestResult.Bad:
						text = ConstParameter.TestString_Bad;
						break;
					}
				}
				if (num < ItemList.Count)
				{
					Item item = ItemList[num];
					item.LabelText.text = availableItem.ToText();
					item.ValueText.text = text;
				}
				num++;
			}
			text = ((_mode.get() == 0) ? "" : ((_mode.get() == 1) ? (Utility.isBlinkDisp(_timer) ? ConstParameter.TestString_Check : "") : ((!Auth.IsGood) ? ConstParameter.TestString_NA : ((!Singleton<OperationManager>.Instance.IsAliveServer) ? ConstParameter.TestString_Bad : ConstParameter.TestString_Good))));
			ItemList[8].ValueText.text = text;
		}

		private void TestAmDaemon_Init()
		{
			AMDaemon.Network.StartTest();
		}

		private void TestAmDaemon_Proc()
		{
			if (AMDaemon.Network.TestInfo.IsCompleted)
			{
				_mode.set(Auth.IsGood ? State.TestTitleServer : State.Done);
			}
		}

		private void TestTitleServer_Init()
		{
			Singleton<OperationManager>.Instance.StartTest();
		}

		private void TestTitleServer_Proc()
		{
			if (!Singleton<OperationManager>.Instance.IsBusy())
			{
				_mode.set(State.Done);
			}
		}

		private void Done_Init()
		{
			ChangeOpType(OpType.TestExit);
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeNetworkID)Enum.Parse(typeof(TestmodeNetworkID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeNetworkID)Enum.Parse(typeof(TestmodeNetworkID), GetTitleName(index))).GetName();
		}
	}
}
