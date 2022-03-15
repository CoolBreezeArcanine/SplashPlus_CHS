using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace PartyLink
{
	public class BroadcastSocket : SocketBase
	{
		private IPEndPoint _broadcastAddress;

		private string _addressString;

		private ushort _port;

		public override void recv()
		{
		}

		private BroadcastSocket()
			: this("", -1)
		{
		}

		public BroadcastSocket(string name, int mockID)
			: base(name, mockID)
		{
			clear();
		}

		~BroadcastSocket()
		{
			dispose(disposing: false);
		}

		protected override void dispose(bool disposing)
		{
			base.dispose(disposing);
		}

		public void clear()
		{
			_port = 0;
			_broadcastAddress = new IPEndPoint(0L, 0);
			_addressString = "";
		}

		public bool open(IpAddress addrTo, ushort port)
		{
			_ = _socket;
			_addressString = addrTo.convIpString();
			_port = port;
			_broadcastAddress = new IPEndPoint(0L, 0);
			_broadcastAddress.Port = port;
			_broadcastAddress.Address = new IPAddress(addrTo.GetAddressBytes());
			NFSocket nFSocket = new NFSocket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp, -1);
			initialize(nFSocket);
			bool optionValue = true;
			try
			{
				nFSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, optionValue);
			}
			catch (Exception ex)
			{
				error("setsockopt failed" + ex.Message, Marshal.GetLastWin32Error());
				return false;
			}
			return true;
		}

		public override void send(PacketType data)
		{
			send(_broadcastAddress, data);
		}

		public void send(IPEndPoint addrTo, PacketType data)
		{
			if (_socket == null)
			{
				error("send failed null", 0);
				return;
			}
			_ = data.Count;
			_ = SizeDef.CUdpMtuSize;
			if (!SocketBase.checkSendEnable(_socket))
			{
				error("sendTo failed checkSendEnable", 0);
				return;
			}
			_socket.SendTo(data.GetBuffer(), 0, data.Count, SocketFlags.None, addrTo);
			_ = data.Count;
			_sendByte += (uint)data.Count;
		}

		public void sendTo(IpAddress addrTo, PacketType data)
		{
			IPEndPoint iPEndPoint = new IPEndPoint(0L, 0);
			iPEndPoint.Port = _port;
			iPEndPoint.Address = new IPAddress(addrTo.GetAddressBytes());
			send(iPEndPoint, data);
		}

		public void sendToPacket(IpAddress addrTo, Packet packet)
		{
			sendTo(addrTo, packet.getEncrypt());
		}

		public void sendToClass(IpAddress addrTo, ICommandParam info)
		{
			_packet.encode(info);
			sendToPacket(addrTo, _packet);
		}

		public override string ToString()
		{
			string text = "";
			text = text + " isValid " + isValid().ToString() + " " + _addressString + ":" + _port;
			return text + " S " + _sendByte;
		}
	}
}
