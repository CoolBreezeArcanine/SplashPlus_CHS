using System;
using DB;
using Manager;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageInputTest : TestModePage
	{
		public enum ItemEnum
		{
			Button1P1,
			Button1P2,
			Button1P3,
			Button1P4,
			Button1P5,
			Button1P6,
			Button1P7,
			Button1P8,
			Select1P,
			Button2P1,
			Button2P2,
			Button2P3,
			Button2P4,
			Button2P5,
			Button2P6,
			Button2P7,
			Button2P8,
			Select2P,
			ServiceButton,
			TestButton
		}

		protected override void OnUpdateItem(Item item, int index)
		{
			base.OnUpdateItem(item, index);
			switch (index)
			{
			case 0:
				item.SetValueOnOff(InputManager.GetButtonPush(0, InputManager.ButtonSetting.Button01));
				break;
			case 9:
				item.SetValueOnOff(InputManager.GetButtonPush(1, InputManager.ButtonSetting.Button01));
				break;
			case 1:
				item.SetValueOnOff(InputManager.GetButtonPush(0, InputManager.ButtonSetting.Button02));
				break;
			case 10:
				item.SetValueOnOff(InputManager.GetButtonPush(1, InputManager.ButtonSetting.Button02));
				break;
			case 2:
				item.SetValueOnOff(InputManager.GetButtonPush(0, InputManager.ButtonSetting.Button03));
				break;
			case 11:
				item.SetValueOnOff(InputManager.GetButtonPush(1, InputManager.ButtonSetting.Button03));
				break;
			case 3:
				item.SetValueOnOff(InputManager.GetButtonPush(0, InputManager.ButtonSetting.Button04));
				break;
			case 12:
				item.SetValueOnOff(InputManager.GetButtonPush(1, InputManager.ButtonSetting.Button04));
				break;
			case 4:
				item.SetValueOnOff(InputManager.GetButtonPush(0, InputManager.ButtonSetting.Button05));
				break;
			case 13:
				item.SetValueOnOff(InputManager.GetButtonPush(1, InputManager.ButtonSetting.Button05));
				break;
			case 5:
				item.SetValueOnOff(InputManager.GetButtonPush(0, InputManager.ButtonSetting.Button06));
				break;
			case 14:
				item.SetValueOnOff(InputManager.GetButtonPush(1, InputManager.ButtonSetting.Button06));
				break;
			case 6:
				item.SetValueOnOff(InputManager.GetButtonPush(0, InputManager.ButtonSetting.Button07));
				break;
			case 15:
				item.SetValueOnOff(InputManager.GetButtonPush(1, InputManager.ButtonSetting.Button07));
				break;
			case 7:
				item.SetValueOnOff(InputManager.GetButtonPush(0, InputManager.ButtonSetting.Button08));
				break;
			case 16:
				item.SetValueOnOff(InputManager.GetButtonPush(1, InputManager.ButtonSetting.Button08));
				break;
			case 8:
				item.SetValueOnOff(InputManager.GetButtonPush(0, InputManager.ButtonSetting.Select));
				break;
			case 17:
				item.SetValueOnOff(InputManager.GetButtonPush(1, InputManager.ButtonSetting.Select));
				break;
			case 19:
				item.SetValueOnOff(InputManager.GetSystemInputPush(InputManager.SystemButtonSetting.ButtonTest));
				break;
			case 18:
				item.SetValueOnOff(InputManager.GetSystemInputPush(InputManager.SystemButtonSetting.ButtonService));
				break;
			}
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeInputID)Enum.Parse(typeof(TestmodeInputID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeInputID)Enum.Parse(typeof(TestmodeInputID), GetTitleName(index))).GetName();
		}
	}
}
