using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class MusicGroupData : SerializeBase, ISerialize
	{
		public StringID name;

		public StringsCollection MusicIds;

		public string dataName;

		public MusicGroupData()
		{
			name = new StringID();
			MusicIds = new StringsCollection();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.MusicGroupData(MusicGroupData sz)
		{
			Manager.MaiStudio.MusicGroupData musicGroupData = new Manager.MaiStudio.MusicGroupData();
			musicGroupData.Init(sz);
			return musicGroupData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			MusicIds.AddPath(parentPath);
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
