using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct UserMap
	{
		public int mapId;

		public uint distance;

		public bool isLock;

		public bool isClear;

		public bool isComplete;
	}
}
