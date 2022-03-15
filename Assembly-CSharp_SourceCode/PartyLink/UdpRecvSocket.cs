using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace PartyLink
{
	public class UdpRecvSocket : SocketBase
	{
		private uint _port;

		private IpAddress _lastRecvAddress;

		private IpAddress _address;

		private byte[] _recvTempBuffer;

		private UdpRecvSocket()
			: this("", -1)
		{
		}

		public UdpRecvSocket(string name, int mockID)
			: base(name, mockID)
		{
			_port = 0u;
			_lastRecvAddress = IpAddress.Zero;
			_address = IpAddress.Zero;
			_recvTempBuffer = new byte[SizeDef.CBufferSize];
		}

		~UdpRecvSocket()
		{
			dispose(disposing: false);
		}

		protected override void dispose(bool disposing)
		{
			base.dispose(disposing);
		}

		public void open(IpAddress addrTo, ushort port)
		{
			_port = port;
			_address = addrTo;
			IPEndPoint iPEndPoint = new IPEndPoint(0L, 0);
			iPEndPoint.Port = port;
			iPEndPoint.Address = new IPAddress(addrTo.GetAddressBytes());
			NFSocket nFSocket = new NFSocket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp, -1);
			initialize(nFSocket);
			try
			{
				nFSocket.Bind(iPEndPoint);
			}
			catch (Exception)
			{
				error("bind failed", Marshal.GetLastWin32Error());
			}
		}

		public override void recv()
		{
			if (_socket == null || !SocketBase.checkRecvEnable(_socket))
			{
				return;
			}
			EndPoint remoteEP = new IPEndPoint(0L, 0);
			int num = 0;
			try
			{
				num = _socket.ReceiveFrom(_recvTempBuffer, SocketFlags.None, ref remoteEP);
			}
			catch (Exception)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				error("recv failed unknown error", lastWin32Error);
				return;
			}
			if (remoteEP is IPEndPoint iPEndPoint)
			{
				_lastRecvAddress = new IpAddress(iPEndPoint.Address);
				if (num < 0)
				{
					close();
					return;
				}
				_recvBuffer.CopyRange(_recvTempBuffer, 0, num);
				_recvByte += (uint)num;
				_analyzer.analyze(_lastRecvAddress, getRecvBuffer(), _isCheckVersion);
				_ = _recvBuffer.Count;
				_ = 0;
				_recvBuffer.Clear();
			}
		}

		public IpAddress getLastRecvAddress()
		{
			return _lastRecvAddress;
		}

		public override string ToString()
		{
			string text = "";
			return text + "isValid " + isValid().ToString() + " A " + _address.convIpString() + " P " + _port + " R " + _recvByte;
		}
	}
}
