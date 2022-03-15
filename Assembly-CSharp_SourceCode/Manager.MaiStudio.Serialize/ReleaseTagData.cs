using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class ReleaseTagData : SerializeBase
	{
		public StringID name;

		public string dataName;

		public ReleaseTagData()
		{
			name = new StringID();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.ReleaseTagData(ReleaseTagData sz)
		{
			Manager.MaiStudio.ReleaseTagData releaseTagData = new Manager.MaiStudio.ReleaseTagData();
			releaseTagData.Init(sz);
			return releaseTagData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
		}
	}
}
