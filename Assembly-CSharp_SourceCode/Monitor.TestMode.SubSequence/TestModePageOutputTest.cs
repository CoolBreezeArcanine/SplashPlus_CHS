using System;
using DB;
using IO;
using MAI2.Util;
using Manager;
using UnityEngine;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageOutputTest : TestModePage
	{
		private enum MenuItem
		{
			ButtonLed_1P,
			BodyLed_1P,
			CircleLed_1P,
			SideLed_1P,
			BillboardLed_1P,
			QrLed_1P,
			ButtonLed_2P,
			BodyLed_2P,
			CircleLed_2P,
			SideLed_2P,
			BillboardLed_2P,
			QrLed_2P,
			CameraLed,
			CameraLedRec,
			CoinBlocker,
			Max
		}

		public enum LedStatus
		{
			Off,
			Red,
			Green,
			Blue,
			White,
			Sequential,
			Max
		}

		private enum AutoBtnPtn
		{
			AllOff,
			White1,
			White2,
			White3,
			White4,
			White5,
			White6,
			White7,
			White8,
			End
		}

		private readonly LedBlockID[] _menuItem2LedBlockID = new LedBlockID[15]
		{
			LedBlockID.Button1_1P,
			LedBlockID.Body_1P,
			LedBlockID.Circle_1P,
			LedBlockID.Side_1P,
			LedBlockID.Billboard_1P,
			LedBlockID.QrLed_1P,
			LedBlockID.Button1_2P,
			LedBlockID.Body_2P,
			LedBlockID.Circle_2P,
			LedBlockID.Side_2P,
			LedBlockID.Billboard_2P,
			LedBlockID.QrLed_2P,
			LedBlockID.Cam,
			LedBlockID.CamRec,
			LedBlockID.End
		};

		private bool _isCoinBlockerEnable = true;

		private readonly LedStatus[] _ledStatus = new LedStatus[29];

		private AutoBtnPtn[] _buttonLedAutoTestIndex = new AutoBtnPtn[2];

		private long[] _buttonLedAutoTestCounter = new long[2];

		private readonly bool[] _extcButtonAutoTest = new bool[2];

		protected override void OnCreate()
		{
			base.OnCreate();
			SetCoinBlockerEnable(flag: true);
			MechaManager.SetAllCuOff();
			AllButtonOff();
		}

		protected override void OnUpdateItem(Item item, int index)
		{
			switch (index)
			{
			case 14:
				item.SetValueString(GetCoinBlockerOnOff(_isCoinBlockerEnable));
				break;
			case 0:
			case 6:
				item.SetValueString(getRGBLedStatusString(_ledStatus[(int)_menuItem2LedBlockID[index]]));
				break;
			case 1:
			case 2:
			case 3:
			case 5:
			case 7:
			case 8:
			case 9:
			case 11:
			case 12:
			case 13:
				if (index < 14)
				{
					item.SetValueString(getWhiteLedStatusString(_ledStatus[(int)_menuItem2LedBlockID[index]]));
				}
				break;
			default:
				if (index < 14)
				{
					item.SetValueString(getRGBLedStatusString(_ledStatus[(int)_menuItem2LedBlockID[index]]));
				}
				break;
			}
		}

		protected override void OnSelectItem(Item item, int index)
		{
			switch (index)
			{
			case 14:
				ToggleCoinBlocker();
				return;
			case 0:
				ToggleButtonLed(LedBlockID.Button1_1P, 0);
				return;
			case 6:
				ToggleButtonLed(LedBlockID.Button1_2P, 1);
				return;
			case 1:
			case 2:
			case 3:
			case 5:
			case 7:
			case 8:
			case 9:
			case 11:
			case 12:
			case 13:
			{
				LedBlockID ledBlockID = _menuItem2LedBlockID[index];
				if (ledBlockID.IsValid())
				{
					ToggleWhiteLed(ledBlockID);
				}
				return;
			}
			}
			if (index < 14)
			{
				LedBlockID ledBlockID2 = _menuItem2LedBlockID[index];
				if (ledBlockID2.IsValid())
				{
					ToggleRgbLed(ledBlockID2);
				}
			}
		}

		protected override void OnUpdate()
		{
			for (int i = 0; i < 2; i++)
			{
				if (_extcButtonAutoTest[i])
				{
					_buttonLedAutoTestCounter[i] += GameManager.GetGameMSecAdd();
					int num = (int)_buttonLedAutoTestCounter[i] / 500;
					if (num >= 9)
					{
						_buttonLedAutoTestCounter[i] = 0L;
					}
					DispAutoButtonLed(i, (AutoBtnPtn)num);
				}
			}
		}

		protected override void Destroy()
		{
			SetCoinBlockerEnable(flag: true);
			MechaManager.SetAllCuOff();
			AllButtonOff();
		}

		private void ToggleButtonLed(LedBlockID ledBlock, int index)
		{
			LedStatus ledStatus = _ledStatus[ledBlock.GetEnumValue()] + 1;
			if (ledStatus >= LedStatus.Max)
			{
				ledStatus = LedStatus.Off;
			}
			_ledStatus[ledBlock.GetEnumValue()] = ledStatus;
			_extcButtonAutoTest[index] = false;
			if (ledStatus != LedStatus.Sequential)
			{
				MechaManager.LedIf[index].SetColorMulti(getRGBLedStatusColor(ledStatus), 0);
				return;
			}
			_buttonLedAutoTestCounter[index] = 0L;
			_extcButtonAutoTest[index] = true;
		}

		private void ToggleCoinBlocker()
		{
			bool coinBlockerEnable = !_isCoinBlockerEnable;
			SetCoinBlockerEnable(coinBlockerEnable);
		}

		private void ToggleRgbLed(LedBlockID ledBlock)
		{
			LedStatus ledStatus = _ledStatus[ledBlock.GetEnumValue()] + 1;
			if (ledStatus > LedStatus.White)
			{
				ledStatus = LedStatus.Off;
			}
			SetRgbLedStatus(ledBlock, ledStatus);
		}

		private void ToggleWhiteLed(LedBlockID ledBlock)
		{
			LedStatus ledStatus = _ledStatus[ledBlock.GetEnumValue()] + 1;
			if (ledStatus >= LedStatus.Green)
			{
				ledStatus = LedStatus.Off;
			}
			SetWhiteLedStatus(ledBlock, ledStatus);
		}

		private void SetCoinBlockerEnable(bool flag)
		{
			_isCoinBlockerEnable = flag;
			Singleton<OperationManager>.Instance.SetCoinBlockerMode(CoinBlocker.Mode.TestMode, flag);
		}

		public string GetCoinBlockerOnOff(bool flag)
		{
			if (!flag)
			{
				return TestmodeOutputID.BlockerOff.GetName();
			}
			return TestmodeOutputID.BlockerOn.GetName();
		}

		private void AllButtonOff()
		{
			for (int i = 0; i < MechaManager.LedIf.Length; i++)
			{
				MechaManager.LedIf[i].SetColorMulti(Color.black, 0);
				_extcButtonAutoTest[i] = false;
				_buttonLedAutoTestCounter[i] = 0L;
				_buttonLedAutoTestIndex[i] = AutoBtnPtn.End;
			}
		}

		private void SetRgbLedStatus(LedBlockID ledBlock, LedStatus ledStatus)
		{
			_ledStatus[ledBlock.GetEnumValue()] = ledStatus;
			ledBlock.SetColor(getRGBLedStatusColor(ledStatus));
		}

		private void SetWhiteLedStatus(LedBlockID ledBlock, LedStatus ledStatus)
		{
			_ledStatus[ledBlock.GetEnumValue()] = ledStatus;
			ledBlock.SetColor(getWhiteLedStatusColor(ledStatus));
		}

		private string getRGBLedStatusString(LedStatus ledStatus)
		{
			return ledStatus switch
			{
				LedStatus.Red => "ON(RED)", 
				LedStatus.Green => "ON(GREEN)", 
				LedStatus.Blue => "ON(BLUE)", 
				LedStatus.White => "ON(WHITE)", 
				LedStatus.Sequential => "ON(SEQUENTIAL)", 
				_ => "OFF", 
			};
		}

		private string getWhiteLedStatusString(LedStatus ledStatus)
		{
			if (ledStatus == LedStatus.Off || ledStatus != LedStatus.Red)
			{
				return "OFF";
			}
			return "ON";
		}

		private Color getRGBLedStatusColor(LedStatus ledStatus)
		{
			return ledStatus switch
			{
				LedStatus.Red => Color.red, 
				LedStatus.Green => Color.green, 
				LedStatus.Blue => Color.blue, 
				LedStatus.White => Color.white, 
				_ => Color.black, 
			};
		}

		private Color getWhiteLedStatusColor(LedStatus ledStatus)
		{
			if (ledStatus == LedStatus.Off || ledStatus != LedStatus.Red)
			{
				return Color.black;
			}
			return Color.white;
		}

		private void DispAutoButtonLed(int monitor, AutoBtnPtn tempIndex)
		{
			bool flag = false;
			if (_buttonLedAutoTestIndex[monitor] != tempIndex)
			{
				_buttonLedAutoTestIndex[monitor] = tempIndex;
				flag = true;
			}
			if (flag)
			{
				switch (tempIndex)
				{
				case AutoBtnPtn.AllOff:
				case AutoBtnPtn.End:
					MechaManager.LedIf[monitor].SetColorMulti(Color.black, 0);
					break;
				case AutoBtnPtn.White1:
				case AutoBtnPtn.White2:
				case AutoBtnPtn.White3:
				case AutoBtnPtn.White4:
				case AutoBtnPtn.White5:
				case AutoBtnPtn.White6:
				case AutoBtnPtn.White7:
				case AutoBtnPtn.White8:
				{
					int num = (int)(tempIndex - 1);
					MechaManager.LedIf[monitor].SetColor((byte)num, Color.white);
					break;
				}
				}
			}
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeOutputID)Enum.Parse(typeof(TestmodeOutputID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeOutputID)Enum.Parse(typeof(TestmodeOutputID), GetTitleName(index))).GetName();
		}
	}
}
