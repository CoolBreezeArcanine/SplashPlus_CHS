namespace Net.Packet
{
	public enum PacketState
	{
		Ready,
		Process,
		Done,
		RetryWait,
		Dialog,
		Error
	}
}
