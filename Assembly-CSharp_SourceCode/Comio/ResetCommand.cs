namespace Comio
{
	public class ResetCommand : CommandBase
	{
		public class Req : PacketReqHeader
		{
			public new const int Size = 6;

			public byte sum
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

			public Req()
				: base(6)
			{
			}
		}

		public class Ack : PacketAckHeader
		{
			public new const int Size = 8;

			public byte sum
			{
				get
				{
					return base[7];
				}
				set
				{
					base[7] = value;
				}
			}
		}

		public override byte GetCommandNo()
		{
			return 16;
		}

		public override byte GetLength()
		{
			return 6;
		}

		public override PacketReqHeader CreateReq()
		{
			return new Req();
		}

		public override PacketAckHeader CreateAck()
		{
			return new Ack();
		}

		public override void DoRecv()
		{
		}

		public void set()
		{
		}
	}
}
