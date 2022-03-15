using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using DB;
using RD1.SSS;
using UnityEngine;

namespace PartyLink
{
	public class ConnectSocket : SocketBase
	{
		private IPEndPoint _socketAddr;

		private bool _isAccept;

		private IpAddress _myAddress;

		private IpAddress _destAddress;

		private bool _isRecvEnd;

		private SocketAsyncEventArgs _socketConnectAsyncArg;

		private byte[] _recvTempBuffer;

		private bool _connectDone;

		private StateMachine<ConnectSocket, PartyConnectStateID> _stateMachine;

		public string stateString => _stateMachine.getStateName();

		public override void recv()
		{
			if (_socket == null || !SocketBase.checkRecvEnable(_socket))
			{
				return;
			}
			SocketError errorCode;
			int num = _socket.Receive(_recvTempBuffer, 0, _recvTempBuffer.Length, SocketFlags.None, out errorCode);
			switch (errorCode)
			{
			case SocketError.ConnectionAborted:
			case SocketError.ConnectionReset:
				close();
				break;
			default:
				error(string.Concat("recv failed unknown error(errorCode=", errorCode, ")"), Marshal.GetLastWin32Error());
				break;
			case SocketError.Success:
				if (num == 0)
				{
					if (!_isRecvEnd)
					{
						_isRecvEnd = true;
					}
				}
				else if (num < 0)
				{
					close();
				}
				else
				{
					_recvBuffer.CopyRange(_recvTempBuffer, 0, num);
					_recvByte += (uint)num;
					_analyzer.analyze(getDestinationAddress(), getRecvBuffer(), _isCheckVersion);
				}
				break;
			}
		}

		private ConnectSocket()
			: this("", -1)
		{
		}

		public ConnectSocket(string name, NFSocket socket)
			: base(name, -1)
		{
			StateMachine_ctor();
			_isAccept = true;
			_myAddress = IpAddress.Zero;
			_destAddress = IpAddress.Zero;
			_isRecvEnd = false;
			_socketConnectAsyncArg = new SocketAsyncEventArgs();
			_socketConnectAsyncArg.Completed += ConnectCompletedEvent;
			_recvTempBuffer = new byte[SizeDef.CBufferSize];
			_connectDone = false;
			initialize(socket);
			SocketInfo.acceptSocket++;
			_socketAddr = new IPEndPoint(0L, 0);
		}

		public ConnectSocket(string name, int mockID)
			: base(name, mockID)
		{
			StateMachine_ctor();
			_isAccept = false;
			_myAddress = IpAddress.Zero;
			_destAddress = IpAddress.Zero;
			_isRecvEnd = false;
			_socketConnectAsyncArg = new SocketAsyncEventArgs();
			_socketConnectAsyncArg.Completed += ConnectCompletedEvent;
			_recvTempBuffer = new byte[SizeDef.CBufferSize];
			_connectDone = false;
			SocketInfo.connectSocket++;
			_socketAddr = new IPEndPoint(0L, 0);
			setCurrentStateID(PartyConnectStateID.Close);
		}

		~ConnectSocket()
		{
			dispose(disposing: false);
		}

		protected override void dispose(bool disposing)
		{
			if (_alreadyDisposed)
			{
				return;
			}
			if (disposing)
			{
				if (_isAccept)
				{
					SocketInfo.acceptSocket--;
				}
				else
				{
					SocketInfo.connectSocket--;
				}
				Util.SafeDispose(ref _socketConnectAsyncArg);
			}
			base.dispose(disposing);
		}

		public void update()
		{
			updateState();
		}

		public void active()
		{
			if (_socket != null)
			{
				setCurrentStateID(PartyConnectStateID.Active);
			}
		}

		public void connect(IpAddress address, ushort portNumber)
		{
			_isRecvEnd = false;
			_ = _socket;
			_socketAddr = new IPEndPoint(0L, 0);
			_socketAddr.Port = portNumber;
			_socketAddr.Address = new IPAddress(address.GetAddressBytes());
			setCurrentStateID(PartyConnectStateID.Connect);
		}

		public void shutdown()
		{
			if (_socket != null)
			{
				try
				{
					_socket.Shutdown(SocketShutdown.Send);
				}
				catch (Exception)
				{
					error("shutdown error ", Marshal.GetLastWin32Error());
				}
			}
			setCurrentStateID(PartyConnectStateID.Shutdown);
		}

