using System.Diagnostics;
using System.IO.Ports;
using Comio;
using Comio.BD15070_4;
using DB;
using UnityEngine;

namespace Mecha
{
	public class Bd15070_4Control
	{
		public enum FirmUpdateResultId
		{
			Init,
			Ok,
			Ng
		}

		private enum FirmUpdateMode
		{
			Idle,
			HaltReq,
			HaltWait,
			ProcReq,
			ProcWait,
			ResetReq,
			ResetWait
		}

		private const byte BoardNodeId = 17;

		private int _bdID;

		private readonly Board15070_4 _board;

		private readonly Host _host;

		private bool _initHostSuccess;

		private bool _initBoardSuccess;

		private ErrorNo _errorNo;

		private readonly LedData _ledData;

		private Color32 _ledMultiColor;

		private FirmUpdateMode _firmUpdateMode;

		private readonly System.Diagnostics.Process _firmProsess;

		private FirmUpdateResultId _firmUpdateResultId;

		private readonly Stopwatch _stopwatch;

		public Bd15070_4Control(string portName, int index)
		{
			_bdID = index;
			_board = null;
			_host = null;
			_initHostSuccess = false;
			_initBoardSuccess = false;
			_errorNo = ErrorNo.Begin;
			_ledData = new LedData();
			_ledMultiColor = default(Color32);
			_firmUpdateMode = FirmUpdateMode.Idle;
			_firmProsess = null;
			_firmUpdateResultId = FirmUpdateResultId.Init;
			_stopwatch = new Stopwatch();
			_errorNo = ErrorNo.Begin;
			byte firmVersion = 144;
			ushort firmSum = 0;
			_ledData.SetOff();
			Host.InitParam initParam = new Host.InitParam();
			string text = (initParam.PortName = portName);
			initParam.AsyncIo = false;
			initParam.ComParam.BaudRate = 115200;
			initParam.ComParam.Parity = Parity.None;
			initParam.ComParam.DataBits = 8;
			initParam.ComParam.StopBits = StopBits.One;
			initParam.ComParam.DtrEnable = false;
			initParam.ComParam.RtsEnable = false;
			initParam.ComParam.ReadTimeout = 1;
			initParam.ComParam.WriteTimeout = 180;
			initParam.Interval = 33;
			Board15070_4.InitParam initParam2 = new Board15070_4.InitParam
			{
				BoardNodeId = 17,
				FirmVersion = firmVersion,
				FirmSum = firmSum,
				Timeout = 100
			};
			_board = new Board15070_4(initParam2);
			_host = new Host(initParam);
			InitHost();
			_firmUpdateMode = FirmUpdateMode.Idle;
		}

		public void Terminate()
		{
			_host.Terminate();
		}

		public void PreExecute()
		{
			_host.Execute();
			ExeFirmUpdate();
			_initBoardSuccess = _board.IsInitBoard();
			_errorNo = _board.GetErrorNo();
		}

		public bool IsInitialized(out bool success)
		{
			bool result = false;
			success = false;
			if (GetErrorId().IsValid())
			{
				result = true;
				success = false;
			}
			else if (_initBoardSuccess)
			{
				result = true;
				success = true;
			}
			return result;
		}

		public bool Reset()
		{
			bool result = false;
			if (IsInitialized(out var _))
			{
				_board.Reset();
				result = true;
			}
			return result;
		}

		public bool GetBoardSpecInfo(out BoardSpecInfo outInfo)
		{
			bool result = false;
			outInfo = new BoardSpecInfo();
			outInfo.Clear();
			if (_board.IsBoardSpecInfoRecv())
			{
				outInfo = _board.GetBoardSpecInfo();
				result = true;
			}
			return result;
		}

		public bool IsNeedFirmUpdate()
		{
			bool result = false;
			if (GetBoardSpecInfo(out var outInfo) && outInfo.BoardNo.IsEqual(Board15070_4.GetDefBoardNo()))
			{
				byte version = 144;
				if (!outInfo.FirmInfo.CheckFirmVersionSame(version))
				{
					result = true;
				}
				if (44535 != outInfo.FirmInfo.Sum)
				{
					result = true;
				}
				if (!outInfo.FirmInfo.IsAppliMode())
				{
					result = true;
				}
				ErrorID errorId = GetErrorId();
				if (errorId.IsValid() && errorId.IsFirmUpdate())
				{
					result = true;
				}
			}
			return result;
		}

		public bool StartFirmUpdate()
		{
			bool result = false;
			if (IsNeedFirmUpdate())
			{
				_firmUpdateMode = FirmUpdateMode.HaltReq;
				result = true;
			}
			return result;
		}

		public bool IsFinishedFirmUpdate(out FirmUpdateResultId resultId)
		{
			resultId = FirmUpdateResultId.Init;
			bool result;
			if (_firmUpdateMode == FirmUpdateMode.Idle)
			{
				result = true;
				resultId = _firmUpdateResultId;
			}
			else
			{
				result = false;
				resultId = FirmUpdateResultId.Init;
			}
			return result;
		}

