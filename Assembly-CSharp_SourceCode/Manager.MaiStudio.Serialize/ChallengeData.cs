using System;
using System.Collections.Generic;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class ChallengeData : SerializeBase, ISerialize
	{
		public StringID name;

		public StringID Music;

		public StringID EventName;

		public List<ChallengeRelax> Relax;

		public string dataName;

		public ChallengeData()
		{
			name = new StringID();
			Music = new StringID();
			EventName = new StringID();
			Relax = new List<ChallengeRelax>();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.ChallengeData(ChallengeData sz)
		{
			Manager.MaiStudio.ChallengeData challengeData = new Manager.MaiStudio.ChallengeData();
			challengeData.Init(sz);
			return challengeData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			Music.AddPath(parentPath);
			EventName.AddPath(parentPath);
			foreach (ChallengeRelax item in Relax)
			{
				item.AddPath(parentPath);
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
