using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class UdemaeData : SerializeBase, ISerialize
	{
		public bool disable;

		public StringID name;

		public int border;

		public int rate;

		public int winBase;

		public int loseBase;

		public int rateDiff;

		public float coeffcient;

		public int loseAdjust;

		public int npcAchieve;

		public int npcFluctuation;

		public StringID npcDifficulty;

		public StringID udemaeBoss;

		public string dataName;

		public UdemaeData()
		{
			disable = false;
			name = new StringID();
			border = 0;
			rate = 0;
			winBase = 0;
			loseBase = 0;
			rateDiff = 0;
			coeffcient = 0f;
			loseAdjust = 0;
			npcAchieve = 0;
			npcFluctuation = 0;
			npcDifficulty = new StringID();
			udemaeBoss = new StringID();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.UdemaeData(UdemaeData sz)
		{
			Manager.MaiStudio.UdemaeData udemaeData = new Manager.MaiStudio.UdemaeData();
			udemaeData.Init(sz);
			return udemaeData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			npcDifficulty.AddPath(parentPath);
			udemaeBoss.AddPath(parentPath);
		}

		public int GetID()
		{
			return name.id;
		}

		public void SetPriority(int pri)
		{
		}

		public bool IsDisable()
		{
			return disable;
		}
	}
}
