namespace Comio.BD15070_4
{
	public class SetLedGs8BitMultiCommand : CommandBase
	{
		public class Req : PacketReqHeader
		{
			public new const int Size = 13;

			public byte start
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

			public byte end
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

			public byte skip
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

			public byte r
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

			public byte g
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

			public byte b
			{
				get
				{
					return base[10];
				}
				set
				{
					base[10] = value;
				}
			}

			public byte speed
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

			public byte sum
			{
				get
				{
					return base[12];
				}
				set
				{
					base[12] = value;
				}
			}

			public Req()
				: base(13)
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
			return 50;
		}

		public override byte GetLength()
		{
			return 13;
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

		public void setColor(Gs8BitMulti info)
		{
			Req obj = (Req)ReqPacket;
			obj.start = info.Start;
			obj.end = info.End;
			obj.skip = info.Skip;
			obj.r = info.Color.r;
			obj.g = info.Color.g;
			obj.b = info.Color.b;
			obj.speed = info.Speed;
		}

		public void setAllOff()
		{
			Req obj = (Req)ReqPacket;
			obj.start = 0;
			obj.end = 10;
			obj.skip = 0;
			obj.r = 0;
			obj.g = 0;
			obj.b = 0;
			obj.speed = 0;
		}
	}
}
