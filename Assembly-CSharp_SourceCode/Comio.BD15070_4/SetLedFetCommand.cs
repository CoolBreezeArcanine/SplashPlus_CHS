using UnityEngine;

namespace Comio.BD15070_4
{
	public class SetLedFetCommand : CommandBase
	{
		public class Req : PacketReqHeader
		{
			public new const int Size = 9;

			public byte r
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

			public byte g
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

			public byte b
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

			public Req()
				: base(9)
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
			return 57;
		}

		public override byte GetLength()
		{
			return 9;
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

		public void setColor(Color32 color)
		{
			Req obj = (Req)ReqPacket;
			obj.r = color.r;
			obj.g = color.g;
			obj.b = color.b;
		}

		public void setColorOff()
		{
			Req obj = (Req)ReqPacket;
			obj.r = 0;
			obj.g = 0;
			obj.b = 0;
		}
	}
}
