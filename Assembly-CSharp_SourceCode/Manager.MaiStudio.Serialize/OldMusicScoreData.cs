using System;
using System.Collections.Generic;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class OldMusicScoreData : SerializeBase, ISerialize
	{
		public StringID name;

		public List<OldMusicScoreSt> notesData;

		public string dataName;

		public OldMusicScoreData()
		{
			name = new StringID();
			notesData = new List<OldMusicScoreSt>();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.OldMusicScoreData(OldMusicScoreData sz)
		{
			Manager.MaiStudio.OldMusicScoreData oldMusicScoreData = new Manager.MaiStudio.OldMusicScoreData();
			oldMusicScoreData.Init(sz);
			return oldMusicScoreData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			foreach (OldMusicScoreSt notesDatum in notesData)
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
		}

		public bool IsDisable()
		{
			return false;
		}
	}
}
