namespace Comio
{
	public class PacketAckHeader : PacketHeader
	{
		public new const int Size = 7;

		public byte status
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

		public byte command
		{
			get
			{
				return base[5];
			}
			set
			{
				base[5] = value;
			}
		}

		public byte report
		{
			get
			{
				return base[6];
			}
			set
			{
				base[6] = value;
			}
		}

		public PacketAckHeader()
		{
		}

		public PacketAckHeader(int length)
			: base(length)
		{
		}
	}
}
