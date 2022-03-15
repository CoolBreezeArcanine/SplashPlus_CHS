using System.Collections.Generic;
using System.Linq;
using Net.Packet;
using Net.Packet.Mai2;
using Net.VO.Mai2;

namespace Manager.Operation
{
	public class DataDownloaderMai2 : DataDownloader
	{
		private enum PacketType
		{
			GameSetting,
			GameRanking,
			GameEvent,
			GameTournamentInfo,
			GameCharge
		}

		private Packet _current;

		private readonly Queue<PacketType> _queue = new Queue<PacketType>();

		public OperationData OperationData { get; } = new OperationData();


		protected override void InitPacketList(bool isNetworkTest)
		{
			if (_current == null)
			{
				_queue.Clear();
				if (isNetworkTest)
				{
					_queue.Enqueue(PacketType.GameSetting);
				}
				else if (!base.WasDownloadSuccessOnce)
				{
					_queue.Enqueue(PacketType.GameSetting);
					_queue.Enqueue(PacketType.GameRanking);
					_queue.Enqueue(PacketType.GameEvent);
					_queue.Enqueue(PacketType.GameTournamentInfo);
					_queue.Enqueue(PacketType.GameCharge);
				}
				else
				{
					_queue.Enqueue(PacketType.GameSetting);
					_queue.Enqueue(PacketType.GameEvent);
					_queue.Enqueue(PacketType.GameTournamentInfo);
					_queue.Enqueue(PacketType.GameCharge);
				}
				_current = CreatePacket(_queue.Dequeue());
			}
		}

		protected override PacketState ProcPacket()
		{
			return _current?.Proc() ?? PacketState.Done;
		}

		protected override bool NextPacket()
		{
			if (_queue.Any())
			{
				_current = CreatePacket(_queue.Dequeue());
				return true;
			}
			OperationData.IsUpdate = true;
			_current = null;
			return false;
		}

		protected override void EndPacket()
		{
			_current = null;
		}

		private Packet CreatePacket(PacketType type)
		{
			return type switch
			{
				PacketType.GameSetting => new PacketGetGameSetting(delegate(GameSetting data)
				{
					OperationData.GameSetting = data;
				}), 
				PacketType.GameRanking => new PacketGetGameRanking(1, delegate(GameRanking[] data)
				{
					OperationData.GameRankings = data;
				}), 
				PacketType.GameEvent => new PacketGetGameEvent(delegate(GameEvent[] data)
				{
					OperationData.GameEvents = data;
				}), 
				PacketType.GameTournamentInfo => new PacketGetGameTournamentInfo(delegate(GameTournamentInfo[] data)
				{
					OperationData.GameTournamentInfos = data;
				}), 
				PacketType.GameCharge => new PacketGetGameCharge(delegate(GameCharge[] data)
				{
					OperationData.GameCharges = data;
				}), 
				_ => null, 
			};
		}
	}
}
