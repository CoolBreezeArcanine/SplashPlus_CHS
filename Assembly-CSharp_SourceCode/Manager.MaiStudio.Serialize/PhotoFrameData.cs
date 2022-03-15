using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class PhotoFrameData : SerializeBase, ISerialize
	{
		public StringID name;

		public bool disable;

		public StringID eventName;

		public string imageFile;

		public int priority;

		public string dataName;

		public PhotoFrameData()
		{
			name = new StringID();
			disable = false;
			eventName = new StringID();
			imageFile = "";
			priority = 0;
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.PhotoFrameData(PhotoFrameData sz)
		{
			Manager.MaiStudio.PhotoFrameData photoFrameData = new Manager.MaiStudio.PhotoFrameData();
			photoFrameData.Init(sz);
			return photoFrameData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			eventName.AddPath(parentPath);
		}

		public int GetID()
		{
			return name.id;
		}

		public void SetPriority(int pri)
		{
			priority = pri;
		}

		public bool IsDisable()
		{
			return disable;
		}
	}
}
