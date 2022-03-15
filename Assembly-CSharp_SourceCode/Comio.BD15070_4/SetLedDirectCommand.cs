using UnityEngine;

namespace Comio.BD15070_4
{
	public class SetLedDirectCommand : CommandBase
	{
		public class Req : PacketReqHeader
		{
			private const int SumIndex = 38;

			public new const int Size = 39;

			public byte sum
			{
				get
				{
					return base[38];
				}
				set
				{
					base[38] = value;
				}
			}

			public Req()
				: base(39)
			{
			}

			public void setColor(int index, Color32 color)
			{
				int num = 5 + index * 3;
				base[num] = color.r;
				base[num + 1] = color.g;
				base[num + 2] = color.b;
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
			return 130;
		}

		public override byte GetLength()
		{
			return 39;
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

		public void setColor(byte ledPos, Color color)
		{
			if (11 > ledPos)
			{
				((Req)ReqPacket).setColor(ledPos, color);
			}
		}
	}
}
