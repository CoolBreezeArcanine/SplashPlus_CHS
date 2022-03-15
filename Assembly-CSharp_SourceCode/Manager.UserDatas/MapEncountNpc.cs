using System;

namespace Manager.UserDatas
{
	[Serializable]
	public class MapEncountNpc
	{
		public int NpcId;

		public int MusicId;

		public MapEncountNpc()
		{
		}

		public MapEncountNpc(int ncpId, int musicId)
		{
			NpcId = ncpId;
			MusicId = musicId;
		}

		public void Clear()
		{
			NpcId = 0;
			MusicId = 0;
		}
	}
}
