using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class CourseMusicData : AccessorBase
	{
		public StringID musicId { get; private set; }

		public StringID difficulty { get; private set; }

		public CourseMusicData()
		{
			musicId = new StringID();
			difficulty = new StringID();
		}

		public void Init(Manager.MaiStudio.Serialize.CourseMusicData sz)
		{
			musicId = (StringID)sz.musicId;
			difficulty = (StringID)sz.difficulty;
		}
	}
}