		public override void close()
		{
			setCurrentStateID(PartyConnectStateID.Close);
		}

		public bool isRecvEnd()
		{
			return _isRecvEnd;
		}

		public bool isActive()
		{
			return getCurrentState() == PartyConnectStateID.Active;
		}

		public bool isClose()
		{
			return getCurrentState() == PartyConnectStateID.Close;
		}

		public IpAddress getMyAddress()
		{
			return _myAddress;
		}

		public IpAddress getDestinationAddress()
		{
			return _destAddress;
		}

		public override string ToString()
		{
			string text = "";
			text += stateString;
			return text + " S " + _sendByte + " R " + _recvByte;
		}

		public void Enter_Close()
		{
			base.close();
		}

		public void Execute_Close()
		{
		}

		public void Enter_Connect()
		{
			_ = _socket;
			NFSocket socket = new NFSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, -1);
			initialize(socket);
			SocketInfo.create++;
			_socketConnectAsyncArg.RemoteEndPoint = _socketAddr;
			_connectDone = false;
			try
			{
				if (!_socket.ConnectAsync(_socketConnectAsyncArg, -1))
				{
					ConnectCompletedEvent(this, _socketConnectAsyncArg);
				}
			}
			catch (Exception ex)
			{
				error("ConnectSocket.ConnectAsync: " + ex.Message, 0);
			}
		}

		private void ConnectCompletedEvent(object sender, SocketAsyncEventArgs e)
		{
			_connectDone = true;
		}

		public void Execute_Connect()
		{
			if (_connectDone)
			{
				setCurrentStateID(PartyConnectStateID.Active);
			}
		}

		public void Enter_Active()
		{
			try
			{
				if (!(_socket.RemoteEndPoint is IPEndPoint iPEndPoint))
				{
					error("failed getpeername", Marshal.GetLastWin32Error());
				}
				else
				{
					_destAddress = new IpAddress(iPEndPoint.Address);
				}
			}
			catch (Exception ex)
			{
				error("RemoteEndPoint failed " + ex.Message, Marshal.GetLastWin32Error());
				return;
			}
			try
			{
				IPEndPoint iPEndPoint2 = _socket.LocalEndPoint as IPEndPoint;
				if (iPEndPoint2 == null)
				{
					error("failed getsockname", Marshal.GetLastWin32Error());
				}
				_myAddress = new IpAddress(iPEndPoint2.Address);
			}
			catch (Exception ex2)
			{
				error("LocalEndPoint failed " + ex2.Message, Marshal.GetLastWin32Error());
			}
		}

		public void Execute_Active()
		{
			recv();
		}

		public void Execute_Shutdown()
		{
		}

		public void Leave_Shutdown()
		{
			recv();
			if (_isRecvEnd)
			{
				close();
			}
		}

		public void Execute_End()
		{
		}

		public void Execute_Begin()
		{
		}

		public void Execute_Invalid()
		{
		}

		private void StateMachine_ctor()
		{
			_stateMachine = new StateMachine<ConnectSocket, PartyConnectStateID>(this, "_");
			updateStateString();
		}

		public PartyConnectStateID getCurrentState()
		{
			return _stateMachine.State;
		}

		public PartyConnectStateID getStatus()
		{
			return getCurrentState();
		}

		public bool isStateEnd()
		{
			return _stateMachine.IsExited;
		}

		public void addChildState<AAA>(PartyConnectStateID state) where AAA : IStateMachine, new()
		{
			_stateMachine.addChildState<AAA>(state);
		}

		public bool isChildStateEnd()
		{
			return _stateMachine.isChildStateEnd;
		}

		public void setNextState(PartyConnectStateID state)
		{
			_stateMachine.GoNext(state);
		}

		public void setStateEnd()
		{
			_stateMachine.GoExit();
		}

		public bool updateState(float deltaTime = -1f)
		{
			if (deltaTime < 0f)
			{
				deltaTime = Time.deltaTime;
			}
			PartyConnectStateID currentState = getCurrentState();
			bool result = _stateMachine.updateState(deltaTime);
			PartyConnectStateID currentState2 = getCurrentState();
			if (!currentState.Equals(currentState2))
			{
				updateStateString();
			}
			return result;
		}

		private void updateStateString()
		{
		}

		private void setCurrentStateID(PartyConnectStateID nextState)
		{
			setNextState(nextState);
			updateState();
		}
	}
}
