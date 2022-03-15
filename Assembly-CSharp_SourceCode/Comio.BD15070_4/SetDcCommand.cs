namespace Comio.BD15070_4
{
	public class SetDcCommand : CommandBase
	{
		public class Req : PacketReqHeader
		{
			public new const int Size = 12;

			public byte sum
			{
				get
				{
					return base[11];
				}
				set
				{
					base[11] = value;
				}
			}

			public Req()
				: base(12)
			{
			}

			public void setDc(Gs8BitMulti info)
			{
				base[5] = info.Start;
				base[6] = info.End;
				base[7] = info.Skip;
				base[8] = (byte)(info.Color.r & 0x3Fu);
				base[9] = (byte)(info.Color.g & 0x3Fu);
				base[10] = (byte)(info.Color.b & 0x3Fu);
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
			return 63;
		}

		public override byte GetLength()
		{
			return 12;
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

		public void setDc(Gs8BitMulti info)
		{
			((Req)ReqPacket).setDc(info);
		}
	}
}
