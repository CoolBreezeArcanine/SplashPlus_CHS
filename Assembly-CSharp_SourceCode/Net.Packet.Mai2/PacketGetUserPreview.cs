using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserPreview : Packet
	{
		private readonly Action<ulong, UserPreviewResponseVO> _onDone;

		private readonly Action<PacketStatus> _onError;

		private readonly ulong _userId;

		public PacketGetUserPreview(ulong userId, string authKey, Action<ulong, UserPreviewResponseVO> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			_userId = userId;
			NetQuery<UserPreviewRequestVO, UserPreviewResponseVO> netQuery = new NetQuery<UserPreviewRequestVO, UserPreviewResponseVO>("GetUserPreviewApi", userId);
			netQuery.Request.userId = userId;
			netQuery.Request.segaIdAuthKey = authKey;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserPreviewRequestVO, UserPreviewResponseVO> netQuery = base.Query as NetQuery<UserPreviewRequestVO, UserPreviewResponseVO>;
				_onDone(_userId, netQuery.Response);
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
