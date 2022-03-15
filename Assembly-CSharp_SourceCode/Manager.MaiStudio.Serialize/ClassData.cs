using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class ClassData : SerializeBase, ISerialize
	{
		public bool disable;

		public StringID name;

		public int startPoint;

		public int classupPoint;

		public int winSameClass;

		public int loseSameClass;

		public int winUpperClass;

		public int loseUpperClass;

		public int winLowerClass;

		public int loseLowerClass;

		public StringID classBoss;

		public StringID npcBaseMusicByBoss;

		public int npcFluctuationAchieveLower;

		public int npcFluctuationAchieveUpper;

		public int npcFluctuationLevelLower;

		public int npcFluctuationLevelUpper;

		public string dataName;

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

		public static explicit operator Manager.MaiStudio.ClassData(ClassData sz)
		{
			Manager.MaiStudio.ClassData classData = new Manager.MaiStudio.ClassData();
			classData.Init(sz);
			return classData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			classBoss.AddPath(parentPath);
			npcBaseMusicByBoss.AddPath(parentPath);
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
