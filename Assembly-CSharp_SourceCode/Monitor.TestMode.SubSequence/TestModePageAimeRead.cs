using System;
using System.Diagnostics;
using AMDaemon;
using DB;
using MAI2System;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageAimeRead : TestModePage
	{
		private enum State
		{
			Wait,
			Start,
			Polling,
			Cancelling,
			Finish
		}

		private enum LedState
		{
			Off,
			Red,
			Green,
			Blue,
			White,
			Undefined
		}

		private enum ItemEnum
		{
			HardwareVersion,
			FirmwareVersion,
			CardReaderStatus,
			ReadResult,
			Time,
			LedStatus,
			Start,
			CheckLed,
			Finish
		}

		private AimeUnit _unit;

		private readonly Stopwatch _stopwatch = new Stopwatch();

		private State _state;

		private float _time;

		private bool _isTimeOut;

		private string _hwVersion = string.Empty;

		private string _fwVersion = string.Empty;

		private LedState _ledState;

		protected override void OnCreate()
		{
			if (Aime.UnitCount > 0)
			{
				_unit = Aime.Units[0];
			}
			_time = 0f;
			_state = State.Wait;
			_ledState = LedState.Off;
			_unit.SetLed(onR: false, onG: false, onB: false);
		}

		protected override void OnUpdate()
		{
			if (_unit != null && _unit.Result != null)
			{
				if (string.IsNullOrEmpty(_hwVersion))
				{
					_hwVersion = _unit.Result.HardVersion;
				}
				if (string.IsNullOrEmpty(_fwVersion))
				{
					_fwVersion = _unit.Result.FirmVersion;
				}
			}
			switch (_state)
			{
			case State.Start:
				if (_unit != null)
				{
					_isTimeOut = false;
					if (_unit.Start(AimeCommand.ScanOffline))
					{
						_unit.SetLedStatus(AimeLedStatus.Scanning);
						_ledState = LedState.Undefined;
						_stopwatch.Reset();
						_stopwatch.Start();
						_state = State.Polling;
					}
				}
				break;
			case State.Polling:
				_time = _stopwatch.ElapsedMilliseconds;
				if (!_unit.IsBusy)
				{
					_state = State.Finish;
					if (_unit.HasError)
					{
						_unit.SetLedStatus(AimeLedStatus.Error);
						_ledState = LedState.Undefined;
						_ = _unit.ErrorInfo;
					}
					else
					{
						_unit.SetLedStatus(AimeLedStatus.Success);
						_ledState = LedState.Undefined;
					}
				}
				else if ((float)_stopwatch.ElapsedMilliseconds > 10000f)
				{
					_ledState = LedState.Undefined;
					_unit.Cancel();
					_time = _stopwatch.ElapsedMilliseconds;
					_isTimeOut = true;
					_state = State.Cancelling;
				}
				break;
			case State.Cancelling:
				if (!_unit.IsBusy)
				{
					_unit.SetLedStatus(AimeLedStatus.Error);
					_state = State.Finish;
				}
				break;
			case State.Finish:
				break;
			}
		}

		protected override void OnUpdateItem(Item item, int index)
		{
			base.OnUpdateItem(item, index);
			switch (index)
			{
			case 0:
				item.ValueText.text = GetHardwareVersionText();
				break;
			case 1:
				item.ValueText.text = GetFirmwareVersionText();
				break;
			case 2:
				item.ValueText.text = GetCardReaderStatusText();
				break;
			case 3:
				item.ValueText.text = GetCardReaderResultText();
				break;
			case 4:
				item.ValueText.text = GetCardReaderScanTime();
				break;
			case 5:
				item.ValueText.text = GetCardReaderLedStatusText();
				break;
			case 6:
				item.LabelText.text = GetCardReaderStartLabelText();
				break;
			case 7:
				item.SetState(IsReading() ? Item.State.UnselectableTemp : Item.State.Selectable);
				break;
			}
		}

		protected override void OnSelectItem(Item item, int index)
		{
			base.OnSelectItem(item, index);
			switch (index)
			{
			case 6:
				_unit.Cancel();
				if (IsReading())
				{
					_unit.SetLedStatus(AimeLedStatus.None);
					_ledState = LedState.Undefined;
					_state = State.Wait;
				}
				else
				{
					_state = State.Start;
				}
				break;
			case 7:
				switch (_ledState)
				{
				case LedState.Off:
					_ledState = LedState.Red;
					_unit.SetLed(onR: true, onG: false, onB: false);
					break;
				case LedState.Red:
					_ledState = LedState.Green;
					_unit.SetLed(onR: false, onG: true, onB: false);
					break;
				case LedState.Green:
					_ledState = LedState.Blue;
					_unit.SetLed(onR: false, onG: false, onB: true);
					break;
				case LedState.Blue:
					_ledState = LedState.White;
					_unit.SetLed(onR: true, onG: true, onB: true);
					break;
				case LedState.White:
				case LedState.Undefined:
					_ledState = LedState.Off;
					_unit.SetLed(onR: false, onG: false, onB: false);
					break;
				}
				break;
			}
		}

		protected override void Destroy()
		{
			if (_unit != null)
			{
				_unit.Cancel();
				_unit.SetLed(onR: false, onG: false, onB: false);
			}
		}

		private string GetHardwareVersionText()
		{
			return _hwVersion;
		}

		private string GetFirmwareVersionText()
		{
			return _fwVersion;
		}

		private string GetCardReaderStatusText()
		{
			string result = "";
			switch (_state)
			{
			case State.Wait:
			case State.Start:
				result = TestmodeAimeReadID.Label02_00.GetName();
				break;
			case State.Polling:
				result = TestmodeAimeReadID.Label02_01.GetName();
				break;
			case State.Cancelling:
			case State.Finish:
				result = TestmodeAimeReadID.Label02_02.GetName();
				break;
			}
			return result;
		}

		private string GetCardReaderResultText()
		{
			string result = "";
			State state = _state;
			if (state == State.Finish)
			{
				result = ((!_isTimeOut) ? (_unit.HasError ? ConstParameter.TestString_Bad : ConstParameter.TestString_Good) : ConstParameter.TestString_Bad);
			}
			return result;
		}

		private string GetCardReaderScanTime()
		{
			return $"{_time / 1000f:F1}Sec";
		}

		private string GetCardReaderLedStatusText()
		{
			string result = "";
			switch (_ledState)
			{
			case LedState.Off:
				result = TestmodeAimeReadID.Label05_00.GetName();
				break;
			case LedState.Red:
				result = TestmodeAimeReadID.Label05_01.GetName();
				break;
			case LedState.Green:
				result = TestmodeAimeReadID.Label05_02.GetName();
				break;
			case LedState.Blue:
				result = TestmodeAimeReadID.Label05_03.GetName();
				break;
			case LedState.White:
				result = TestmodeAimeReadID.Label05_04.GetName();
				break;
			case LedState.Undefined:
				result = TestmodeAimeReadID.Label05_05.GetName();
				break;
			}
			return result;
		}

		private string GetCardReaderStartLabelText()
		{
			if (!IsReading())
			{
				return TestmodeAimeReadID.Label06_00.GetName();
			}
			return TestmodeAimeReadID.Label06_01.GetName();
		}

		private bool IsReading()
		{
			State state = _state;
			if (state == State.Wait || state == State.Finish)
			{
				return false;
			}
			return true;
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeAimeReadID)Enum.Parse(typeof(TestmodeAimeReadID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeAimeReadID)Enum.Parse(typeof(TestmodeAimeReadID), GetTitleName(index))).GetName();
		}
	}
}
