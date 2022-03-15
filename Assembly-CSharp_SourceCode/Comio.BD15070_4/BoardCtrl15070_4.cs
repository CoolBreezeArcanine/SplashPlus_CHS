using System.Diagnostics;
using UnityEngine;

namespace Comio.BD15070_4
{
	public class BoardCtrl15070_4 : BoardCtrlBase
	{
		private enum Mode
		{
			Init,
			InitBoardInitReset,
			InitBoardGetBoardInfo,
			InitBoardGetProtocolVersion,
			InitBoardSetTimeoutInfinite,
			InitBoardReset,
			InitBoardWaitReset,
			InitBoardGetBoardStatus,
			InitBoardSetTimeout,
			InitBoardGetEepRom,
			InitBoardSetEepRom,
			InitBoardSetDc,
			InitBoardSetDcUpdate,
			InitBoardSetGsOff,
			InitBoardSetFetOff,
			InitBoardSetGsUpdate,
			InitBoardSetLedCount,
			WaitAutoSend,
			Exec,
			Error,
			StartHalt,
			StartHaltWaitReset,
			Halt
		}

		private enum EepRomAddress
		{
			Enable,
			Fet0,
			Fet1,
			Fet2,
			DcRed,
			DcGreen,
			DcBlue,
			Out15,
			End
		}

		private enum EepRomCheck
		{
			None,
			Ok,
			Ng,
			End
		}

		private struct EepRom
		{
			public EepRomCheck Check;

			public byte ReadData;

			public byte WriteData;

			public byte WriteCount;
		}

		private const uint ResetWait = 1000u;

		private static BoardNo BoardNo = new BoardNo("15070-04");

		private const int EepromWriteRetryMax = 3;

		private Board15070_4.InitParam _initParam;

		private Mode _mode;

		private bool _loop;

		private bool _resetOn;

		private ErrorNo _errorNo;

		private GetBoardStatusCommand _getBoardStatusCommand;

		private SetTimeoutCommand _setTimeoutCommand;

		private GetBoardInfoCommand _getBoardInfoCommand;

		private GetProtocolVersionCommand _getProtocolVersionCommand;

		private ResetCommand _resetCommand;

		private GetEEPRomCommand _getEepRomCommand;

		private SetEEPRomCommand _setEepRomCommand;

		private SetDcCommand _setDcCommand;

		private SetDcUpdateCommand _setDcUpdateCommand;

		private SetLedGs8BitMultiCommand _setLedGs8BitMultiCommand;

		private SetLedFetCommand _setLedFetCommand;

		private SetLedGsUpdateCommand _setLedGsUpdateCommand;

		private IoCtrl _ioCtrl;

		private Stopwatch _modeTimer;

		private EepRom[] _eepRom;

		private EepRomAddress _eepRomCheckAdress;

		private Gs8BitMulti _dcParam;

		public BoardCtrl15070_4(Board15070_4.InitParam initParam)
			: base(initParam.BoardNodeId)
		{
			_construct(initParam);
		}

		public bool IsError()
		{
			lock (Cs)
			{
				return _isError();
			}
		}

		public void ClearError()
		{
			lock (Cs)
			{
				_errorNo = ErrorNo.Begin;
				ClearErrorRegisteredCommand();
			}
		}

		public ErrorNo GetErrorNo()
		{
			lock (Cs)
			{
				return _errorNo;
			}
		}

		public override bool CheckFirmVersion(byte boardVersion, byte fileVersion)
		{
			return new FirmInfo
			{
				Revision = boardVersion
			}.CheckFirmVersionSame(fileVersion);
		}

		public override bool ReqHalt()
		{
			lock (Cs)
			{
				return _reqHalt();
			}
		}

		public override bool IsHalted()
		{
			lock (Cs)
			{
				return _isHalted();
			}
		}

		public override void Reset()
		{
			lock (Cs)
			{
				_reset();
			}
		}

