using System.Collections.Generic;
using DB;
using IO;
using Manager;
using UnityEngine;

namespace Mecha
{
	public class Bd15070_4IF
	{
		public class InitParam
		{
			public bool Dummy;

			public string ComName;

			public int index;

			public InitParam()
			{
				Dummy = false;
				ComName = "";
				index = 0;
			}
		}

		public static readonly Color32 BodyBrightOutGame = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		public static readonly Color32 BodyBrightInGame = new Color32(128, 128, byte.MaxValue, byte.MaxValue);

		private InitParam _initParam;

		private Bd15070_4Control _control;

		private Jvs.LedPwmFadeData _fadeParam;

		private Jvs.LedPwmFadeParam _fadeTempParam;

		private Jvs.LedFetFadeData _fadeFetParam;

		private Jvs.LedFetFadeParam _fadeTempFetParam;

		private const int FetFadeWaitCount = 2;

		private int FetFadeWaitFrame;

		private Jvs.LedSwitchParam[] _switchParam;

		private bool _gsUpdate;

		public Bd15070_4IF(InitParam param)
		{
			_construct(param);
		}

		public void Terminate()
		{
			_terminate();
		}

		public void PreExecute()
		{
			if (_fadeParam.ExecuteFlag)
			{
				_updateAutoFade();
			}
			if (_fadeFetParam.ExecuteFlag)
			{
				_updateAutoFadeBody();
			}
			else
			{
				_updateSwitch();
			}
			if (_gsUpdate)
			{
				SetColorUpdate();
				_gsUpdate = false;
			}
			_preExecute();
		}

		public bool IsInitialized(out bool success)
		{
			return _isInitialized(out success);
		}

		public bool Reset()
		{
			return _reset();
		}

		public BoardInfo GetBoardInfo()
		{
			return _getBoardInfo();
		}

		public bool IsNeedFirmUpdate()
		{
			return _isNeedFirmUpdate();
		}

		public bool StartFirmUpdate()
		{
			return _startFirmUpdate();
		}

		public bool IsFinishedFirmUpdate(out bool success)
		{
			return _isFinishedFirmUpdate(out success);
		}

		public ErrorID GetErrorId()
		{
			return _getErrorID();
		}

		public void ClearError()
		{
			_clearError();
		}

		public float GetLedAnimeSpeed()
		{
			return _getLedAnimeSpeed();
		}

		public void SetColor(byte ledPos, Color32 color)
		{
			if ((uint)(ledPos - 8) > 2u)
			{
				_setColor(ledPos, color);
				_gsUpdate = true;
				_fadeParam.ExecuteFlag = false;
				_switchParam[ledPos].EndColor = color;
			}
		}

		public void SetColorFet(byte ledPos, byte color)
		{
			if ((uint)(ledPos - 8) <= 2u)
			{
				_fadeFetParam.ExecuteFlag = false;
				_setColorFet(ledPos, color);
			}
		}

		public void SetColorMultiFet(Color32 color, float bodyBrightPercent = 1f)
		{
			_setColorMultiFet(new Color32((byte)((float)(int)color.r * bodyBrightPercent), (byte)((float)(int)color.g * bodyBrightPercent), color.b, color.a));
			_fadeFetParam.ExecuteFlag = false;
		}

		public void SetColorFetAutoFade(List<Jvs.LedFetFadeParam> list)
		{
			_fadeFetParam.ExecuteFlag = true;
			_fadeFetParam.FadeList = list;
			_fadeFetParam.NowTime = 0L;
			_fadeFetParam.NowIndex = 0;
			_fadeTempFetParam = _fadeFetParam.FadeList[_fadeFetParam.NowIndex];
			FetFadeWaitFrame = 2;
			_setColorMultiFet(_fadeFetParam.FadeList[0].StartFadeColor);
		}

		public void SetColorMulti(Color32 color, byte speed = 0)
		{
			for (int i = 0; i < 8; i++)
			{
				_switchParam[i].EndColor = color;
			}
			_setColorMulti(color, speed);
			_gsUpdate = true;
			_fadeParam.ExecuteFlag = false;
		}

		public void SetColorMultiFade(Color32 color, byte speed)
		{
			color.a = byte.MaxValue;
			_setColorMultiFade(color, speed);
			_gsUpdate = true;
			_fadeParam.ExecuteFlag = false;
		}

