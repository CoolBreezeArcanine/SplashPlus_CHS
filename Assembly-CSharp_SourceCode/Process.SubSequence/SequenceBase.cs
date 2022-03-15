using System;

namespace Process.SubSequence
{
	public abstract class SequenceBase
	{
		protected const float MoveTime = 100f;

		protected IMusicSelectProcessProcessing ProcessProcessing;

		protected int PlayerIndex;

		protected float Timer;

		protected bool IsChange;

		protected bool IsStart;

		protected int SlideScrollCount;

		protected bool SlideScrollToRight;

		protected float SlideScrollTime;

		public Action<int, MusicSelectProcess.SubSequence> Next;

		public Action<MusicSelectProcess.SubSequence> SyncNext;

		public bool IsValidity { get; private set; }

		protected SequenceBase(int index, bool isValidity, IMusicSelectProcessProcessing processing)
		{
			PlayerIndex = index;
			ProcessProcessing = processing;
			IsValidity = isValidity;
		}

		public abstract void Initialize();

		public virtual void OnStartSequence()
		{
		}

		public virtual bool Update()
		{
			return false;
		}

		public virtual void Reset()
		{
		}

		public virtual void OnGameStart()
		{
		}

		public virtual void OnRelease()
		{
			ProcessProcessing = null;
		}
	}
}
