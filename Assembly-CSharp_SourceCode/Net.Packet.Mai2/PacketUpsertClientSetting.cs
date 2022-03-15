using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketUpsertClientSetting : Packet
	{
		private readonly Action<int> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketUpsertClientSetting(Action<int> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<ClientSettingRequestVO, UpsertResponseVO> netQuery = new NetQuery<ClientSettingRequestVO, UpsertResponseVO>("UpsertClientSettingApi", 0uL);
			VOExtensions.ExportClientSetting(ref netQuery.Request.clientSetting);
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<ClientSettingRequestVO, UpsertResponseVO> netQuery = base.Query as NetQuery<ClientSettingRequestVO, UpsertResponseVO>;
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
