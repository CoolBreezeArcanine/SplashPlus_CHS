namespace Comio
{
	public class PacketHeader : Packet
	{
		public const byte Size = 4;

		public byte sync
		{
			get
			{
				return base[0];
			}
			set
			{
				base[0] = value;
			}
		}

		public byte dstNodeID
		{
			get
			{
				return base[1];
			}
			set
			{
				base[1] = value;
			}
		}

		public byte srcNodeID
		{
			get
			{
				return base[2];
			}
			set
			{
				base[2] = value;
			}
		}

		public byte length
		{
			get
			{
				return base[3];
			}
			set
			{
				base[3] = value;
			}
		}

		public PacketHeader()
		{
		}

		public PacketHeader(int length)
			: base(length)
		{
		}
	}
}