		public void SetLedData(byte ledPos, LedData data)
		{
			lock (Cs)
			{
				_setLedData(ledPos, data);
			}
		}

		public void SetLedDataMulti(Color32 data, byte speed)
		{
			lock (Cs)
			{
				_setLedDataMulti(data, speed);
			}
		}

		public void SetLedDataMultiFade(Color32 data, byte speed)
		{
			lock (Cs)
			{
				_setLedDataMultiFade(data, speed);
			}
		}

		public void SetLedDataMultiFet(Color32 data)
		{
			lock (Cs)
			{
				_setLedDataMultiFet(data);
			}
		}

		public void SetLedDataUpdate()
		{
			lock (Cs)
			{
				_setLedDataUpdate();
			}
		}

		public void SetLedDataAllOff()
		{
			lock (Cs)
			{
				_setLedDataAllOff();
			}
		}

		public bool IsWithoutResponse()
		{
			return _isWithoutResponse();
		}

		public ushort GetLedInterval()
		{
			return _getLedInterval();
		}

		public static BoardNo GetDefBoardNo()
		{
			return BoardNo;
		}

		public override void Initialize()
		{
			lock (Cs)
			{
				_initialize();
			}
		}

		public override void Terminate()
		{
			lock (Cs)
			{
			}
		}

		public override void Execute()
		{
			lock (Cs)
			{
				_execute();
			}
		}

		public override void ExecThread()
		{
			lock (Cs)
			{
				_execThread();
			}
		}

		public override void NotifyBaseError(BaseErrorNo baseErrorNo)
		{
			lock (Cs)
			{
				switch (baseErrorNo)
				{
				case BaseErrorNo.BaseErrorNoTimeout:
					_setError(ErrorNo.Timeout);
					break;
				case BaseErrorNo.BaseErrorNoReportError:
					_setError(ErrorNo.ReportError);
					break;
				case BaseErrorNo.BaseErrorNoRecvError:
					_setError(ErrorNo.RecvError);
					break;
				case BaseErrorNo.BaseErrorNoComError:
					_setError(ErrorNo.Timeout);
					break;
				case BaseErrorNo.BaseErrorNoSumError:
					_setError(ErrorNo.SumError);
					break;
				}
			}
		}

		private void _construct(Board15070_4.InitParam initParam)
		{
			_initParam = initParam;
			_mode = Mode.Init;
			_loop = false;
			_resetOn = false;
			_errorNo = ErrorNo.Begin;
			_getBoardStatusCommand = new GetBoardStatusCommand();
			_setTimeoutCommand = new SetTimeoutCommand();
			_getBoardInfoCommand = new GetBoardInfoCommand();
			_getProtocolVersionCommand = new GetProtocolVersionCommand();
			_resetCommand = new ResetCommand();
			_getEepRomCommand = new GetEEPRomCommand();
			_setEepRomCommand = new SetEEPRomCommand();
			_setDcCommand = new SetDcCommand();
			_setDcUpdateCommand = new SetDcUpdateCommand();
			_setLedGs8BitMultiCommand = new SetLedGs8BitMultiCommand();
			_setLedFetCommand = new SetLedFetCommand();
			_setLedGsUpdateCommand = new SetLedGsUpdateCommand();
			_ioCtrl = new IoCtrl(this);
			_modeTimer = new Stopwatch();
			if (33 > _initParam.LedInteval)
			{
				_initParam.LedInteval = 33;
			}
			_initWork();
			InitAndRegisterCommand(_getBoardInfoCommand);
			InitAndRegisterCommand(_getBoardStatusCommand);
			InitAndRegisterCommand(_getProtocolVersionCommand);
			InitAndRegisterCommand(_resetCommand);
			InitAndRegisterCommand(_setTimeoutCommand);
			InitAndRegisterCommand(_getEepRomCommand);
			InitAndRegisterCommand(_setEepRomCommand);
			InitAndRegisterCommand(_setDcCommand);
			InitAndRegisterCommand(_setDcUpdateCommand);
			InitAndRegisterCommand(_setLedGs8BitMultiCommand);
			InitAndRegisterCommand(_setLedFetCommand);
			InitAndRegisterCommand(_setLedGsUpdateCommand);
			_eepRomCheckAdress = EepRomAddress.Enable;
			_eepRom = new EepRom[8];
			for (int i = 0; i < 8; i++)
			{
				_eepRom[i] = new EepRom
				{
					Check = EepRomCheck.None,
					ReadData = 0,
					WriteData = 0,
					WriteCount = 0
				};
			}
			_dcParam = new Gs8BitMulti
			{
				Start = 0,
				End = 10,
				Skip = 1,
				Color = 
				{
					r = 63,
					g = 63,
					b = 63,
					a = byte.MaxValue
				}
			};
		}

