using System.Net;

namespace PartyLink
{
	public struct IpAddress
	{
		public static readonly IpAddress Zero = new IpAddress(new IPAddress(0L));

		public static readonly IpAddress Any = new IpAddress(IPAddress.Any);

		private uint _value;

		public IpAddress(IPAddress ipAddr)
		{
			_value = 0u;
			if (ipAddr != null)
			{
				byte[] addressBytes = ipAddr.GetAddressBytes();
				if (addressBytes != null && addressBytes.Length == 4)
				{
					_value = (uint)((addressBytes[0] << 24) | (addressBytes[1] << 16) | (addressBytes[2] << 8) | addressBytes[3]);
				}
			}
		}

		public IpAddress(uint value)
		{
			_value = value;
		}

		public IpAddress(IpAddress arg)
		{
			_value = arg._value;
		}

		public static bool operator ==(IpAddress a, IpAddress b)
		{
			return a._value == b._value;
		}

		public static bool operator !=(IpAddress a, IpAddress b)
		{
			return !(a == b);
		}

		public override bool Equals(object arg)
		{
			if (arg.GetType() == typeof(IpAddress))
			{
				return (IpAddress)arg == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)_value;
		}

		public override string ToString()
		{
			return this.convIpString();
		}

		public byte[] GetAddressBytes()
		{
			return new byte[4]
			{
				(byte)((_value >> 24) & 0xFFu),
				(byte)((_value >> 16) & 0xFFu),
				(byte)((_value >> 8) & 0xFFu),
				(byte)(_value & 0xFFu)
			};
		}

		public uint ToNetworkByteOrderU32()
		{
			return _value;
		}
	}
}
