using UnityEngine;

namespace Comio.BD15070_4
{
	public class SetLedGs8BitCommand : CommandBase
	{
		public class Req : PacketReqHeader
		{
			public new const int Size = 10;

			public byte index
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

			public byte r
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

			public byte g
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

			public byte b
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

			public Req()
				: base(10)
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
			return 49;
		}

		public override byte GetLength()
		{
			return 10;
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

		public void setColor(byte ledPos, Color32 color)
		{
			if (11 > ledPos)
			{
				Req obj = (Req)ReqPacket;
				obj.index = ledPos;
				obj.r = color.r;
				obj.g = color.g;
				obj.b = color.b;
			}
		}
	}
}
