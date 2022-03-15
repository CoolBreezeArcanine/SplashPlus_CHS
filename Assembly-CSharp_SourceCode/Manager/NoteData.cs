using System.Collections.Generic;

namespace Manager
{
	public class NoteData : TimingBase
	{
		public class NoteInfo
		{
			public NoteTypeID note = NoteTypeID.Def.End;

			public bool isNote => note != NoteTypeID.Def.End;
		}

		public enum DetailSetType
		{
			none,
			oldSet,
			newSet
		}

		public class Detail
		{
			public DetailSetType setType;
		}

		public float speed = 1f;

		public Detail detail = new Detail();

		public NoteInfo noteinfo = new NoteInfo();

		public NoteTypeID type;

		public NoteData parent;

		public List<NoteData> child = new List<NoteData>();

		public bool isEach;

		public List<NoteData> eachChild = new List<NoteData>();

		public int option;

		public NotesTime end;

		public SlideData slideData = new SlideData();

		public int indexNote;

		public int indexSlide;

		public int indexEach;

		public int indexTouchGroup;

		public int startButtonPos;

		public NoteTypeID.TouchArea touchArea;

		public int effect;

		public NoteTypeID.TouchSize touchSize;

		public bool isUsed;

		public bool playAnsSoundHead;

		public bool playAnsSoundTail;

		public bool isJudged;

		public NoteData()
		{
			clear();
		}

		public void clear()
		{
			speed = 1f;
			type = new NoteTypeID(NoteTypeID.Def.Invalid);
			time.clear();
			end.clear();
			slideData.init();
			indexNote = -1;
			indexSlide = -1;
			indexEach = -1;
			indexTouchGroup = -1;
			isEach = false;
			isUsed = false;
			isJudged = false;
			playAnsSoundHead = false;
			playAnsSoundTail = false;
			touchArea = NoteTypeID.TouchArea.None;
			parent = null;
			child.Clear();
			eachChild.Clear();
			startButtonPos = 0;
		}

		public NotesTime getEndTiming()
		{
			return end;
		}

		public float getLengthMsec()
		{
			return getEndTiming().msec - time.msec;
		}

		public float getLengthFrame()
		{
			return getLengthMsec() * 0.06f;
		}
	}
}