		private void _initWork()
		{
			_resetOn = false;
			_errorNo = ErrorNo.Begin;
		}

		private void _initialize()
		{
			_initWork();
			_ioCtrl.Initialize();
			_loop = false;
			_mode = Mode.Init;
		}

		private bool _isError()
		{
			return _errorNo != ErrorNo.Begin;
		}

		private void _setError(ErrorNo no)
		{
			if (!_isHalted() && Mode.StartHalt != _mode && !_isError())
			{
				_errorNo = no;
				_mode = Mode.Error;
			}
		}

		private bool _reqHalt()
		{
			if (_isHalted())
			{
				return true;
			}
			if (_mode != Mode.StartHalt)
			{
				_mode = Mode.StartHalt;
				return true;
			}
			return false;
		}

		private bool _isHalted()
		{
			return _mode == Mode.Halt;
		}

		private void _reset()
		{
			if (!_resetOn)
			{
				_errorNo = ErrorNo.Begin;
				ClearErrorRegisteredCommand();
				if (IsHalted())
				{
					_resetOn = true;
				}
				else if (_mode != Mode.StartHalt)
				{
					_resetOn = true;
					_mode = Mode.StartHalt;
				}
				if (_resetOn)
				{
					InitBoard = false;
				}
			}
		}

		private void _setLedData(byte ledPos, LedData data)
		{
			_ioCtrl.SetLedData(ledPos, data);
		}

		private void _setLedDataMulti(Color32 data, byte speed)
		{
			_ioCtrl.SetLedDataMulti(data, speed);
		}

		private void _setLedDataMultiFade(Color32 data, byte speed)
		{
			_ioCtrl.SetLedDataMultiFade(data, speed);
		}

		private void _setLedDataMultiFet(Color32 data)
		{
			_ioCtrl.SetLedDataMultiFet(data);
		}

		private void _setLedDataUpdate()
		{
			_ioCtrl.SetUpdateGs();
		}

		private void _setLedDataAllOff()
		{
			_ioCtrl.SetLedDataAllOff();
		}

		private bool _isWithoutResponse()
		{
			return _initParam.WithoutResponse;
		}

		private ushort _getLedInterval()
		{
			return _initParam.LedInteval;
		}

		private void _md_init()
		{
			Initialize();
			InitBase();
			_mode = Mode.InitBoardInitReset;
		}

		private void _md_initBoard_InitReset()
		{
			if (ExecCommand(_resetCommand))
			{
				_modeTimer.Reset();
				_modeTimer.Start();
				_mode = Mode.InitBoardGetBoardInfo;
				_resetCommand.Reset();
			}
		}

		private void _md_initBoard_GetBoardInfo()
		{
			if (ExecCommand(_getBoardInfoCommand))
			{
				BoardSpecInfo.BoardNo = _getBoardInfoCommand.getBoardNo();
				BoardSpecInfo.FirmInfo.Revision = _getBoardInfoCommand.getFirmRevision();
				if (!BoardSpecInfo.BoardNo.IsEqual(BoardNo))
				{
					_setError(ErrorNo.Timeout);
					return;
				}
				BoardSpecInfoRecv = true;
				_mode = Mode.InitBoardGetProtocolVersion;
			}
		}

