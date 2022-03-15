using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct UserLoginBonus
	{
		public int bonusId;

		public uint point;

		public bool isCurrent;

		public bool isComplete;
	}
}
