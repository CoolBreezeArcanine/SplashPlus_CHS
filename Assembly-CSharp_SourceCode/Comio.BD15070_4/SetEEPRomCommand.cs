namespace Comio.BD15070_4
{
	public class SetEEPRomCommand : CommandBase
	{
		public class Req : PacketReqHeader
		{
			public new const int Size = 8;

			public byte adress
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

			public byte writeData
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

			public Req()
				: base(8)
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
			return 123;
		}

		public override byte GetLength()
		{
			return 8;
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
			_ = AckPacket.Count;
			_ = 8;
		}

		public void SetEEPData(byte adress, byte data)
		{
			Req obj = (Req)ReqPacket;
			obj.adress = adress;
			obj.writeData = data;
		}
	}
}
