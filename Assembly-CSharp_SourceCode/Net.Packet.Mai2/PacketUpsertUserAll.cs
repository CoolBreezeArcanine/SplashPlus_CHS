using System;
using AMDaemon.Allnet;
using MAI2.Util;
using Manager;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketUpsertUserAll : Packet
	{
		private readonly Action<int> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketUpsertUserAll(int index, UserData src, Action<int> onDone, Action<PacketStatus> onError = null)
			: this(index, UserID.IsGuest(src.Detail.UserID) ? UserID.GuestID(Auth.LocationId) : src.Detail.UserID, ToUserAll(src), onDone, onError)
		{
		}

		public PacketUpsertUserAll(int index, ulong userId, UserAll src, Action<int> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<UserAllRequestVO, UpsertResponseVO> netQuery = new NetQuery<UserAllRequestVO, UpsertResponseVO>("UpsertUserAllApi", userId);
			netQuery.Request.userId = userId;
			netQuery.Request.playlogId = Singleton<NetDataManager>.Instance.GetLoginVO(index)?.loginId ?? 0;
			netQuery.Request.isEventMode = GameManager.IsEventMode;
			netQuery.Request.isFreePlay = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsFreePlay();
			netQuery.Request.upsertUserAll = src;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserAllRequestVO, UpsertResponseVO> netQuery = base.Query as NetQuery<UserAllRequestVO, UpsertResponseVO>;
				_onDone(netQuery.Response.returnCode);
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private static UserAll ToUserAll(UserData src)
		{
			UserAll dst = new UserAll();
			src.ExportUserAll(ref dst);
			return dst;
		}
	}
}
