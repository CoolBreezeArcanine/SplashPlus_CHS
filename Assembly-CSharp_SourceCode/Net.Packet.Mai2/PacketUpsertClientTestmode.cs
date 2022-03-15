using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketUpsertClientTestmode : Packet
	{
		private readonly Action<int> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketUpsertClientTestmode(Action<int> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<ClientTestmodeRequestVO, UpsertResponseVO> netQuery = new NetQuery<ClientTestmodeRequestVO, UpsertResponseVO>("UpsertClientTestmodeApi", 0uL);
			VOExtensions.ExportClientTestmode(ref netQuery.Request.clientTestmode);
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<ClientTestmodeRequestVO, UpsertResponseVO> netQuery = base.Query as NetQuery<ClientTestmodeRequestVO, UpsertResponseVO>;
				_onDone(netQuery.Response.returnCode);
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