		public ErrorID GetErrorId()
		{
			ErrorID result = ErrorID.Invalid;
			if (!_initHostSuccess)
			{
				result = ((_bdID != 0) ? ErrorID.LedBoard_Right_InitError : ErrorID.LedBoard_Left_InitError);
			}
			else
			{
				switch (_errorNo)
				{
				case ErrorNo.Timeout:
					result = ((_bdID != 0) ? ErrorID.LedBoard_Right_InitError : ErrorID.LedBoard_Left_InitError);
					break;
				case ErrorNo.SumError:
				case ErrorNo.ReportError:
				case ErrorNo.RecvError:
					result = ((_bdID == 0) ? ErrorID.LedBoard_Left_ResponseError : ErrorID.LedBoard_Right_ResponseError);
					break;
				case ErrorNo.FirmError:
					result = ((_bdID == 0) ? ErrorID.LedBoard_Left_FirmError : ErrorID.LedBoard_Right_FirmError);
					break;
				case ErrorNo.FirmVersionError:
					result = ((_bdID == 0) ? ErrorID.LedBoard_Left_FirmVerError : ErrorID.LedBoard_Right_FirmVerError);
					break;
				}
			}
			return result;
		}

		public void ClearError()
		{
			_board.ClearError();
			_errorNo = ErrorNo.Begin;
		}

		public float GetLedAnimeSpeed()
		{
			return 0.5f;
		}

		public void SetColor(byte ledPos, Color32 color)
		{
			_ledData.SetColorRgb(ledPos, color.r, color.g, color.b);
			_board.SetLedData(ledPos, _ledData);
		}

		public void SetColorMulti(Color32 color, byte speed)
		{
			_ledMultiColor = color;
			_board.SetLedDataMulti(_ledMultiColor, speed);
		}

		public void SetColorMultiFade(Color32 color, byte speed)
		{
			_ledMultiColor = color;
			_board.SetLedDataMultiFade(_ledMultiColor, speed);
		}

		public void SetColorMultiFet(Color32 color)
		{
			_ledMultiColor = color;
			_board.SetLedDataMultiFet(_ledMultiColor);
		}

		public void SetColorFet(byte ledPos, byte color)
		{
			_ledData.SetColorRgb(ledPos, color);
			_board.SetLedData(ledPos, _ledData);
		}

		public void SetColorUpdate()
		{
			_board.SetLedDataUpdate();
		}

		public void SetLedAllOff()
		{
			_board.SetLedDataAllOff();
		}

		private void InitHost()
		{
			if (!_initHostSuccess && _host.Initialize())
			{
				_host.RegisterBoard(_board);
				_initHostSuccess = true;
			}
		}

		private void ExeFirmUpdate()
		{
			if (_firmUpdateMode == FirmUpdateMode.Idle)
			{
				return;
			}
			bool flag = false;
			switch (_firmUpdateMode)
			{
			case FirmUpdateMode.HaltReq:
				_firmUpdateResultId = FirmUpdateResultId.Init;
				_board.ReqHalt();
				_firmUpdateMode = FirmUpdateMode.HaltWait;
				_stopwatch.Reset();
				_stopwatch.Start();
				break;
			case FirmUpdateMode.HaltWait:
				if (_board.IsHalted())
				{
					_firmUpdateMode = FirmUpdateMode.ProcReq;
				}
				else if (_stopwatch.ElapsedMilliseconds >= 30000)
				{
					flag = true;
				}
				break;
			case FirmUpdateMode.ProcWait:
				if (_firmProsess.HasExited)
				{
					if (_firmProsess.ExitCode == 0 || _firmProsess.ExitCode == 1)
					{
						_firmUpdateMode = FirmUpdateMode.ResetReq;
					}
					else
					{
						flag = true;
					}
				}
				else if (_stopwatch.ElapsedMilliseconds >= 300000)
				{
					flag = true;
				}
				break;
			case FirmUpdateMode.ResetReq:
				InitHost();
				if (_initHostSuccess)
				{
					_board.Reset();
				}
				_firmUpdateResultId = FirmUpdateResultId.Ok;
				_firmUpdateMode = FirmUpdateMode.ResetWait;
				_stopwatch.Reset();
				_stopwatch.Start();
				break;
			case FirmUpdateMode.ResetWait:
				if (_stopwatch.ElapsedMilliseconds >= 3000)
				{
					_firmUpdateMode = FirmUpdateMode.Idle;
				}
				break;
			}
			if (flag)
			{
				InitHost();
				_firmUpdateResultId = FirmUpdateResultId.Ng;
				_firmUpdateMode = FirmUpdateMode.Idle;
			}
		}
	}
}
