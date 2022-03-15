namespace Net.Packet
{
	public enum PacketStatus
	{
		Ok = 0,
		ErrorCreate = -1,
		ErrorEcodeRequest = -2,
		ErrorDecodeResponse = -3,
		ErrorTimeout = -4,
		ErrorConnection = -5,
		ErrorInternal = -6,
		ErrorHttpStatus = -7
	}
}
