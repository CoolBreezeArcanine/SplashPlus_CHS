using System;
using Manager;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct UserGhost
	{
		public string name;

		public int iconId;

		public int plateId;

		public int titleId;

		public int rate;

		public int udemaeRate;

		public uint courseRank;

		public uint classRank;

		public int classValue;

		public string playDatetime;

		public uint shopId;

		public int regionCode;

		public MusicDifficultyID typeId;

		public int musicId;

		public int difficulty;

		public int version;

		public byte[] resultBitList;

		public int resultNum;

		public int achievement;
	}
}
