using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class SkillData : SerializeBase
	{
		public StringID name;

		public StringsCollection targetCharaNames;

		public string dataName;

		public SkillData()
		{
			name = new StringID();
			targetCharaNames = new StringsCollection();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.SkillData(SkillData sz)
		{
			Manager.MaiStudio.SkillData skillData = new Manager.MaiStudio.SkillData();
			skillData.Init(sz);
			return skillData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			targetCharaNames.AddPath(parentPath);
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
			return false;
		}
	}
}
