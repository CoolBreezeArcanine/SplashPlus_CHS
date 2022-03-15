namespace Comio.BD15070_4
{
	public class SetDisableResponseCommand : CommandBase
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

		public override byte GetCommandNo()
		{
			return 126;
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
			return null;
		}

		public override void DoRecv()
		{
		}
	}
}
