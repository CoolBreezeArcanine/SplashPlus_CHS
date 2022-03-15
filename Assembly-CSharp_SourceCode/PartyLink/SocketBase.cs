using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace PartyLink
{
	public abstract class SocketBase : IDisposable
	{
		protected static readonly int c_recvBufferSize = 4096;

		protected string _socketName;

		protected NFSocket _socket;

		protected BufferType _recvBuffer;

		protected uint _recvByte;

		protected uint _sendByte;

		protected Analyzer _analyzer;

		protected Packet _packet;

		protected bool _isError;

		protected int _errorNo;

		protected string _errorMessage;

		protected bool _isCheckVersion;

		protected bool _alreadyDisposed;

		public const int MockID = -1;

		public static void closeSocket(ref NFSocket pSock, string socketName)
		{
			if (pSock != null)
			{
				SocketInfo.close++;
				pSock.Close();
				pSock = null;
			}
		}

		public static bool checkRecvEnable(NFSocket socket)
		{
			try
			{
				return NFSocket.Poll(socket, SelectMode.SelectRead);
			}
			catch
			{
			}
			return false;
		}

		public static bool checkSendEnable(NFSocket socket)
		{
			try
			{
				return NFSocket.Poll(socket, SelectMode.SelectWrite);
			}
			catch
			{
			}
			return false;
		}

		public SocketBase(string name, int mockID)
		{
			_socket = null;
			_socketName = name;
			_recvBuffer = new BufferType(c_recvBufferSize);
			_recvByte = 0u;
			_sendByte = 0u;
			_analyzer = new Analyzer();
			_packet = new Packet();
			_isError = false;
			_errorNo = 0;
			_errorMessage = string.Empty;
			_isCheckVersion = true;
			SocketInfo.socket++;
		}

		~SocketBase()
		{
			dispose(disposing: false);
		}

		public virtual void Dispose()
		{
			dispose(disposing: true);
		}

		protected virtual void dispose(bool disposing)
		{
			if (!_alreadyDisposed)
			{
				if (disposing)
				{
					SocketInfo.socket--;
					close();
					GC.SuppressFinalize(this);
				}
				_alreadyDisposed = true;
			}
		}

		public virtual void close()
		{
			_recvBuffer.Clear();
			closeSocket(ref _socket, _socketName);
		}

		public void initialize(NFSocket socket)
		{
			if (socket != null)
			{
				_ = _socket;
			}
			_socket = socket;
			_recvBuffer.Clear();
			_isError = false;
			_errorMessage = "";
			_errorNo = 0;
		}

		public void sendClass(ICommandParam info)
		{
			_packet.encode(info);
			sendPacket(_packet);
		}

		public void setCheckVersion(bool isCheckVersion)
		{
			_isCheckVersion = isCheckVersion;
		}

		public bool isCheckVersion()
		{
			return _isCheckVersion;
		}

		public virtual void send(PacketType data)
		{
			if (_socket == null)
			{
				error("send failed null", 0);
				return;
			}
			if (!checkSendEnable(_socket))
			{
				error("send failed checkSendEnable", 0);
				return;
			}
			byte[] buffer = data.GetBuffer();
			int num = 0;
			int num2 = data.Count;
			while (true)
			{
				try
				{
					int num3 = _socket.Send(buffer, num, num2, SocketFlags.None);
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (num3 <= 0)
					{
						error("send failed Send Error", lastWin32Error);
						break;
					}
					if (num2 <= num3)
					{
						_sendByte += (uint)num3;
						break;
					}
					num += num3;
					num2 -= num3;
					_sendByte += (uint)num3;
				}
				catch (Exception ex)
				{
					error("send failed Send Error " + ex.Message, 0);
					break;
				}
			}
		}

		public abstract void recv();

		public void sendPacket(Packet packet)
		{
			send(packet.getEncrypt());
		}

		public void registCommand(Command command, CallBackFunction callback)
		{
			_analyzer.registCommand(command, callback);
		}

		public bool isRegisted(Command command)
		{
			return _analyzer.isRegisted(command);
		}

		public BufferType getRecvBuffer()
		{
			return _recvBuffer;
		}

		public bool isValid()
		{
			return _socket != null;
		}

		public string getSocketName()
		{
			return _socketName;
		}

		public uint getRecvCount(Command command)
		{
			return _analyzer.getRecvCount(command);
		}

		public bool isError()
		{
			return _isError;
		}

		protected void error(string message, int no)
		{
			_isError = true;
			_errorMessage = message;
			_errorNo = no;
			close();
		}
	}
}
