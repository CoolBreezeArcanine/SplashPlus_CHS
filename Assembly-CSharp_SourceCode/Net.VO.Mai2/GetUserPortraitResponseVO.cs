using System;
using System.Collections.Generic;

namespace Net.VO.Mai2
{
	[Serializable]
	public class GetUserPortraitResponseVO : VOSerializer
	{
		public int length;

		public List<UserPortrait> userPortraitList;
	}
}