		private void _md_initBoard_GetProtocolVersion()
		{
			if (ExecCommand(_getProtocolVersionCommand))
			{
				BoardSpecInfo.FirmInfo.FirmAppli = _getProtocolVersionCommand.isAppliMode();
				BoardSpecInfo.FirmInfo.Major = _getProtocolVersionCommand.getMajor();
				BoardSpecInfo.FirmInfo.Minor = _getProtocolVersionCommand.getMinor();
				_mode = Mode.InitBoardReset;
			}
		}

		private void _md_initBoard_SetTimeoutInfinite()
		{
			if (ExecCommand(_setTimeoutCommand))
			{
				_resetCommand.set();
				_mode = Mode.InitBoardReset;
			}
		}

		private void _md_initBoard_Reset()
		{
			if (ExecCommand(_resetCommand))
			{
				_modeTimer.Reset();
				_modeTimer.Start();
				_mode = Mode.InitBoardWaitReset;
			}
		}

		private void _md_initBoard_WaitReset()
		{
			if (_modeTimer.ElapsedMilliseconds >= 1000)
			{
				_mode = Mode.InitBoardGetEepRom;
				_eepRomCheckAdress = EepRomAddress.Enable;
				for (int i = 0; i < 8; i++)
				{
					_eepRom[i].Check = EepRomCheck.None;
					_eepRom[i].ReadData = 0;
					_eepRom[i].WriteData = 0;
					_eepRom[i].WriteCount = 0;
				}
				_getEepRomCommand.SetEEPDataAdress((byte)_eepRomCheckAdress);
			}
		}

		private void _md_initBoard_GetEEPRom()
		{
			if (!ExecCommand(_getEepRomCommand))
			{
				return;
			}
			_eepRom[(int)_eepRomCheckAdress].ReadData = _getEepRomCommand.GetEEPData();
			if (_eepRom[(int)_eepRomCheckAdress].ReadData != _eepRom[(int)_eepRomCheckAdress].WriteData)
			{
				if (_eepRom[(int)_eepRomCheckAdress].WriteCount >= 3)
				{
					_setError(ErrorNo.EepWriteError);
					return;
				}
				_eepRom[(int)_eepRomCheckAdress].Check = EepRomCheck.Ng;
				_setEepRomCommand.Reset();
				_setEepRomCommand.SetEEPData((byte)_eepRomCheckAdress, _eepRom[(int)_eepRomCheckAdress].WriteData);
				_mode = Mode.InitBoardSetEepRom;
				return;
			}
			_eepRom[(int)_eepRomCheckAdress].Check = EepRomCheck.Ok;
			_eepRomCheckAdress++;
			if (_eepRomCheckAdress >= EepRomAddress.End)
			{
				_mode = Mode.InitBoardSetDc;
				_setDcCommand.setDc(_dcParam);
			}
			else
			{
				_getEepRomCommand.Reset();
				_getEepRomCommand.SetEEPDataAdress((byte)_eepRomCheckAdress);
			}
		}

		private void _md_initBoard_SetEEPRom()
		{
			if (ExecCommand(_setEepRomCommand))
			{
				_mode = Mode.InitBoardGetEepRom;
				_eepRom[(int)_eepRomCheckAdress].WriteCount++;
			}
		}

		private void _md_initBoard_SetDc()
		{
			if (ExecCommand(_setDcCommand))
			{
				_mode = Mode.InitBoardSetDcUpdate;
			}
		}

		private void _md_initBoard_SetDcUpdate()
		{
			if (ExecCommand(_setDcUpdateCommand))
			{
				_setLedGs8BitMultiCommand.setAllOff();
				_mode = Mode.InitBoardSetGsOff;
			}
		}

		private void _md_initBoard_SetGsOff()
		{
			if (ExecCommand(_setLedGs8BitMultiCommand))
			{
				_setLedFetCommand.setColorOff();
				_mode = Mode.InitBoardSetFetOff;
			}
		}

