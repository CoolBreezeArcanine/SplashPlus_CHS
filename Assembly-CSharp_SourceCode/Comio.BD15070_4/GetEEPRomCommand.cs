namespace Comio.BD15070_4
{
	public class GetEEPRomCommand : CommandBase
	{
		public class Req : PacketReqHeader
		{
			public new const int Size = 7;

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

			public byte sum
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

			public Req()
				: base(7)
			{
			}
		}

		public class Ack : PacketAckHeader
		{
			public new const int Size = 9;

			public byte eepData
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

			public byte sum
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
		}

		public override byte GetCommandNo()
		{
			return 124;
		}

		public override byte GetLength()
		{
			return 7;
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
			_ = 9;
		}

		public void SetEEPDataAdress(byte adress)
		{
			((Req)ReqPacket).adress = adress;
		}

		public byte GetEEPData()
		{
			return ((Ack)AckPacket).eepData;
		}
	}
}