		public void SetColorMultiAutoFade(List<Jvs.LedPwmFadeParam> list)
		{
			_gsUpdate = true;
			_fadeParam.ExecuteFlag = true;
			_fadeParam.FadeList = list;
			_fadeParam.NowTime = 0L;
			_fadeParam.NowIndex = 0;
			_fadeTempParam = _fadeParam.FadeList[_fadeParam.NowIndex];
			_setColorMulti(_fadeParam.FadeList[0].StartFadeColor, 0);
			_setColorMultiFade(_fadeParam.FadeList[0].EndFadeColor, _getMsec2Byte(_fadeParam.FadeList[0].FadeTime));
		}

		public void SetColorSwitch(byte ledPos, Color32 color)
		{
			_gsUpdate = true;
			_switchParam[ledPos].ExecuteFlag = true;
			_switchParam[ledPos].NowTime = 0L;
			_switchParam[ledPos].StartColor = color;
			_setColor(ledPos, _switchParam[ledPos].StartColor);
		}

		public void ButtonLedReset()
		{
			SetColorMulti(Color.black, 0);
		}

		public void SetColorButton(byte ledPos, Color32 color)
		{
			SetColor(ledPos, color);
		}

		public void SetColorButtonPressed(byte ledPos, Color32 color)
		{
			SetColorSwitch(ledPos, color);
		}

		public void SetColorUpdate()
		{
			_setLedUpdate();
		}

		public void SetLedAllOff()
		{
			_fadeParam.ExecuteFlag = false;
			_fadeFetParam.ExecuteFlag = false;
			_setLedAllOff();
			_gsUpdate = true;
		}

		private void _construct(InitParam param)
		{
			_control = null;
			_initParam = param;
			_control = new Bd15070_4Control(param.ComName, param.index);
			_fadeParam = new Jvs.LedPwmFadeData
			{
				ExecuteFlag = false
			};
			_fadeFetParam = new Jvs.LedFetFadeData
			{
				ExecuteFlag = false
			};
			_switchParam = new Jvs.LedSwitchParam[8];
			for (int i = 0; i < 8; i++)
			{
				_switchParam[i] = new Jvs.LedSwitchParam
				{
					ExecuteFlag = false,
					SwitchTime = 500L
				};
			}
		}

		private void _terminate()
		{
			_control.Terminate();
			_control = null;
		}

		private void _preExecute()
		{
			_control.PreExecute();
		}

		private bool _isInitialized(out bool success)
		{
			bool result;
			if (IsDummy())
			{
				result = true;
				success = true;
			}
			else
			{
				result = _control.IsInitialized(out success);
			}
			return result;
		}

		private bool _reset()
		{
			if (!IsDummy())
			{
				return _control.Reset();
			}
			return true;
		}

		private BoardInfo _getBoardInfo()
		{
			BoardInfo boardInfo = new BoardInfo();
			if (_control.GetBoardSpecInfo(out var outInfo))
			{
				boardInfo.BoardNo = outInfo.BoardNo.Text;
				boardInfo.FirmRev = outInfo.FirmInfo.Revision;
			}
			return boardInfo;
		}

		private bool _isNeedFirmUpdate()
		{
			bool result = false;
			if (!IsDummy() && IsInitialized(out var _))
			{
				result = _control.IsNeedFirmUpdate();
			}
			return result;
		}

		private bool _startFirmUpdate()
		{
			bool result = false;
			if (IsFinishedFirmUpdate(out var _) && IsNeedFirmUpdate())
			{
				result = _control.StartFirmUpdate();
			}
			return result;
		}

		private bool _isFinishedFirmUpdate(out bool success)
		{
			success = false;
			Bd15070_4Control.FirmUpdateResultId resultId;
			bool result = _control.IsFinishedFirmUpdate(out resultId);
			if (Bd15070_4Control.FirmUpdateResultId.Ok == resultId)
			{
				success = true;
			}
			return result;
		}

		private ErrorID _getErrorID()
		{
			if (IsDummy())
			{
				return ErrorID.Invalid;
			}
			return _control.GetErrorId();
		}

		private void _clearError()
		{
			_control.ClearError();
		}

		private float _getLedAnimeSpeed()
		{
			return _control.GetLedAnimeSpeed();
		}