		private void _md_initBoard_SetFetOff()
		{
			if (ExecCommand(_setLedFetCommand))
			{
				_mode = Mode.InitBoardSetGsUpdate;
			}
		}

		private void _md_initBoard_SetGsUpdate()
		{
			if (ExecCommand(_setLedGsUpdateCommand))
			{
				_mode = Mode.InitBoardGetBoardStatus;
			}
		}

		private void _md_initBoard_GetBoardStatus()
		{
			if (ExecCommand(_getBoardStatusCommand))
			{
				_setTimeoutCommand.setTimeout(_initParam.Timeout);
				_mode = Mode.InitBoardSetTimeout;
			}
		}

		private void _md_initBoard_SetTimeout()
		{
			InitBoard = true;
			_mode = Mode.Exec;
			_loop = true;
		}

		private void _md_exec()
		{
			_ioCtrl.Execute();
		}

		private void _md_error()
		{
			if (_errorNo == ErrorNo.Begin)
			{
				_mode = Mode.Init;
				_loop = true;
			}
		}

		private void _md_startHalt()
		{
			_resetCommand.set();
			if (ExecCommand(_resetCommand) || _resetCommand.GetComState() == ComState.Def.Timeout)
			{
				_modeTimer.Reset();
				_modeTimer.Start();
				_mode = Mode.StartHaltWaitReset;
			}
		}

		private void _md_startHalt_WaitReset()
		{
			if (_modeTimer.ElapsedMilliseconds >= 1000)
			{
				InitBase();
				_mode = Mode.Halt;
			}
		}

		private void _md_halt()
		{
			if (_resetOn)
			{
				_mode = Mode.Init;
				_resetOn = false;
				_loop = true;
			}
		}

		private void _execute()
		{
		}

		private void _execThread()
		{
			do
			{
				_loop = false;
				switch (_mode)
				{
				case Mode.Init:
					_md_init();
					break;
				case Mode.InitBoardGetBoardInfo:
					_md_initBoard_GetBoardInfo();
					break;
				case Mode.InitBoardInitReset:
					_md_initBoard_InitReset();
					break;
				case Mode.InitBoardGetProtocolVersion:
					_md_initBoard_GetProtocolVersion();
					break;
				case Mode.InitBoardSetTimeoutInfinite:
					_md_initBoard_SetTimeoutInfinite();
					break;
				case Mode.InitBoardReset:
					_md_initBoard_Reset();
					break;
				case Mode.InitBoardWaitReset:
					_md_initBoard_WaitReset();
					break;
				case Mode.InitBoardGetEepRom:
					_md_initBoard_GetEEPRom();
					break;
				case Mode.InitBoardSetEepRom:
					_md_initBoard_SetEEPRom();
					break;
				case Mode.InitBoardSetDc:
					_md_initBoard_SetDc();
					break;
				case Mode.InitBoardSetDcUpdate:
					_md_initBoard_SetDcUpdate();
					break;
				case Mode.InitBoardSetGsOff:
					_md_initBoard_SetGsOff();
					break;
				case Mode.InitBoardSetFetOff:
					_md_initBoard_SetFetOff();
					break;
				case Mode.InitBoardSetGsUpdate:
					_md_initBoard_SetGsUpdate();
					break;
				case Mode.InitBoardGetBoardStatus:
					_md_initBoard_GetBoardStatus();
					break;
				case Mode.InitBoardSetTimeout:
					_md_initBoard_SetTimeout();
					break;
				case Mode.Exec:
					_md_exec();
					break;
				case Mode.Error:
					_md_error();
					break;
				case Mode.StartHalt:
					_md_startHalt();
					break;
				case Mode.StartHaltWaitReset:
					_md_startHalt_WaitReset();
					break;
				case Mode.Halt:
					_md_halt();
					break;
				}
			}
			while (_loop);
		}
	}
}
