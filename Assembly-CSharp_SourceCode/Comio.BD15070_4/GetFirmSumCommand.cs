namespace Comio.BD15070_4
{
	public class GetFirmSumCommand : CommandBase
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
			public new const int Size = 10;

			public byte sum_upper
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

			public byte sum_lower
			{
				get
				{
					return base[8];
				}
				set
				{
					base[8] = value;
				}
			}

			public byte sum
			{
				get
				{
					return base[9];
				}
				set
				{
					base[9] = value;
				}
			}
		}

		public ushort _sum;

		public override byte GetCommandNo()
		{
			return 242;
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

		public GetFirmSumCommand()
		{
			_sum = 0;
		}

		public override void DoRecv()
		{
			if (AckPacket.Count == 10)
			{
				Ack ack = (Ack)AckPacket;
				_sum = (ushort)(ack.sum_upper << 8);
				_sum |= ack.sum_lower;
			}
		}

		public ushort getSum()
		{
			return _sum;
		}
	}
}
