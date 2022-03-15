using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class UdemaeData : AccessorBase
	{
		public bool disable { get; private set; }

		public StringID name { get; private set; }

		public int border { get; private set; }

		public int rate { get; private set; }

		public int winBase { get; private set; }

		public int loseBase { get; private set; }

		public int rateDiff { get; private set; }

		public float coeffcient { get; private set; }

		public int loseAdjust { get; private set; }

		public int npcAchieve { get; private set; }

		public int npcFluctuation { get; private set; }

		public StringID npcDifficulty { get; private set; }

		public StringID udemaeBoss { get; private set; }

		public string dataName { get; private set; }

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

		public void Init(Manager.MaiStudio.Serialize.UdemaeData sz)
		{
			disable = sz.disable;
			name = (StringID)sz.name;
			border = sz.border;
			rate = sz.rate;
			winBase = sz.winBase;
			loseBase = sz.loseBase;
			rateDiff = sz.rateDiff;
			coeffcient = sz.coeffcient;
			loseAdjust = sz.loseAdjust;
			npcAchieve = sz.npcAchieve;
			npcFluctuation = sz.npcFluctuation;
			npcDifficulty = (StringID)sz.npcDifficulty;
			udemaeBoss = (StringID)sz.udemaeBoss;
			dataName = sz.dataName;
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
