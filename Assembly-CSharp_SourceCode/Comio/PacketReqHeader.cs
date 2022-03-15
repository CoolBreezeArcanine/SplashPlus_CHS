namespace Comio
{
	public class PacketReqHeader : PacketHeader
	{
		public new const int Size = 5;

		public byte command
		{
			get
			{
				return base[4];
			}
			set
			{
				base[4] = value;
			}
		}

		public PacketReqHeader()
			: base(5)
		{
		}

		public PacketReqHeader(int length)
			: base(length)
		{
		}
	}
}
