namespace Comio.BD15070_4
{
	public class GetBoardStatusCommand : CommandBase
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
			public new const int Size = 12;

			public byte timeoutStat
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

			public byte timeoutSec
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

			public byte pwmIo
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

			public byte fetTimeout
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
		}

		public struct AppReportData
		{
			public const int size = 5;

			public byte timeoutStat;

			public byte timeoutSec;

			public byte pwmIo;

			public byte fetTimeout;
		}

		private BoardStatus _boardStatus;

		public override byte GetCommandNo()
		{
			return 241;
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

		public GetBoardStatusCommand()
		{
			_boardStatus = new BoardStatus();
		}

		public override void DoRecv()
		{
			AppReportData ack = default(AppReportData);
			if (AckPacket.Count == 12)
			{
				_boardStatus.Clear();
				Ack ack2 = (Ack)AckPacket;
				ack.timeoutStat = ack2.timeoutStat;
				ack.timeoutSec = ack2.timeoutSec;
				ack.pwmIo = ack2.pwmIo;
				ack.fetTimeout = ack2.fetTimeout;
				makeBoardStatus(_boardStatus, ack);
			}
		}

		public static void makeBoardStatus(BoardStatus boardStatus, AppReportData ack)
		{
			boardStatus.Clear();
			boardStatus.TimeoutStat = ack.timeoutStat;
			boardStatus.TimeoutSec = ack.timeoutSec;
			boardStatus.PwmIo = ack.pwmIo;
			boardStatus.FetTimeout = ack.fetTimeout;
		}

		public BoardStatus getBoardStatus()
		{
			return _boardStatus;
		}
	}
}
