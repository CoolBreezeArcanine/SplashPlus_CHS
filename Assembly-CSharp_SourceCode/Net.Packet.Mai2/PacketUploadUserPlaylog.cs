using System;
using AMDaemon.Allnet;
using Manager;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketUploadUserPlaylog : Packet
	{
		private readonly Action<int> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketUploadUserPlaylog(int index, UserData src, int trackNo, Action<int> onDone, Action<PacketStatus> onError = null)
			: this(UserID.IsGuest(src.Detail.UserID) ? UserID.GuestID(Auth.LocationId) : src.Detail.UserID, Convert(index, src, trackNo), onDone, onError)
		{
		}

		public PacketUploadUserPlaylog(ulong userId, UserPlaylog src, Action<int> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<UserPlaylogRequestVO, UpsertResponseVO> netQuery = new NetQuery<UserPlaylogRequestVO, UpsertResponseVO>("UploadUserPlaylogApi", userId);
			netQuery.Request.userPlaylog = src;
			netQuery.Request.userId = userId;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserPlaylogRequestVO, UpsertResponseVO> netQuery = base.Query as NetQuery<UserPlaylogRequestVO, UpsertResponseVO>;
				_onDone(netQuery.Response.returnCode);
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private static UserPlaylog Convert(int index, UserData src, int trackNo)
		{
			return src.ExportUserPlaylog(index, trackNo);
		}
	}
}
