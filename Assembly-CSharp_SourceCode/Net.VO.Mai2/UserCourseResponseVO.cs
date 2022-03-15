using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserCourseResponseVO : VOSerializer
	{
		public ulong userId;

		public long nextIndex;

		public UserCourse[] userCourseList;
	}
}
