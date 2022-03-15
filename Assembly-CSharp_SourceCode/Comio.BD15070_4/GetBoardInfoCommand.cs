namespace Comio.BD15070_4
{
	public class GetBoardInfoCommand : CommandBase
	{
		public class Req : PacketReqHeader
		{
			public new const int Size = 6;

			public byte Sum
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
		}

		private BoardNo _boardNo;

		private byte _firmRevision;

		public override byte GetCommandNo()
		{
			return 240;
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

		public GetBoardInfoCommand()
		{
			_boardNo = new BoardNo();
			_firmRevision = 0;
		}

		public BoardNo getBoardNo()
		{
			return _boardNo;
		}

		public byte getFirmRevision()
		{
			return _firmRevision;
		}

		public override void DoRecv()
		{
			int count = AckPacket.Count;
			int num = 7;
			if (count <= num)
			{
				return;
			}
			_boardNo.Clear();
			_firmRevision = 0;
			int num2 = 0;
			int num3 = num;
			while (num3 < count)
			{
				if (num3 >= count)
				{
					return;
				}
				if (AckPacket[num3] == byte.MaxValue)
				{
					num3++;
					break;
				}
				_boardNo.Text += (char)AckPacket[num3];
				num3++;
				num2++;
			}
			if (num3 < count && num3 < count)
			{
				_firmRevision = AckPacket[num3++];
			}
		}
	}
}
