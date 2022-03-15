using System;
using AMDaemon;
using AMDaemon.Allnet;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketUserLogin : Packet
	{
		private readonly Action<UserLoginResponseVO> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketUserLogin(ulong userId, string acsessCode, Action<UserLoginResponseVO> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<UserLoginRequestVO, UserLoginResponseVO> netQuery = new NetQuery<UserLoginRequestVO, UserLoginResponseVO>("UserLoginApi", userId);
			netQuery.Request.userId = userId;
			netQuery.Request.accessCode = acsessCode;
			netQuery.Request.regionId = Auth.RegionCode;
			netQuery.Request.placeId = (int)Auth.LocationId;
			netQuery.Request.clientId = AMDaemon.System.KeychipId.ShortValue;
			netQuery.Request.dateTime = new DateTimeOffset(Auth.AuthTime).ToUnixTimeSeconds();
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserLoginRequestVO, UserLoginResponseVO> netQuery = base.Query as NetQuery<UserLoginRequestVO, UserLoginResponseVO>;
				_onDone(netQuery.Response);
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
