using System.Collections.Generic;
using Manager.MaiStudio;

namespace Manager
{
	public class NotesWrapper
	{
		public StringID Name = new StringID();

		public List<Notes> NotesList = new List<Notes>();

		public List<StringID> EventName = new List<StringID>();

		public List<MusicLockType> LockType = new List<MusicLockType>();

		public List<bool> IsEnable = new List<bool>();

		public List<bool> IsUnlock = new List<bool>();

		public List<bool> IsMapBonus = new List<bool>();

		public List<bool>[] IsRating = new List<bool>[2]
		{
			new List<bool>(),
			new List<bool>()
		};

		public List<bool>[] IsNeedUnlock = new List<bool>[2]
		{
			new List<bool>(),
			new List<bool>()
		};

		public List<GhostMatchData>[] VsGhost = new List<GhostMatchData>[2]
		{
			new List<GhostMatchData>(),
			new List<GhostMatchData>()
		};

		public List<GhostMatchData>[] MapGhost = new List<GhostMatchData>[2]
		{
			new List<GhostMatchData>(),
			new List<GhostMatchData>()
		};

		public List<GhostMatchData>[] BossGhost = new List<GhostMatchData>[2]
		{
			new List<GhostMatchData>(),
			new List<GhostMatchData>()
		};

		public ChallengeDetail[] ChallengeDetail = new ChallengeDetail[2];

		public bool IsNewMusic;

		public int Ranking = -1;

		public bool IsPlayable;

		public bool[] IsMapTaskMusic = new bool[2];

		public bool IsTournament;

		public List<int> ScoreRankings = new List<int>();

		public bool SpecialUnlock;
	}
}
