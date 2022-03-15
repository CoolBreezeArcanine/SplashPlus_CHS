using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class MapOtomodachiData : SerializeBase, ISerialize
	{
		public StringID name;

		public StringID BonusMusic;

		public int Rate;

		public int WinOdds;

		public int LoseOdds;

		public StringID Silhouette;

		public StringID Title;

		public string dataName;

		public MapOtomodachiData()
		{
			name = new StringID();
			BonusMusic = new StringID();
			Rate = 0;
			WinOdds = 0;
			LoseOdds = 0;
			Silhouette = new StringID();
			Title = new StringID();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.MapOtomodachiData(MapOtomodachiData sz)
		{
			Manager.MaiStudio.MapOtomodachiData mapOtomodachiData = new Manager.MaiStudio.MapOtomodachiData();
			mapOtomodachiData.Init(sz);
			return mapOtomodachiData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			BonusMusic.AddPath(parentPath);
			Silhouette.AddPath(parentPath);
			Title.AddPath(parentPath);
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
