namespace PartyLink
{
	public static class IpAddressHelper
	{
		public static string convIpString(this uint val)
		{
			return ((val >> 24) & 0xFFu) + "." + ((val >> 16) & 0xFFu) + "." + ((val >> 8) & 0xFFu) + "." + (val & 0xFFu);
		}

		public static string convIpString(this IpAddress ipAddress)
		{
			byte[] addressBytes = ipAddress.GetAddressBytes();
			if (addressBytes.Length != 4)
			{
				return "0.0.0.0";
			}
			return addressBytes[0] + "." + addressBytes[1] + "." + addressBytes[2] + "." + addressBytes[3];
		}
	}
}
