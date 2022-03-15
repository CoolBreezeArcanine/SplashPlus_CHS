using System.Collections.Generic;
using AMDaemon;
using DB;
using MAI2.Util;
using Manager;
using UnityEngine;

namespace IO
{
	public class Jvs
	{
		private class JvsSwitch
		{
			private readonly SwitchInput _switchInput;

			private readonly int _playerNo;

			private readonly InputId _inputId;

			private readonly KeyCode _subKey;

			private readonly bool _invert;

			private bool _isStateOn;

			private bool _isTriggerOn;

			private bool _isTriggerOff;

			public bool isStateOn => _isStateOn;

			public bool isTriggerOn => _isTriggerOn;

			public bool isTriggerOff => _isTriggerOff;

			public JvsSwitch(int playerNo, string inputId, KeyCode subKey, bool invert, bool systemButton)
			{
				_playerNo = playerNo;
				_inputId = ((inputId != null) ? new InputId(inputId) : null);
				_subKey = subKey;
				_invert = invert;
				if (_inputId != null)
				{
					_switchInput = ((!systemButton) ? AMDaemon.Input.Players[_playerNo].GetSwitch(_inputId) : AMDaemon.Input.System.GetSwitch(_inputId));
				}
				_isStateOn = false;
				_isTriggerOn = false;
				_isTriggerOff = false;
			}

			public void Execute()
			{
				bool flag = _isStateOn;
				bool flag2 = false;
				flag2 |= DebugInput.GetKey(_subKey);
				flag2 = (_isStateOn = flag2 | ((!_invert) ? (_switchInput.IsOn || _switchInput.HasOnNow) : (!_switchInput.IsOn || _switchInput.HasOffNow)));
				_isTriggerOn = flag2 && (flag2 ^ flag);
				_isTriggerOff = !flag2 && (flag2 ^ flag);
			}
		}

		private class JvsOutput
		{
			private readonly OutputId _outputId;

			private readonly SwitchOutput _switchOutput;

			public JvsOutput(JvsOutputID id)
			{
				_outputId = new OutputId(id.GetOutputIDName());
				_switchOutput = Output.System.GetSwitch(_outputId);
			}

			public void Set(bool on)
			{
				_switchOutput.Set(0, on);
			}
		}

		public struct LedPwmFadeParam
		{
			public Color32 StartFadeColor;

			public Color32 EndFadeColor;

			public long FadeTime;

			public int NextIndex;
		}

		public struct LedPwmFadeData
		{
			public bool ExecuteFlag;

			public long NowTime;

			public int NowIndex;

			public List<LedPwmFadeParam> FadeList;
		}

		public struct LedFetFadeParam
		{
			public Color32 StartFadeColor;

			public Color32 EndFadeColor;

			public long FadeTime;

			public int NextIndex;
		}

		public struct LedFetFadeData
		{
			public bool ExecuteFlag;

			public long NowTime;

			public int NowIndex;

			public List<LedFetFadeParam> FadeList;
		}

		public struct LedSwitchParam
		{
			public bool ExecuteFlag;

			public Color32 StartColor;

			public Color32 EndColor;

			public long SwitchTime;

			public long NowTime;
		}

		private class JvsOutputPwm
		{
			private const byte PwmCommand = 65;

			private const byte PwmDataSize = 8;

			private readonly byte[] _datas = new byte[8];

			private readonly LedPwmFadeData[] _fadeParam = new LedPwmFadeData[2];

			private Color32 _fadeTempColor;

			private LedPwmFadeParam _fadeTempParam;

			private readonly UsbIOUniqueOutput _outputId;

			public JvsOutputPwm()
			{
				if (UsbIO.NodeCount != 0)
				{
					_outputId = UsbIO.Nodes[0].UniqueOutput;
				}
				_datas[0] = 252;
				_datas[1] = 0;
				for (int i = 0; i < _fadeParam.Length; i++)
				{
					_fadeParam[i].ExecuteFlag = false;
				}
			}

