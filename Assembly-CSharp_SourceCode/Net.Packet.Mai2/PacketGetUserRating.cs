using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserRating : Packet
	{
		private readonly Action<UserRating> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketGetUserRating(ulong userId, Action<UserRating> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<UserRatingRequestVO, UserRatingResponseVO> netQuery = new NetQuery<UserRatingRequestVO, UserRatingResponseVO>("GetUserRatingApi", userId);
			netQuery.Request.userId = userId;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserRatingRequestVO, UserRatingResponseVO> netQuery = base.Query as NetQuery<UserRatingRequestVO, UserRatingResponseVO>;
				SafeNullMember(netQuery.Response);
				_onDone(netQuery.Response.userRating);
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private static void SafeNullMember(UserRatingResponseVO src)
		{
			if (src.userRating.ratingList == null)
			{
				src.userRating.ratingList = Array.Empty<UserRate>();
			}
			if (src.userRating.newRatingList == null)
			{
				src.userRating.newRatingList = Array.Empty<UserRate>();
			}
		}
	}
}
