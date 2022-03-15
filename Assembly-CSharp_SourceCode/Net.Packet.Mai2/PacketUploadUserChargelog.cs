using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketUploadUserChargelog : Packet
	{
		private readonly Action<int> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketUploadUserChargelog(ulong userId, UserCharge src, UserChargelog srclog, Action<int> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<UserChargelogRequestVO, UpsertResponseVO> netQuery = new NetQuery<UserChargelogRequestVO, UpsertResponseVO>("UpsertUserChargelogApi", userId);
			netQuery.Request.userCharge = src;
			netQuery.Request.userChargelog = srclog;
			netQuery.Request.userId = userId;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserChargelogRequestVO, UpsertResponseVO> netQuery = base.Query as NetQuery<UserChargelogRequestVO, UpsertResponseVO>;
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
