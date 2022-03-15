using AMDaemon;
using MAI2.Util;

namespace Manager
{
	public class AimeReaderManager
	{
		public enum Result
		{
			None,
			NewAime,
			NewFelica,
			Done,
			Error
		}

		private enum State
		{
			First,
			Idle,
			DBAlive,
			Start,
			Polling,
			Confirm,
			Result,
			Cancel,
			Cancelling,
			Done
		}

		private bool _readFlag;

		private bool _enableFlag;

		private AimeUnit _unit;

		private AimeResult _readResult;

		private AccessCode _accessCode;

		private AimeId _aimeId;

		private string _segaIdAuthKey;

		private AimeErrorInfo _errorInfo;

		private int _isAccepted;

		private AimeOfflineId _offlineId;

		private Result _result;

		private State currentState;

		public bool EnableFlag => _enableFlag;

		public Result GetResult()
		{
			return _result;
		}

		public AimeErrorCategory GetErrorCategory()
		{
			if (_errorInfo != null)
			{
				return _errorInfo.Category;
			}
			return AimeErrorCategory.None;
		}

		public string GetAccessCode()
		{
			return _accessCode.ToString();
		}

		public string GetOfflineIdString()
		{
			return _offlineId.Type switch
			{
				AimeOfflineIdType.AccessCode => _offlineId.GetAccessCodeData().ToString(), 
				AimeOfflineIdType.FeliCaId => _offlineId.GetFeliCaIdData().ToString(), 
				_ => "", 
			};
		}

		public AimeId GetAimeId()
		{
			return _aimeId;
		}

		public string GetSegaIdAuthKey()
		{
			return _segaIdAuthKey;
		}

		public void EnableRead(bool flag)
		{
			if (Aime.UnitCount > 0)
			{
				Aime.Units[0].Cancel();
			}
			_readFlag = false;
			_enableFlag = flag;
			_unit = null;
			_readResult = null;
			_accessCode = AccessCode.Invalid;
			_aimeId = AimeId.Invalid;
			_segaIdAuthKey = "";
			_errorInfo = null;
			_isAccepted = 0;
			_result = Result.None;
			currentState = State.First;
			_offlineId = AimeOfflineId.Zero;
		}

		public void Initialize()
		{
		}

		public void Execute()
		{
			switch (currentState)
			{
			case State.First:
				if (Core.IsReady && Aime.UnitCount > 0 && !Aime.Units[0].IsBusy)
				{
					_unit = Aime.Units[0];
					_readResult = null;
					_accessCode = AccessCode.Zero;
					_aimeId = AimeId.Zero;
					_segaIdAuthKey = "";
					_errorInfo = null;
					_isAccepted = 0;
					currentState = State.Idle;
					if (_unit.HasError && _unit.ErrorInfo.Category == AimeErrorCategory.Fatal)
					{
						Singleton<OperationManager>.Instance.IsAliveAimeReader = false;
					}
				}
				break;
			case State.Idle:
				if (_enableFlag && _unit.Start(AimeCommand.Scan))
				{
					_unit.SetLedStatus(AimeLedStatus.Scanning);
					currentState = State.Polling;
					_readFlag = true;
				}
				break;
			case State.Polling:
				if (_unit.IsBusy)
				{
					if (_unit.HasConfirm)
					{
						if (_unit.Confirm == AimeConfirm.AimeDB)
						{
							_result = Result.NewAime;
						}
						else if (_unit.Confirm == AimeConfirm.FeliCaDB)
						{
							_result = Result.NewFelica;
						}
						_isAccepted = 0;
						currentState = State.Confirm;
					}
				}
				else
				{
					currentState = State.Result;
				}
				break;
			case State.Confirm:
				if (_isAccepted == 1)
				{
					_isAccepted = 0;
					_unit.AcceptConfirm();
					currentState = State.Polling;
				}
				else if (_isAccepted == 2)
				{
					currentState = State.Cancel;
				}
				break;
			case State.Cancel:
				if (_unit.Cancel())
				{
					currentState = State.Cancelling;
				}
				else
				{
					currentState = State.Polling;
				}
				break;
			case State.Cancelling:
				if (!_unit.IsBusy)
				{
					_result = Result.Error;
					currentState = State.First;
				}
				break;
			case State.Result:
				if (_unit.HasError)
				{
					_errorInfo = _unit.ErrorInfo;
					if (_errorInfo.Category != AimeErrorCategory.Warning)
					{
						if (_errorInfo.Category == AimeErrorCategory.Network)
						{
							_unit.SetLedStatus(AimeLedStatus.Warning);
						}
						else
						{
							_unit.SetLedStatus(AimeLedStatus.Error);
						}
					}
					_result = Result.Error;
				}
				else
				{
					_unit.SetLedStatus(AimeLedStatus.Success);
				}
				if (_unit.HasResult)
				{
					_readResult = _unit.Result;
					_accessCode = _readResult.AccessCode;
					_aimeId = _readResult.AimeId;
					_segaIdAuthKey = _readResult.SegaIdAuthKey;
					_offlineId = _readResult.OfflineId;
					if (!_accessCode.IsValid || !_aimeId.IsValid)
					{
						_result = Result.Error;
					}
					else
					{
						_result = Result.Done;
					}
				}
				currentState = State.Done;
				break;
			case State.DBAlive:
			case State.Start:
			case State.Done:
				break;
			}
		}

		private void OnDestroy()
		{
			EnableRead(flag: false);
		}

		public bool AdvCheck()
		{
			if (_readResult != null || currentState == State.Confirm)
			{
				return true;
			}
			return false;
		}

		public bool AnyRead()
		{
			if (_unit != null && _unit.Result.IsValid && _enableFlag && _readFlag && (_unit.Result.OfflineId.IsValid || (_unit.HasError && currentState != 0 && currentState != State.Idle)))
			{
				return true;
			}
			return false;
		}

		public void Confirm(bool flag)
		{
			if (flag)
			{
				_isAccepted = 1;
			}
			else
			{
				_isAccepted = 2;
			}
		}

		public bool IsPollingState()
		{
			return currentState == State.Polling;
		}
	}
}
