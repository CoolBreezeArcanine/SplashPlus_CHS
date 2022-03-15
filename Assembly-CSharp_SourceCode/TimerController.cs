using MAI2.Util;
using MAI2System;
using Manager;
using UnityEngine;

public class TimerController
{
	private const int MONITOR_NUM = 2;

	private const float TIMER_RESISTANCE_VALUE = 0.25f;

	private const float TIMER_ACCELERATION = 50f;

	private const float TIMER_ACCELERATION_SOFT = 12.5f;

	private const float NORMAL_ACCELERATION = 1f;

	private const int LONG_SKIP_SUBSTRACT_VALUE = 30;

	private const int DEFAULT_DECREMENT_TIME_VALUE = 1;

	private int _countDownSecond;

	private int _delayCount;

	private const float OneSecond = 1000f;

	private float _timeCounter;

	private float _timerAcceleration;

	private bool _isTimeCounting;

	private bool _isTimeUp;

	private bool _isCompleted;

	private bool _isEnableRequest;

	private bool _isLongSkip;

	private bool _isFastSkip;

	private bool _isFreedomSync;

	private bool IsFreezeTimer;

	public uint CountDownSecond
	{
		get
		{
			if (!_isTimeUp)
			{
				return (uint)_countDownSecond;
			}
			return 0u;
		}
	}

	public bool IsInfinity { get; private set; }

	public bool IsTimeCounting
	{
		set
		{
			_isTimeCounting = value;
		}
	}

	public bool IsCompleted => _isCompleted;

	public bool IsTimeUp => _isTimeUp;

	public bool IsLongSkip
	{
		set
		{
			_isLongSkip = value;
		}
	}

	public bool IsFastSkip
	{
		set
		{
			_isFastSkip = value;
		}
	}

	public void PrepareTimer(int second)
	{
		_isFastSkip = false;
		_isLongSkip = false;
		_isTimeUp = false;
		_timeCounter = 0f;
		_delayCount = 0;
		_isTimeCounting = true;
		_isCompleted = false;
		_isFreedomSync = second == 65535;
		_countDownSecond = second;
		IsInfinity = false;
		_isEnableRequest = false;
	}

	public void DelayCount(int delayCount)
	{
		_delayCount = delayCount;
	}

	public void UpdateTimer(float gameMsecAdd, int monitorId)
	{
		if (GameManager.IsFreedomMode && GameManager.IsFreedomCountDown && !GameManager.IsFreedomTimerPause)
		{
			UpdateFreedomModeCounter();
		}
		if (!_isTimeCounting)
		{
			return;
		}
		if (_isFastSkip)
		{
			_timerAcceleration = ((_countDownSecond < 10) ? 12.5f : 50f);
		}
		else
		{
			_timerAcceleration = (IsFreezeTimer ? 0f : 1f);
		}
		if (!GameManager.IsEventMode || _isFastSkip)
		{
			_timeCounter += gameMsecAdd * _timerAcceleration;
		}
		_isFastSkip = false;
		if (_isLongSkip)
		{
			_isLongSkip = false;
			_countDownSecond -= 30;
			_timeCounter = 0f;
		}
		if (_timeCounter >= 1000f && !_isFreedomSync)
		{
			_timeCounter = 0f;
			_countDownSecond--;
		}
		if (_countDownSecond >= 0)
		{
			return;
		}
		if (_isTimeUp)
		{
			CountingDelayTime();
			return;
		}
		_countDownSecond = 0;
		_isTimeUp = true;
		if (_delayCount == 0)
		{
			_isTimeCounting = false;
			_isCompleted = true;
		}
	}

	private void UpdateFreedomModeCounter()
	{
		if (GameManager.GetFreedomModeMSec() > 100000)
		{
			IsInfinity = true;
			return;
		}
		IsInfinity = false;
		if (_isFreedomSync)
		{
			double num = (double)GameManager.GetFreedomModeMSec() * 0.001;
			_countDownSecond = (int)num;
		}
	}

	private void CountingDelayTime()
	{
		_delayCount--;
		if (_delayCount < 0)
		{
			_isCompleted = true;
			_isTimeCounting = false;
		}
	}

	public void DecrementTime(int decrementValue)
	{
		_countDownSecond -= decrementValue;
		if (_countDownSecond < 0)
		{
			_countDownSecond = 0;
			_timeCounter = 1000f;
		}
		else
		{
			_timeCounter = 0f;
		}
	}

	public void AddTime(int addSecond)
	{
		_countDownSecond += addSecond;
	}

	public void SetTimeZero()
	{
		_timeCounter = 0f;
		_countDownSecond = 0;
		_isTimeCounting = false;
		_isCompleted = true;
	}

	public void ResetCompleteFlag()
	{
		_isCompleted = false;
	}

	public void SetTimerSecurity(int second)
	{
		if (_countDownSecond <= second)
		{
			PrepareTimer(second);
		}
	}

	private void DebugCode(int monitorId)
	{
		if (DebugInput.GetKeyDown(KeyCode.F3))
		{
			if (GameManager.IsFreedomMode && Singleton<UserDataManager>.Instance.GetUserData(monitorId).IsEntry)
			{
				if (!_isEnableRequest)
				{
					if (!GameManager.IsFreedomTimerPause)
					{
						_isEnableRequest = true;
						GameManager.PauseFreedomModeTimer(isPause: true);
					}
				}
				else if (GameManager.IsFreedomTimerPause)
				{
					_isEnableRequest = false;
					GameManager.PauseFreedomModeTimer(isPause: false);
				}
			}
			_isTimeCounting = !_isTimeCounting;
		}
		if (DebugInput.GetKeyDown(KeyCode.F4))
		{
			_countDownSecond = 0;
		}
	}

	public TimerController()
	{
		using IniFile iniFile = new IniFile("mai2.ini");
		IsFreezeTimer = iniFile.getValue("Debug", "FreezeTimer", 0) == 1;
	}
}
