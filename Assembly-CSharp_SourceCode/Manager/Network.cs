using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using AMDaemon;
using MAI2.Util;
using MAI2System;

namespace Manager
{
	public class Network
	{
		private enum State
		{
			WaitInit,
			Ready
		}

		private State _state;

		private IPAddress _address = new IPAddress(0L);

		private IPAddress _subnetMask = new IPAddress(0L);

		private IPAddress _broadcastAdress = new IPAddress(0L);

		public bool isAvailable => _state == State.Ready;

		public IPAddress address => _address;

		public IPAddress subnetMask => _subnetMask;

		public IPAddress broadcastAddress => _broadcastAdress;

		public void initialize()
		{
			if (Singleton<SystemConfig>.Instance.config.IsUseNetwork)
			{
				NetworkProperty property = AMDaemon.Network.Property;
				IPAddress iPAddress = IPAddress.Parse(property.Address);
				IPAddress iPAddress2 = IPAddress.Parse(property.SubnetMask);
				_broadcastAdress = makeBroadcastAddress(iPAddress, iPAddress2);
			}
		}

		public void execute()
		{
			if (_state != 0)
			{
				_ = 1;
				return;
			}
			if (Singleton<SystemConfig>.Instance.config.IsUseNetwork)
			{
				if (AMDaemon.Network.IsLanAvailable)
				{
					NetworkProperty property = AMDaemon.Network.Property;
					_address = IPAddress.Parse(property.Address);
					_subnetMask = IPAddress.Parse(property.SubnetMask);
					_state = State.Ready;
				}
				return;
			}
			UnicastIPAddressInformation iPAddressInformtion = getIPAddressInformtion();
			if (iPAddressInformtion != null && iPAddressInformtion.Address.AddressFamily == AddressFamily.InterNetwork)
			{
				_address = iPAddressInformtion.Address;
				_subnetMask = iPAddressInformtion.IPv4Mask;
				_broadcastAdress = makeBroadcastAddress(_address, _subnetMask);
				_state = State.Ready;
			}
		}

		public void terminate()
		{
		}

		private UnicastIPAddressInformation getIPAddressInformtion()
		{
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in allNetworkInterfaces)
			{
				if (networkInterface.OperationalStatus != OperationalStatus.Up)
				{
					continue;
				}
				foreach (UnicastIPAddressInformation unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
				{
					if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
					{
						return unicastAddress;
					}
				}
			}
			return null;
		}

		private IPAddress makeBroadcastAddress(IPAddress address, IPAddress subnetMask)
		{
			byte[] addressBytes = address.GetAddressBytes();
			byte[] addressBytes2 = subnetMask.GetAddressBytes();
			if (addressBytes.Length != addressBytes2.Length)
			{
				return new IPAddress(0L);
			}
			byte[] array = new byte[addressBytes.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (byte)(addressBytes[i] | (addressBytes2[i] ^ 0xFFu));
			}
			return new IPAddress(array);
		}
	}
}
