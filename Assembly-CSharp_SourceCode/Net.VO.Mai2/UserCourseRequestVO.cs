using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserCourseRequestVO : VOSerializer
	{
		public ulong userId;

		public long nextIndex;
	}
}
