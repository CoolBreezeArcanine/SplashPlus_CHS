using System.Net;
using System.Net.Sockets;

namespace PartyLink
{
	public class NFSocket
	{
		private Socket _nfSocket;

		public EndPoint RemoteEndPoint
		{
			get
			{
				if (_nfSocket != null)
				{
					return _nfSocket.RemoteEndPoint as IPEndPoint;
				}
				return null;
			}
		}

		public EndPoint LocalEndPoint
		{
			get
			{
				if (_nfSocket != null)
				{
					return _nfSocket.LocalEndPoint as IPEndPoint;
				}
				return null;
			}
		}

		private NFSocket()
		{
		}

		private NFSocket(Socket nfSocket)
			: this()
		{
			_nfSocket = nfSocket;
		}

		public NFSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, int mockID)
			: this()
		{
			_nfSocket = new Socket(addressFamily, socketType, protocolType);
		}

		public static bool Poll(NFSocket socket, SelectMode mode)
		{
			return socket._nfSocket.Poll(0, mode);
		}

		public void Listen(int backlog)
		{
			if (_nfSocket != null)
			{
				_nfSocket.Listen(backlog);
			}
		}

		public bool ConnectAsync(SocketAsyncEventArgs e, int mockID)
		{
			if (_nfSocket != null)
			{
				return _nfSocket.ConnectAsync(e);
			}
			return false;
		}

		public NFSocket Accept()
		{
			Socket nfSocket = null;
			if (_nfSocket != null)
			{
				nfSocket = _nfSocket.Accept();
			}
			return new NFSocket(nfSocket);
		}

		public void Bind(EndPoint localEndP)
		{
			if (_nfSocket != null)
			{
				_nfSocket.Bind(localEndP);
			}
		}

		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, bool optionValue)
		{
			if (_nfSocket != null)
			{
				_nfSocket.SetSocketOption(optionLevel, optionName, optionValue);
			}
		}

		public int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags)
		{
			if (_nfSocket != null)
			{
				return _nfSocket.Send(buffer, offset, size, socketFlags);
			}
			return 0;
		}

		public int SendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP)
		{
			if (_nfSocket != null)
			{
				try
				{
					return _nfSocket.SendTo(buffer, offset, size, socketFlags, remoteEP);
				}
				catch
				{
				}
			}
			return 0;
		}

		public int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode)
		{
			if (_nfSocket != null)
			{
				return _nfSocket.Receive(buffer, offset, size, socketFlags, out errorCode);
			}
			errorCode = SocketError.NotSocket;
			return 0;
		}

		public int ReceiveFrom(byte[] buffer, SocketFlags socketFlags, ref EndPoint remoteEP)
		{
			if (_nfSocket != null)
			{
				return _nfSocket.ReceiveFrom(buffer, socketFlags, ref remoteEP);
			}
			return 0;
		}

		public void Close()
		{
			if (_nfSocket != null)
			{
				_nfSocket.Close();
			}
		}

		public void Shutdown(SocketShutdown how)
		{
			if (_nfSocket != null)
			{
				_nfSocket.Shutdown(how);
			}
		}
	}
}
