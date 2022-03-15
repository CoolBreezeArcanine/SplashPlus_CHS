using System;
using System.Collections.Generic;
using System.Linq;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserCourse : Packet
	{
		private readonly Action<UserCourse[]> _onDone;

		private readonly Action<PacketStatus> _onError;

		private readonly List<UserCourseResponseVO> _responseVOs;

		public PacketGetUserCourse(ulong userId, Action<UserCourse[]> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			_responseVOs = new List<UserCourseResponseVO>();
			Create(GetNetQuery(userId, 0L));
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserCourseRequestVO, UserCourseResponseVO> netQuery = base.Query as NetQuery<UserCourseRequestVO, UserCourseResponseVO>;
				SafeNullMember(netQuery.Response);
				_responseVOs.Add(netQuery.Response);
				if (netQuery.Response.nextIndex != 0L)
				{
					Create(GetNetQuery(netQuery.Response.userId, netQuery.Response.nextIndex));
					break;
				}
				_onDone(_responseVOs.SelectMany((UserCourseResponseVO i) => i.userCourseList).ToArray());
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private NetQuery<UserCourseRequestVO, UserCourseResponseVO> GetNetQuery(ulong userId, long nextIndex)
		{
			NetQuery<UserCourseRequestVO, UserCourseResponseVO> netQuery = new NetQuery<UserCourseRequestVO, UserCourseResponseVO>("GetUserCourseApi", userId);
			netQuery.Request.userId = userId;
			netQuery.Request.nextIndex = nextIndex;
			return netQuery;
		}

		private static void SafeNullMember(UserCourseResponseVO src)
		{
			if (src.userCourseList == null)
			{
				src.userCourseList = Array.Empty<UserCourse>();
			}
		}
	}
}