		private void _setColor(byte ledPos, Color32 color)
		{
			_control.SetColor(ledPos, color);
		}

		private void _setColorMulti(Color32 color, byte speed)
		{
			_control.SetColorMulti(color, speed);
		}

		private void _setColorMultiFade(Color32 color, byte speed)
		{
			_control.SetColorMultiFade(color, speed);
		}

		private void _setColorMultiFet(Color32 color)
		{
			_control.SetColorMultiFet(color);
		}

		private void _setLedUpdate()
		{
			_control.SetColorUpdate();
		}

		private void _setColorFet(byte ledPos, byte color)
		{
			_control.SetColorFet(ledPos, color);
		}

		private void _setLedAllOff()
		{
			_control.SetLedAllOff();
		}

		private byte _getMsec2Byte(long msec)
		{
			return (byte)(4095 / (msec / 8));
		}

		private void _updateSwitch()
		{
			for (int i = 0; i < 8; i++)
			{
				if (_switchParam[i].ExecuteFlag)
				{
					_switchParam[i].NowTime += GameManager.GetGameMSecAdd();
					if (_switchParam[i].NowTime >= _switchParam[i].SwitchTime)
					{
						_setColor((byte)i, _switchParam[i].EndColor);
						_gsUpdate = true;
						_switchParam[i].ExecuteFlag = false;
						_switchParam[i].NowTime = 0L;
					}
				}
			}
		}

		private void _updateAutoFade()
		{
			if (!_fadeParam.ExecuteFlag)
			{
				return;
			}
			_fadeParam.NowTime += GameManager.GetGameMSecAdd();
			if (_fadeParam.NowTime >= _fadeTempParam.FadeTime)
			{
				_fadeParam.NowTime = _fadeTempParam.FadeTime - _fadeParam.NowTime;
				if (_fadeParam.NowTime < 0)
				{
					_fadeParam.NowTime = 0L;
				}
				if (_fadeTempParam.NextIndex < _fadeParam.FadeList.Count && _fadeTempParam.NextIndex >= 0)
				{
					_fadeParam.NowIndex = _fadeTempParam.NextIndex;
					_fadeTempParam = _fadeParam.FadeList[_fadeParam.NowIndex];
					_setColorMulti(_fadeTempParam.StartFadeColor, 0);
					_setColorMultiFade(_fadeTempParam.EndFadeColor, _getMsec2Byte(_fadeTempParam.FadeTime));
					_gsUpdate = true;
				}
				else
				{
					_fadeParam.ExecuteFlag = false;
				}
			}
		}

		private void _updateAutoFadeBody()
		{
			if (!_fadeFetParam.ExecuteFlag)
			{
				return;
			}
			_fadeFetParam.NowTime += GameManager.GetGameMSecAdd();
			if (_fadeFetParam.NowTime >= _fadeTempFetParam.FadeTime)
			{
				_fadeFetParam.NowTime = _fadeTempFetParam.FadeTime - _fadeFetParam.NowTime;
				if (_fadeFetParam.NowTime < 0)
				{
					_fadeFetParam.NowTime = 0L;
				}
				if (_fadeTempFetParam.NextIndex < _fadeFetParam.FadeList.Count && _fadeTempFetParam.NextIndex >= 0)
				{
					_fadeFetParam.NowIndex = _fadeTempFetParam.NextIndex;
					_fadeTempFetParam = _fadeFetParam.FadeList[_fadeFetParam.NowIndex];
					_setColorMultiFet(_fadeTempFetParam.StartFadeColor);
					_gsUpdate = true;
					FetFadeWaitFrame = 2;
				}
				else
				{
					_fadeFetParam.ExecuteFlag = false;
					_setColorMultiFet(Color32.Lerp(_fadeTempFetParam.StartFadeColor, _fadeTempFetParam.EndFadeColor, 1f));
				}
			}
			else
			{
				FetFadeWaitFrame--;
				if (FetFadeWaitFrame <= 0)
				{
					float t = (float)_fadeFetParam.NowTime / (float)_fadeTempFetParam.FadeTime;
					_setColorMultiFet(Color32.Lerp(_fadeTempFetParam.StartFadeColor, _fadeTempFetParam.EndFadeColor, t));
					FetFadeWaitFrame = 2;
				}
			}
		}

		private bool IsDummy()
		{
			return _initParam.Dummy;
		}
	}
}
