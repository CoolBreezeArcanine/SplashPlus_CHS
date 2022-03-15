namespace Comio
{
	public class GetProtocolVersionCommand : CommandBase
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
			public new const int Size = 11;

			public byte appliMode
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

			public byte major
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

			public byte minor
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

			public byte sum
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
		}

		private bool _appliMode;

		private byte _major;

		private byte _minor;

		public override byte GetCommandNo()
		{
			return 243;
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

		public GetProtocolVersionCommand()
		{
			_appliMode = false;
			_major = 0;
			_minor = 0;
		}

		public override void DoRecv()
		{
			if (AckPacket.Count == 11)
			{
				Ack ack = (Ack)AckPacket;
				_appliMode = ((ack.appliMode != 0) ? true : false);
				_major = ack.major;
				_minor = ack.minor;
			}
		}

		public bool isAppliMode()
		{
			return _appliMode;
		}

		public byte getMajor()
		{
			return _major;
		}

		public byte getMinor()
		{
			return _minor;
		}
	}
}
