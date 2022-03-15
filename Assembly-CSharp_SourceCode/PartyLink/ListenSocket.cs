using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace PartyLink
{
	public class ListenSocket : IDisposable
	{
		private NFSocket _socket;

		private ushort _portNumber;

		private string _socketName;

		private int _acceptCount;

		private bool _alreadyDisposed;

		public const int MockID = -1;

		private ListenSocket()
		{
			_socket = null;
			_portNumber = 0;
			_socketName = "";
			_acceptCount = 0;
		}

		public ListenSocket(string name, int mockID)
			: this()
		{
			_socketName = name;
			SocketInfo.listenSocket++;
		}

		~ListenSocket()
		{
			dispose(disposing: false);
		}

		public void Dispose()
		{
			dispose(disposing: true);
		}

		protected virtual void dispose(bool disposing)
		{
			if (!_alreadyDisposed)
			{
				if (disposing)
				{
					SocketInfo.listenSocket--;
					SocketBase.closeSocket(ref _socket, _socketName);
					GC.SuppressFinalize(this);
				}
				_alreadyDisposed = true;
			}
		}

		public bool isValid()
		{
			return _socket != null;
		}

		public bool open(ushort portNumber)
		{
			_ = _socket;
			_portNumber = portNumber;
			_acceptCount = 0;
			_socket = new NFSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, -1);
			if (_socket == null)
			{
				return false;
			}
			SocketInfo.create++;
			IPEndPoint iPEndPoint = new IPEndPoint(0L, 0);
			iPEndPoint.Port = portNumber;
			iPEndPoint.Address = IPAddress.Any;
			try
			{
				_socket.Bind(iPEndPoint);
			}
			catch (Exception ex)
			{
				error("bind failed " + ex.Message, Marshal.GetLastWin32Error());
				return false;
			}
			try
			{
				_socket.Listen(16);
			}
			catch (Exception ex2)
			{
				error("bind failed " + ex2.Message, Marshal.GetLastWin32Error());
				return false;
			}
			SocketInfo.listening++;
			return true;
		}

		public NFSocket acceptClient(out IpAddress outAddress)
		{
			outAddress = IpAddress.Any;
			if (!isValid())
			{
				return null;
			}
			NFSocket nFSocket = null;
			if (SocketBase.checkRecvEnable(_socket))
			{
				nFSocket = _socket.Accept();
				if (nFSocket != null)
				{
					SocketInfo.create++;
					_acceptCount++;
					if (nFSocket.RemoteEndPoint is IPEndPoint iPEndPoint)
					{
						outAddress = new IpAddress(iPEndPoint.Address);
						return nFSocket;
					}
					return null;
				}
			}
			return null;
		}

		public void close()
		{
			if (_socket != null)
			{
				SocketBase.closeSocket(ref _socket, _socketName);
			}
		}

		public void error(string message, int no)
		{
			close();
		}

		public override string ToString()
		{
			string text = "";
			return text + "isValid " + isValid().ToString() + " P " + _portNumber + " AC " + _acceptCount;
		}
	}
}
