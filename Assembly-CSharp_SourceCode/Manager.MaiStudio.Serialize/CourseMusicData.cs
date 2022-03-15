using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class CourseMusicData : SerializeBase
	{
		public StringID musicId;

		public StringID difficulty;

		public CourseMusicData()
		{
			musicId = new StringID();
			difficulty = new StringID();
		}

		public static explicit operator Manager.MaiStudio.CourseMusicData(CourseMusicData sz)
		{
			Manager.MaiStudio.CourseMusicData courseMusicData = new Manager.MaiStudio.CourseMusicData();
			courseMusicData.Init(sz);
			return courseMusicData;
		}

		public override void AddPath(string parentPath)
		{
			musicId.AddPath(parentPath);
			difficulty.AddPath(parentPath);
		}
	}
}
