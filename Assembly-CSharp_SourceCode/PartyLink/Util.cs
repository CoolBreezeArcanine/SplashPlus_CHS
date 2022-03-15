using System;
using System.Collections.Generic;
using System.Net;
using MAI2.Util;
using Manager;

namespace PartyLink
{
	public static class Util
	{
		public static IPAddress MyIpAddress()
		{
			return MyIpAddress(0);
		}

		public static IPAddress MyIpAddress(int mockID)
		{
			return SingletonStateMachine<AmManager, AmManager.EState>.Instance.Network.address;
		}

		public static IPAddress BroadcastAddress()
		{
			return SingletonStateMachine<AmManager, AmManager.EState>.Instance.Network.broadcastAddress;
		}

		public static IPAddress LoopbackAddress()
		{
			return LoopbackAddress(0);
		}

		public static IPAddress LoopbackAddress(int mockID)
		{
			return IPAddress.Loopback;
		}

		public static bool isMyIP(IPAddress ip)
		{
			return isMyIP(ip.ToNetworkByteOrderU32(), 0);
		}

		public static bool isMyIP(IPAddress ip, int mockID)
		{
			return isMyIP(ip.ToNetworkByteOrderU32(), mockID);
		}

		public static bool isMyIP(IpAddress ip)
		{
			return isMyIP(ip.ToNetworkByteOrderU32(), 0);
		}

		public static bool isMyIP(IpAddress ip, int mockID)
		{
			return isMyIP(ip.ToNetworkByteOrderU32(), mockID);
		}

		public static bool isMyIP(uint ip)
		{
			return isMyIP(ip, 0);
		}

		public static bool isMyIP(uint ip, int mockID)
		{
			if (ip != 0)
			{
				return ip == MyIpAddress(mockID).ToNetworkByteOrderU32();
			}
			return false;
		}

		public static void SafeDispose<T>(ref T target) where T : class, IDisposable
		{
			if (target != null)
			{
				target.Dispose();
				target = null;
			}
		}

		public static void SafeDispose<T>(ICollection<T> targetAry) where T : IDisposable
		{
			if (targetAry == null)
			{
				return;
			}
			foreach (T item in targetAry)
			{
				item?.Dispose();
			}
			targetAry.Clear();
		}

		public static uint ToNetworkByteOrderU32(this IPAddress ip)
		{
			byte[] addressBytes = ip.GetAddressBytes();
			if (addressBytes != null && addressBytes.Length == 4)
			{
				return (uint)((addressBytes[0] << 24) | (addressBytes[1] << 16) | (addressBytes[2] << 8) | addressBytes[3]);
			}
			return 0u;
		}
	}
}