			public void Set(byte index, Color32 color, bool update = false)
			{
				if (update)
				{
					_fadeParam[index].ExecuteFlag = false;
				}
				if (index == 0)
				{
					_datas[2] = color.r;
					_datas[4] = color.g;
					_datas[6] = color.b;
				}
				else
				{
					_datas[3] = color.r;
					_datas[5] = color.g;
					_datas[7] = color.b;
				}
				_outputId?.Set(65, _datas);
			}

			public void SetPwmAutoFade(int targetID, List<LedPwmFadeParam> list)
			{
				_fadeParam[targetID].ExecuteFlag = true;
				_fadeParam[targetID].FadeList = list;
				_fadeParam[targetID].NowTime = 0L;
				_fadeParam[targetID].NowIndex = 0;
			}

			public void UpdatePwmFade()
			{
				for (int i = 0; i < _fadeParam.Length; i++)
				{
					if (!_fadeParam[i].ExecuteFlag)
					{
						continue;
					}
					_fadeParam[i].NowTime += GameManager.GetGameMSecAdd();
					_fadeTempParam = _fadeParam[i].FadeList[_fadeParam[i].NowIndex];
					float num = (float)_fadeParam[i].NowTime / (float)_fadeTempParam.FadeTime;
					_fadeTempColor = Color32.Lerp(_fadeTempParam.StartFadeColor, _fadeTempParam.EndFadeColor, num);
					Set((byte)i, _fadeTempColor);
					if (num >= 1f)
					{
						_fadeParam[i].NowTime = _fadeTempParam.FadeTime - _fadeParam[i].NowTime;
						if (_fadeParam[i].NowTime < 0)
						{
							_fadeParam[i].NowTime = 0L;
						}
						if (_fadeTempParam.NextIndex < _fadeParam[i].FadeList.Count && _fadeTempParam.NextIndex >= 0)
						{
							_fadeParam[i].NowIndex = _fadeTempParam.NextIndex;
						}
						else
						{
							_fadeParam[i].ExecuteFlag = false;
						}
					}
				}
			}
		}

		private bool _isReady;

		private readonly JvsSwitch[] _switch = new JvsSwitch[20];

		private readonly JvsOutput[] _output = new JvsOutput[5];

		private readonly JvsOutputPwm _pwmOut = new JvsOutputPwm();

		public void Initialize()
		{
			CreateSwitches();
			_isReady = false;
		}

		public void Terminate()
		{
		}

		public void Execute()
		{
			if (!_isReady)
			{
				if (!SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsReady)
				{
					return;
				}
				_isReady = true;
			}
			if (_isReady)
			{
				_pwmOut.UpdatePwmFade();
				JvsSwitch[] @switch = _switch;
				for (int i = 0; i < @switch.Length; i++)
				{
					@switch[i].Execute();
				}
			}
		}

		public bool GetRawState(JvsButtonID button)
		{
			if (button.IsValid())
			{
				return _switch[button.GetEnumValue()].isStateOn;
			}
			return false;
		}

		public bool GetTriggerOn(JvsButtonID button)
		{
			if (button.IsValid())
			{
				return _switch[button.GetEnumValue()].isTriggerOn;
			}
			return false;
		}

		public bool GetTriggerOff(JvsButtonID button)
		{
			if (button.IsValid())
			{
				return _switch[button.GetEnumValue()].isTriggerOff;
			}
			return false;
		}

		public void SetOutput(JvsOutputID id, bool on)
		{
			if (id.IsValid())
			{
				_output[id.GetEnumValue()].Set(on);
			}
		}

		public void SetPwmOutput(byte index, Color color)
		{
			_pwmOut.Set(index, color, update: true);
		}

		public void SetPwmAutoFade(byte index, List<LedPwmFadeParam> list)
		{
			_pwmOut.SetPwmAutoFade(index, list);
		}

		private void CreateSwitches()
		{
			for (int i = 0; i < 20; i++)
			{
				JvsButtonID self = (JvsButtonID)i;
				_switch[i] = new JvsSwitch(self.GetJvsPlayer(), self.GetInputIDName(), (KeyCode)self.GetSubstituteKey().GetValue(), self.GetInvert() != 0, self.GetSystem() != 0);
			}
			for (int j = 0; j < 5; j++)
			{
				_output[((JvsOutputID)j).GetEnumValue()] = new JvsOutput((JvsOutputID)j);
			}
		}
	}
}
