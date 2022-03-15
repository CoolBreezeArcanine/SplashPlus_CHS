using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct GameEvent
	{
		public int type;

		public int id;

		public string startDate;

		public string endDate;
	}
}
