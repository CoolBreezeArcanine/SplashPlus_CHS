using System;
using AMDaemon;
using AMDaemon.Allnet;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetGameSetting : Packet
	{
		private readonly Action<GameSetting> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketGetGameSetting(Action<GameSetting> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<GameSettingRequestVO, GameSettingResponseVO> netQuery = new NetQuery<GameSettingRequestVO, GameSettingResponseVO>("GetGameSettingApi", 0uL);
			netQuery.Request.placeId = (int)Auth.LocationId;
			netQuery.Request.clientId = AMDaemon.System.KeychipId.ShortValue;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<GameSettingRequestVO, GameSettingResponseVO> netQuery = base.Query as NetQuery<GameSettingRequestVO, GameSettingResponseVO>;
				_onDone(netQuery.Response.gameSetting);
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
