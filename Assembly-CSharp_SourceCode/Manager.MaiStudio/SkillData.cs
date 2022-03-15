using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class SkillData : AccessorBase
	{
		public StringID name { get; private set; }

		public StringsCollection targetCharaNames { get; private set; }

		public string dataName { get; private set; }

		public SkillData()
		{
			name = new StringID();
			targetCharaNames = new StringsCollection();
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.SkillData sz)
		{
			name = (StringID)sz.name;
			targetCharaNames = (StringsCollection)sz.targetCharaNames;
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
			return false;
		}
	}
}
