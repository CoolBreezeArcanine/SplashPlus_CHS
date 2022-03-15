using System.Collections.Generic;

namespace Comio
{
	public class BoardCtrlBase
	{
		public class PacketQueue : Queue<Packet>
		{
			public bool Empty()
			{
				return base.Count == 0;
			}
		}

		public class CommandMap : Dictionary<byte, CommandBase>
		{
		}

		public enum BaseErrorNo
		{
			BaseErrorNoNone,
			BaseErrorNoTimeout,
			BaseErrorNoReportError,
			BaseErrorNoRecvError,
			BaseErrorNoComError,
			BaseErrorNoSumError,
			BaseErrorNoEnd
		}

		protected byte BoardNodeId;

		protected object Cs;

		protected bool InitBoard;

		protected bool BoardSpecInfoRecv;

		protected BoardSpecInfo BoardSpecInfo;

		protected CommandMap Command;

		protected PacketQueue Queue;

		protected PacketQueue QueueFet;

		protected byte ExecCommandNo;

		public BoardCtrlBase(byte boardNodeId)
		{
			BoardNodeId = boardNodeId;
			Cs = new object();
			InitBoard = false;
			BoardSpecInfoRecv = false;
			BoardSpecInfo = new BoardSpecInfo();
			Command = new CommandMap();
			Queue = new PacketQueue();
			QueueFet = new PacketQueue();
			ExecCommandNo = 0;
		}

		public virtual void Initialize()
		{
		}

		public virtual void Terminate()
		{
		}

		public virtual void Execute()
		{
		}

		public virtual void ExecThread()
		{
		}

		public virtual void NotifyBaseError(BaseErrorNo baseErrorNo)
		{
		}

		public virtual bool IsInitBoard()
		{
			lock (Cs)
			{
				return InitBoard;
			}
		}

		public virtual bool CheckFirmVersion(byte boardVersion, byte fileVersion)
		{
			return new FirmInfo
			{
				Revision = boardVersion
			}.CheckFirmVersion(fileVersion);
		}

		public virtual bool ReqHalt()
		{
			lock (Cs)
			{
				return false;
			}
		}

		public virtual bool IsHalted()
		{
			lock (Cs)
			{
				return false;
			}
		}

		public virtual void Reset()
		{
			lock (Cs)
			{
			}
		}

		public byte GetBoardNodeId()
		{
			lock (Cs)
			{
				return BoardNodeId;
			}
		}

		public bool IsBoardSpecInfoRecv()
		{
			lock (Cs)
			{
				return BoardSpecInfoRecv;
			}
		}

		public BoardSpecInfo GetBoardSpecInfo()
		{
			lock (Cs)
			{
				return BoardSpecInfo;
			}
		}

		public bool ExecCommand(byte cmdNo)
		{
			if (!Command.TryGetValue(cmdNo, out var value))
			{
				return false;
			}
			switch (value.GetComState().GetEnum())
			{
			case ComState.Def.Begin:
				if (value.ReqSend())
				{
					Queue.Enqueue(value.GetReq());
				}
				break;
			case ComState.Def.Complete:
				value.SetComState(ComState.Def.Begin);
				return true;
			}
			return false;
		}

		public bool ExecCommand(CommandBase cmd)
		{
			return ExecCommand(cmd.GetCommandNo());
		}

		public bool SendCommand(byte cmdNo)
		{
			if (Command.TryGetValue(cmdNo, out var value) && value.ReqSend())
			{
				Queue.Enqueue(value.GetReq());
				return true;
			}
			return false;
		}

		public bool SendCommand(CommandBase cmd)
		{
			return SendCommand(cmd.GetCommandNo());
		}

		public bool SendtoCommand(byte cmdNo)
		{
			if (Command.TryGetValue(cmdNo, out var value) && value.ReqSendto())
			{
				Queue.Enqueue(value.GetReq());
				return true;
			}
			return false;
		}

		public void SendtoCommand(CommandBase cmd)
		{
			SendtoCommand(cmd.GetCommandNo());
		}

		public void SendForceCommand(CommandBase cmd)
		{
			if (cmd.GetCommandNo() == 57)
			{
				QueueFet.Clear();
				QueueFet.Enqueue(cmd.GetReq());
			}
			else
			{
				Queue.Enqueue(cmd.GetReq());
			}
		}

		protected void InitBase()
		{
			InitBoard = false;
			ResetRegisteredCommand();
		}

		public bool RegisterCommand(byte cmdNo, CommandBase cmd)
		{
			if (!Command.ContainsKey(cmdNo))
			{
				Command[cmdNo] = cmd;
				return true;
			}
			return false;
		}

		public void InitCommand(CommandBase cmd, byte cmdNo, byte length = 0)
		{
			PacketReqHeader req = cmd.GetReq();
			req.sync = 224;
			req.dstNodeID = GetBoardNodeId();
			req.srcNodeID = 1;
			req.length = (byte)(length - 5);
			req.command = cmdNo;
		}

		public bool InitAndRegisterCommand(CommandBase cmd, byte cmdNo, byte length)
		{
			InitCommand(cmd, cmdNo, length);
			return RegisterCommand(cmdNo, cmd);
		}

		public bool InitAndRegisterCommand(CommandBase cmd)
		{
			return InitAndRegisterCommand(cmd, cmd.GetCommandNo(), cmd.GetLength());
		}

		public void ResetRegisteredCommand()
		{
			foreach (KeyValuePair<byte, CommandBase> item in Command)
			{
				item.Value.Reset();
			}
		}

		public void ClearErrorRegisteredCommand()
		{
			foreach (KeyValuePair<byte, CommandBase> item in Command)
			{
				item.Value.ClearError();
			}
		}

		public void UpdateCommand(PacketQueue sendPacketQueue)
		{
			lock (Cs)
			{
				while (Queue.Count > 0)
				{
					sendPacketQueue.Enqueue(Queue.Dequeue());
				}
				while (QueueFet.Count > 0)
				{
					sendPacketQueue.Enqueue(QueueFet.Dequeue());
				}
				foreach (KeyValuePair<byte, CommandBase> item in Command)
				{
					CommandBase value = item.Value;
					bool isJustHappen;
					if (value.IsReportError())
					{
						NotifyBaseError(BaseErrorNo.BaseErrorNoReportError);
					}
					else if (value.IsRecvError())
					{
						NotifyBaseError(BaseErrorNo.BaseErrorNoRecvError);
					}
					else if (value.IsComError())
					{
						NotifyBaseError(BaseErrorNo.BaseErrorNoComError);
					}
					else if (value.CheckTimeout(out isJustHappen))
					{
						if (isJustHappen)
						{
							NotifyBaseError(BaseErrorNo.BaseErrorNoTimeout);
						}
					}
					else if (value.GetComState() == ComState.Def.WaitSend)
					{
						if (value.GetRetryCount() != 0)
						{
							sendPacketQueue.Enqueue(value.GetReq());
						}
						value.SetComState(ComState.Def.WaitRecv);
					}
				}
			}
		}

		public void AcceptRecv(Packet packet)
		{
			lock (Cs)
			{
				PacketAckHeader packetAckHeader = (PacketAckHeader)packet;
				byte key = ((packetAckHeader.status != 1 || packetAckHeader.report != 1) ? ExecCommandNo : packetAckHeader.command);
				if (Command.TryGetValue(key, out var value))
				{
					value.AcceptRecv(packet);
				}
			}
		}

		public void SetExecCommandNo(byte no)
		{
			lock (Cs)
			{
				ExecCommandNo = no;
			}
		}
	}
}
