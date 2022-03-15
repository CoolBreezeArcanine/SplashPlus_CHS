using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class ClassData : AccessorBase
	{
		public bool disable { get; private set; }

		public StringID name { get; private set; }

		public int startPoint { get; private set; }

		public int classupPoint { get; private set; }

		public int winSameClass { get; private set; }

		public int loseSameClass { get; private set; }

		public int winUpperClass { get; private set; }

		public int loseUpperClass { get; private set; }

		public int winLowerClass { get; private set; }

		public int loseLowerClass { get; private set; }

		public StringID classBoss { get; private set; }

		public StringID npcBaseMusicByBoss { get; private set; }

		public int npcFluctuationAchieveLower { get; private set; }

		public int npcFluctuationAchieveUpper { get; private set; }

		public int npcFluctuationLevelLower { get; private set; }

		public int npcFluctuationLevelUpper { get; private set; }

		public string dataName { get; private set; }

		public ClassData()
		{
			disable = false;
			name = new StringID();
			startPoint = 0;
			classupPoint = 0;
			winSameClass = 0;
			loseSameClass = 0;
			winUpperClass = 0;
			loseUpperClass = 0;
			winLowerClass = 0;
			loseLowerClass = 0;
			classBoss = new StringID();
			npcBaseMusicByBoss = new StringID();
			npcFluctuationAchieveLower = 0;
			npcFluctuationAchieveUpper = 0;
			npcFluctuationLevelLower = 0;
			npcFluctuationLevelUpper = 0;
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.ClassData sz)
		{
			disable = sz.disable;
			name = (StringID)sz.name;
			startPoint = sz.startPoint;
			classupPoint = sz.classupPoint;
			winSameClass = sz.winSameClass;
			loseSameClass = sz.loseSameClass;
			winUpperClass = sz.winUpperClass;
			loseUpperClass = sz.loseUpperClass;
			winLowerClass = sz.winLowerClass;
			loseLowerClass = sz.loseLowerClass;
			classBoss = (StringID)sz.classBoss;
			npcBaseMusicByBoss = (StringID)sz.npcBaseMusicByBoss;
			npcFluctuationAchieveLower = sz.npcFluctuationAchieveLower;
			npcFluctuationAchieveUpper = sz.npcFluctuationAchieveUpper;
			npcFluctuationLevelLower = sz.npcFluctuationLevelLower;
			npcFluctuationLevelUpper = sz.npcFluctuationLevelUpper;
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
