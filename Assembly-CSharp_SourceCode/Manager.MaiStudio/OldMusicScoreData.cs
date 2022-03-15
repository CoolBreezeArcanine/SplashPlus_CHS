using System.Collections.Generic;
using System.Collections.ObjectModel;
using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class OldMusicScoreData : AccessorBase
	{
		public StringID name { get; private set; }

		public ReadOnlyCollection<OldMusicScoreSt> notesData { get; private set; }

		public string dataName { get; private set; }

		public OldMusicScoreData()
		{
			name = new StringID();
			notesData = new ReadOnlyCollection<OldMusicScoreSt>(new List<OldMusicScoreSt>());
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.OldMusicScoreData sz)
		{
			name = (StringID)sz.name;
			List<OldMusicScoreSt> list = new List<OldMusicScoreSt>();
			foreach (Manager.MaiStudio.Serialize.OldMusicScoreSt notesDatum in sz.notesData)
			{
				list.Add((OldMusicScoreSt)notesDatum);
			}
			notesData = new ReadOnlyCollection<OldMusicScoreSt>(list);
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
