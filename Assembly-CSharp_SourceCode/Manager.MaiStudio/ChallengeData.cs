using System.Collections.Generic;
using System.Collections.ObjectModel;
using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class ChallengeData : AccessorBase
	{
		public StringID name { get; private set; }

		public StringID Music { get; private set; }

		public StringID EventName { get; private set; }

		public ReadOnlyCollection<ChallengeRelax> Relax { get; private set; }

		public string dataName { get; private set; }

		public ChallengeData()
		{
			name = new StringID();
			Music = new StringID();
			EventName = new StringID();
			Relax = new ReadOnlyCollection<ChallengeRelax>(new List<ChallengeRelax>());
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.ChallengeData sz)
		{
			name = (StringID)sz.name;
			Music = (StringID)sz.Music;
			EventName = (StringID)sz.EventName;
			List<ChallengeRelax> list = new List<ChallengeRelax>();
			foreach (Manager.MaiStudio.Serialize.ChallengeRelax item in sz.Relax)
			{
				list.Add((ChallengeRelax)item);
			}
			Relax = new ReadOnlyCollection<ChallengeRelax>(list);
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
