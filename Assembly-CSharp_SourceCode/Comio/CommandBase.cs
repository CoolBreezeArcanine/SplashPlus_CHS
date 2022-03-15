using System.Diagnostics;

namespace Comio
{
	public abstract class CommandBase
	{
		private const uint TimeoutDef = 500u;

		private const uint RetryCountDef = 3u;

		private const uint RecvOnlyTimeoutMin = 500u;

		protected ComState ComState;

		protected PacketReqHeader ReqPacket;

		protected PacketAckHeader AckPacket;

		protected bool IsSendOnly;

		protected bool IsRecvOnlyCheckTimeout;

		protected byte RetryCount;

		protected byte ComErrorCount;

		protected bool ReportError;

		protected bool RecvError;

		protected uint Timeout;

		private readonly Stopwatch _stopWatch;

		private long _timerValue;

		protected CommandBase()
		{
			ComState = ComState.Def.Begin;
			ReqPacket = CreateReq();
			AckPacket = CreateAck();
			IsSendOnly = false;
			IsRecvOnlyCheckTimeout = false;
			RetryCount = 0;
			ComErrorCount = 0;
			ReportError = false;
			RecvError = false;
			Timeout = 500u;
			_stopWatch = new Stopwatch();
			_stopWatch.Start();
			_timerValue = 0L;
		}

		public abstract byte GetCommandNo();

		public abstract byte GetLength();

		public abstract PacketReqHeader CreateReq();

		public abstract PacketAckHeader CreateAck();

		public void Reset()
		{
			ComState = ComState.Def.Begin;
			IsRecvOnlyCheckTimeout = false;
			RetryCount = 0;
			ComErrorCount = 0;
			ReportError = false;
			RecvError = false;
		}

		public abstract void DoRecv();

		public void AcceptRecv(Packet packet)
		{
			PacketAckHeader packetAckHeader = (PacketAckHeader)packet;
			_timerValue = 0L;
			switch (packetAckHeader.status)
			{
			case 1:
				switch (packetAckHeader.report)
				{
				case 1:
					AckPacket.setBuffer(packetAckHeader);
					ComState = ComState.Def.Complete;
					DoRecv();
					break;
				case 2:
					if ((uint)(++ComErrorCount) < 3u)
					{
						ComState = ComState.Def.WaitSend;
					}
					else
					{
						ComState = ComState.Def.Complete;
					}
					break;
				case 3:
				case 4:
					ReportError = true;
					ComState = ComState.Def.Complete;
					break;
				default:
					RecvError = true;
					ComState = ComState.Def.Complete;
					break;
				}
				break;
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
				if ((uint)(++ComErrorCount) < 3u)
				{
					ComState = ComState.Def.WaitSend;
				}
				else
				{
					ComState = ComState.Def.Complete;
				}
				break;
			default:
				RecvError = true;
				ComState = ComState.Def.Complete;
				break;
			}
		}

		public ComState GetComState()
		{
			return ComState;
		}

		public void SetComState(ComState comState)
		{
			ComState = comState;
		}

		public void SetRecvOnlyTimeout(uint timeout)
		{
			if (timeout == 0)
			{
				IsRecvOnlyCheckTimeout = false;
				return;
			}
			if (timeout < 500)
			{
				timeout = 500u;
			}
			_timerValue = 0L;
			Timeout = timeout;
			IsRecvOnlyCheckTimeout = true;
		}

		public void SetTimeout(uint value)
		{
			Timeout = value;
		}

		public PacketReqHeader GetReq()
		{
			return ReqPacket;
		}

		public PacketAckHeader GetAck()
		{
			return AckPacket;
		}

		public byte GetRetryCount()
		{
			return RetryCount;
		}

		public bool ReqSend()
		{
			if (!ComState.IsBusy())
			{
				ComState = ComState.Def.WaitSend;
				IsSendOnly = false;
				RetryCount = 0;
				_timerValue = 0L;
				ComErrorCount = 0;
				ReportError = false;
				RecvError = false;
				return true;
			}
			return false;
		}

		public bool ReqSendto()
		{
			if (ComState != ComState.Def.WaitSend)
			{
				ComState = ComState.Def.WaitSend;
				IsSendOnly = true;
				RetryCount = 0;
				_timerValue = 0L;
				ComErrorCount = 0;
				ReportError = false;
				RecvError = false;
				return true;
			}
			return false;
		}

		public bool IsRecv()
		{
			if (ComState == ComState.Def.Complete)
			{
				return true;
			}
			return false;
		}

		public bool CheckTimeout(out bool isJustHappen)
		{
			isJustHappen = false;
			if (IsSendOnly)
			{
				return false;
			}
			if (ComState == ComState.Def.Timeout)
			{
				return true;
			}
			long elapsedMilliseconds = _stopWatch.ElapsedMilliseconds;
			if (elapsedMilliseconds < 1000)
			{
				_timerValue += elapsedMilliseconds;
			}
			_stopWatch.Reset();
			_stopWatch.Start();
			if ((ComState.IsBusy() || IsRecvOnlyCheckTimeout) && _timerValue > Timeout)
			{
				if ((uint)(++RetryCount) >= 3u)
				{
					ComState = ComState.Def.Timeout;
					isJustHappen = true;
					return true;
				}
				_timerValue = 0L;
				if (!IsRecvOnlyCheckTimeout)
				{
					ComState = ComState.Def.WaitSend;
				}
			}
			return false;
		}

		public bool IsComError()
		{
			return (uint)ComErrorCount >= 3u;
		}

		public bool IsReportError()
		{
			return ReportError;
		}

		public bool IsRecvError()
		{
			return RecvError;
		}

		public void ClearError()
		{
			RetryCount = 0;
			ComErrorCount = 0;
			ReportError = false;
			RecvError = false;
			_stopWatch.Reset();
			_stopWatch.Start();
			_timerValue = 0L;
		}
	}
}
