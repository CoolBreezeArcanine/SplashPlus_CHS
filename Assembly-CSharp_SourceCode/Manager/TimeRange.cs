namespace Manager
{
	public class TimeRange : TimingBase
	{
		public NotesTime _end;

		public NotesTime start
		{
			get
			{
				return time;
			}
			set
			{
				if (_end >= value)
				{
					time.copy(value);
					return;
				}
				time.copy(_end);
				_end.copy(value);
			}
		}

		public NotesTime end
		{
			get
			{
				return _end;
			}
			set
			{
				if (time <= value)
				{
					_end.copy(value);
					return;
				}
				_end.copy(time);
				time.copy(value);
			}
		}

		public TimeRange()
		{
			init();
		}

		public TimeRange(TimeRange src)
		{
			copy(src);
		}

		public TimeRange(NotesTime from, NotesTime to)
		{
			if (from <= to)
			{
				time.copy(from);
				_end.copy(to);
			}
			else
			{
				time.copy(to);
				_end.copy(from);
			}
		}

		public new void init()
		{
			base.init();
			_end.clear();
		}

		public void copy(TimeRange src)
		{
			time.copy(src.start);
			_end.copy(src.end);
		}

		public bool isIn(NotesTime t)
		{
			if (start <= t)
			{
				return t <= end;
			}
			return false;
		}

		public bool isHit(TimeRange src)
		{
			if (start > src.end)
			{
				return false;
			}
			if (end < src.start)
			{
				return false;
			}
			return true;
		}

		public bool merge(TimeRange src)
		{
			if (!isHit(src))
			{
				return false;
			}
			if (start > src.start)
			{
				time.copy(src.start);
			}
			if (end < src.end)
			{
				_end.copy(src.end);
			}
			return true;
		}
	}
}
