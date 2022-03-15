using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace Comio
{
	public class Host
	{
		private class BoardMap : Dictionary<uint, BoardBase>
		{
		}

		public class InitParam
		{
			public string PortName;

			public bool AsyncIo;

			public ComParam ComParam;

			public int Interval;

			public InitParam()
			{
				PortName = "COM1";
				AsyncIo = true;
				ComParam = new ComParam
				{
					BaudRate = 115200,
					Parity = Parity.None,
					DataBits = 8,
					StopBits = StopBits.One,
					DtrEnable = false,
					RtsEnable = false,
					ReadTimeout = 1,
					WriteTimeout = 1,
					Handshake = Handshake.None
				};
				Interval = 16;
			}
		}

		private enum Mode
		{
			Init,
			StartThread,
			Exec
		}

		private const uint SumErrorCountMax = 3u;

		private Mode _mode;

		private InitParam _initParam;

		private Thread _thread;

		private volatile bool _threadExit;

		private bool _initialized;

		private SerialPort _port;

		private BoardMap _boardMap;

		private BoardCtrlBase.PacketQueue _reqPacketQueue;

		private BoardCtrlBase.PacketQueue _sendPacketQueue;

		private BoardCtrlBase.PacketQueue _recvPacketQueue;

		private int _writeBufferIndex;

		private Packet _writeBuffer;

		private Packet _readBuffer;

		private byte[] _readBufferTmp;

		private int _sumErrorCount;

		private Stopwatch _waitTimer;

		[Conditional("APP_DEBUG")]
		private void log(string message)
		{
			ComioLog.Log(message);
		}

		public Host()
		{
			InitParam initParam = new InitParam();
			_construct(initParam);
		}

		public Host(InitParam initParam)
		{
			_construct(initParam);
		}

		public bool Initialize()
		{
			return _initialize();
		}

		public void Terminate()
		{
			_terminate();
		}

		public bool IsOpened()
		{
			return _isOpened();
		}

		public bool RegisterBoard(BoardBase board)
		{
			return _registerBoard(board);
		}

		public bool ReqHaltBoard()
		{
			return _reqHaltBoard();
		}

		public void Execute()
		{
			_execute();
		}

		private void _construct(InitParam initParam)
		{
			_mode = Mode.Init;
			_initParam = initParam;
			_thread = null;
			_threadExit = false;
			_initialized = false;
			_port = null;
			_boardMap = new BoardMap();
			_reqPacketQueue = new BoardCtrlBase.PacketQueue();
			_sendPacketQueue = new BoardCtrlBase.PacketQueue();
			_recvPacketQueue = new BoardCtrlBase.PacketQueue();
			_writeBufferIndex = 0;
			_writeBuffer = new Packet();
			_readBuffer = new Packet();
			_readBufferTmp = new byte[1024];
			_sumErrorCount = 0;
			_waitTimer = new Stopwatch();
		}

		private bool _initialize()
		{
			_finishThread();
			_mode = Mode.Init;
			_port = new SerialPort(_initParam.PortName, _initParam.ComParam.BaudRate, _initParam.ComParam.Parity, _initParam.ComParam.DataBits, _initParam.ComParam.StopBits)
			{
				DtrEnable = _initParam.ComParam.DtrEnable,
				RtsEnable = _initParam.ComParam.RtsEnable,
				ReadTimeout = _initParam.ComParam.ReadTimeout,
				WriteTimeout = _initParam.ComParam.WriteTimeout,
				Handshake = _initParam.ComParam.Handshake
			};
			try
			{
				_port.Open();
			}
			catch (Exception)
			{
			}
			_reqPacketQueue.Clear();
			_sendPacketQueue.Clear();
			_recvPacketQueue.Clear();
			_boardMap.Clear();
			_sumErrorCount = 0;
			_initialized = _port.IsOpen;
			return _initialized;
		}

		private void _execute()
		{
			switch (_mode)
			{
			case Mode.Init:
				if (_initialized)
				{
					_mode = Mode.StartThread;
				}
				break;
			case Mode.StartThread:
				if (_startThread())
				{
					_mode = Mode.Exec;
				}
				break;
			case Mode.Exec:
				if (_initialized)
				{
					_execBoard();
				}
				else
				{
					_mode = Mode.Init;
				}
				break;
			}
		}

		private void _terminate()
		{
			foreach (KeyValuePair<uint, BoardBase> item in _boardMap)
			{
				item.Value.GetCtrlBase().Terminate();
			}
			_finishThread();
			if (_port != null && _port.IsOpen)
			{
				_port.Close();
			}
			_initialized = false;
		}

		private bool _isOpened()
		{
			return _port.IsOpen;
		}

		private bool _startThread()
		{
			if (_thread == null)
			{
				if (_initParam.AsyncIo)
				{
					throw new NotImplementedException();
				}
				_thread = new Thread(_execSync);
			}
			if (!_thread.IsAlive)
			{
				_threadExit = false;
				_thread.Start();
			}
			return true;
		}

		private void _finishThread()
		{
			if (_thread != null && _thread.IsAlive)
			{
				_threadExit = true;
				_thread.Join();
			}
		}

		private bool _registerBoard(BoardBase board)
		{
			uint boardNodeId = board.GetBoardNodeId();
			if (_boardMap.ContainsKey(boardNodeId))
			{
				return false;
			}
			_boardMap[boardNodeId] = board;
			board.GetCtrlBase().Initialize();
			return true;
		}

		private bool _reqHaltBoard()
		{
			bool flag = true;
			foreach (KeyValuePair<uint, BoardBase> item in _boardMap)
			{
				item.Value.ReqHalt();
				flag = flag && item.Value.IsHalted();
			}
			return flag;
		}

		private void _updateCommand()
		{
			foreach (KeyValuePair<uint, BoardBase> item in _boardMap)
			{
				BoardCtrlBase.PacketQueue packetQueue = new BoardCtrlBase.PacketQueue();
				item.Value.GetCtrlBase().UpdateCommand(packetQueue);
				while (packetQueue.Count > 0)
				{
					_reqPacketQueue.Enqueue(packetQueue.Dequeue());
				}
			}
		}

		private void _execBoard()
		{
			foreach (KeyValuePair<uint, BoardBase> item in _boardMap)
			{
				item.Value.GetCtrlBase().Execute();
			}
		}

		private void _execThreadBoard()
		{
			foreach (KeyValuePair<uint, BoardBase> item in _boardMap)
			{
				item.Value.GetCtrlBase().ExecThread();
			}
		}

		private void _setExecCommandNo(Packet plain)
		{
			PacketReqHeader packetReqHeader = (PacketReqHeader)plain;
			uint dstNodeID = packetReqHeader.dstNodeID;
			if (_boardMap.TryGetValue(dstNodeID, out var value))
			{
				value.GetCtrlBase().SetExecCommandNo(packetReqHeader.command);
			}
		}

		private void _recv()
		{
			try
			{
				int num = _port.Read(_readBufferTmp, 0, _readBufferTmp.Length);
				if (num != 0)
				{
					for (uint num2 = 0u; num2 < num; num2++)
					{
						_readBuffer.Add(_readBufferTmp[num2]);
					}
				}
			}
			catch (Exception)
			{
			}
			_decodeRecvData();
		}

		private void _send()
		{
			while (_reqPacketQueue.Count > 0)
			{
				_sendPacketQueue.Enqueue(_reqPacketQueue.Dequeue());
			}
			if (_sendPacketQueue.Empty())
			{
				return;
			}
			if (_writeBuffer.Count == 0)
			{
				JvsSyncProtect.SetSum(_sendPacketQueue.Peek());
				if (!JvsSyncProtect.Encode(_writeBuffer, _sendPacketQueue.Peek()))
				{
					_writeBufferIndex = 0;
					_writeBuffer.Clear();
				}
			}
			if (_writeBufferIndex < _writeBuffer.Count)
			{
				int num = _writeBuffer.Count - _writeBufferIndex;
				_port.Write(_writeBuffer.ToArray(), _writeBufferIndex, num);
				_writeBufferIndex += num;
				if (_writeBufferIndex >= _writeBuffer.Count)
				{
					_setExecCommandNo(_sendPacketQueue.Peek());
					_sendPacketQueue.Dequeue();
					_writeBufferIndex = 0;
					_writeBuffer.Clear();
				}
			}
			else
			{
				_writeBufferIndex = 0;
				_writeBuffer.Clear();
			}
		}

		private void _execSync()
		{
			_waitTimer.Start();
			while (!_threadExit && _port != null && _port.IsOpen)
			{
				try
				{
					_updateCommand();
					_recv();
					_execThreadBoard();
					_send();
				}
				catch (Exception)
				{
				}
				_waitTimer.Reset();
				_waitTimer.Start();
			}
		}

		private void _decodeRecvData()
		{
			if (_readBuffer.Count == 0)
			{
				return;
			}
			bool flag = false;
			while (true)
			{
				if (flag)
				{
					_readBuffer[0] = 31;
				}
				bool flag2 = false;
				if (_readBuffer[0] == 224)
				{
					flag2 = true;
				}
				else
				{
					for (int i = 1; i < _readBuffer.Count; i++)
					{
						if (_readBuffer[i] == 224)
						{
							flag2 = true;
							_readBuffer.RemoveRange(0, i);
							break;
						}
					}
					if (!flag2)
					{
						_readBuffer.Clear();
					}
				}
				if (!flag2)
				{
					break;
				}
				PacketAckHeader packetAckHeader = new PacketAckHeader();
				if (!JvsSyncProtect.Decode(packetAckHeader, _readBuffer, out var sumError, out var middleSync))
				{
					if (!middleSync)
					{
						break;
					}
					flag = true;
				}
				else if (sumError)
				{
					if ((long)(++_sumErrorCount) >= 3L)
					{
						foreach (KeyValuePair<uint, BoardBase> item in _boardMap)
						{
							item.Value.GetCtrlBase().NotifyBaseError(BoardCtrlBase.BaseErrorNo.BaseErrorNoSumError);
						}
					}
					flag = true;
				}
				else if (packetAckHeader.srcNodeID == 1)
				{
					flag = true;
				}
				else
				{
					uint srcNodeID = packetAckHeader.srcNodeID;
					if (_boardMap.TryGetValue(srcNodeID, out var value))
					{
						value.GetCtrlBase().AcceptRecv(packetAckHeader);
					}
					_sumErrorCount = 0;
					flag = true;
				}
			}
		}
	}
}
