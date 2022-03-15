using System;
using System.Collections.Generic;
using System.Linq;
using MAI2.Util;
using Manager;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketUpsertClientBookkeeping : Packet
	{
		private readonly Action<int> _onDone;

		private readonly Action<PacketStatus> _onError;

		private readonly Queue<DailyLog> _dailyLogs;

		public PacketUpsertClientBookkeeping(Queue<DailyLog> dailyLogs, Action<int> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			_dailyLogs = dailyLogs;
			NetQuery<ClientBookkeepingRequestVO, UpsertResponseVO> netQuery = new NetQuery<ClientBookkeepingRequestVO, UpsertResponseVO>("UpsertClientBookkeepingApi", 0uL);
			_dailyLogs.Peek().Export(ref netQuery.Request.clientBookkeeping);
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.bookkeep.setDailyLogReported(_dailyLogs.Dequeue().dateTime);
				NetQuery<ClientBookkeepingRequestVO, UpsertResponseVO> netQuery = base.Query as NetQuery<ClientBookkeepingRequestVO, UpsertResponseVO>;
				if (_dailyLogs.Any())
				{
					_dailyLogs.Peek().Export(ref netQuery.Request.clientBookkeeping);
					Create(netQuery);
				}
				else
				{
					_onDone(netQuery.Response.returnCode);
				}
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}
	}
}
