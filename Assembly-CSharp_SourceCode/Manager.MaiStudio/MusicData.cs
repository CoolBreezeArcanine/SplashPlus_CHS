using System.Collections.Generic;
using System.Collections.ObjectModel;
using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class MusicData : AccessorBase
	{
		public StringID netOpenName { get; private set; }

		public StringID releaseTagName { get; private set; }

		public bool disable { get; private set; }

		public StringID name { get; private set; }

		public StringID rightsInfoName { get; private set; }

		public string sortName { get; private set; }

		public StringID artistName { get; private set; }

		public StringID genreName { get; private set; }

		public int bpm { get; private set; }

		public int version { get; private set; }

		public StringID AddVersion { get; private set; }

		public StringID movieName { get; private set; }

		public StringID cueName { get; private set; }

		public bool dresscode { get; private set; }

		public StringID eventName { get; private set; }

		public StringID subEventName { get; private set; }

		public MusicLockType lockType { get; private set; }

		public MusicLockType subLockType { get; private set; }

		public bool dotNetListView { get; private set; }

		public ReadOnlyCollection<Notes> notesData { get; private set; }

		public string jacketFile { get; private set; }

		public string thumbnailName { get; private set; }

		public string rightFile { get; private set; }

		public int priority { get; private set; }

		public string dataName { get; private set; }

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
			notesData = new ReadOnlyCollection<Notes>(new List<Notes>());
			jacketFile = "";
			thumbnailName = "";
			rightFile = "";
			priority = 0;
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.MusicData sz)
		{
			netOpenName = (StringID)sz.netOpenName;
			releaseTagName = (StringID)sz.releaseTagName;
			disable = sz.disable;
			name = (StringID)sz.name;
			rightsInfoName = (StringID)sz.rightsInfoName;
			sortName = sz.sortName;
			artistName = (StringID)sz.artistName;
			genreName = (StringID)sz.genreName;
			bpm = sz.bpm;
			version = sz.version;
			AddVersion = (StringID)sz.AddVersion;
			movieName = (StringID)sz.movieName;
			cueName = (StringID)sz.cueName;
			dresscode = sz.dresscode;
			eventName = (StringID)sz.eventName;
			subEventName = (StringID)sz.subEventName;
			lockType = sz.lockType;
			subLockType = sz.subLockType;
			dotNetListView = sz.dotNetListView;
			List<Notes> list = new List<Notes>();
			foreach (Manager.MaiStudio.Serialize.Notes notesDatum in sz.notesData)
			{
				list.Add((Notes)notesDatum);
			}
			notesData = new ReadOnlyCollection<Notes>(list);
			jacketFile = sz.jacketFile;
			thumbnailName = sz.thumbnailName;
			rightFile = sz.rightFile;
			priority = sz.priority;
			dataName = sz.dataName;
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
