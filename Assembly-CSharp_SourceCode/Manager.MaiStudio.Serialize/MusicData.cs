using System;
using System.Collections.Generic;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class MusicData : SerializeBase, ISerialize
	{
		public StringID netOpenName;

		public StringID releaseTagName;

		public bool disable;

		public StringID name;

		public StringID rightsInfoName;

		public string sortName;

		public StringID artistName;

		public StringID genreName;

		public int bpm;

		public int version;

		public StringID AddVersion;

		public StringID movieName;

		public StringID cueName;

		public bool dresscode;

		public StringID eventName;

		public StringID subEventName;

		public MusicLockType lockType;

		public MusicLockType subLockType;

		public bool dotNetListView;

		public List<Notes> notesData;

		public string jacketFile;

		public string thumbnailName;

		public string rightFile;

		public int priority;

		public string dataName;

		public MusicData()
		{
			netOpenName = new StringID();
			releaseTagName = new StringID();
			disable = false;
			name = new StringID();
			rightsInfoName = new StringID();
			sortName = "";
			artistName = new StringID();
			genreName = new StringID();
			bpm = 0;
			version = 0;
			AddVersion = new StringID();
			movieName = new StringID();
			cueName = new StringID();
			dresscode = false;
			eventName = new StringID();
			subEventName = new StringID();
			lockType = MusicLockType.Unlock;
			subLockType = MusicLockType.Unlock;
			dotNetListView = false;
			notesData = new List<Notes>();
			jacketFile = "";
			thumbnailName = "";
			rightFile = "";
			priority = 0;
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.MusicData(MusicData sz)
		{
			Manager.MaiStudio.MusicData musicData = new Manager.MaiStudio.MusicData();
			musicData.Init(sz);
			return musicData;
		}

		public override void AddPath(string parentPath)
		{
			netOpenName.AddPath(parentPath);
			releaseTagName.AddPath(parentPath);
			name.AddPath(parentPath);
			rightsInfoName.AddPath(parentPath);
			artistName.AddPath(parentPath);
			genreName.AddPath(parentPath);
			AddVersion.AddPath(parentPath);
			movieName.AddPath(parentPath);
			cueName.AddPath(parentPath);
			eventName.AddPath(parentPath);
			subEventName.AddPath(parentPath);
			foreach (Notes notesDatum in notesData)
			{
				notesDatum.AddPath(parentPath);
			}
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
