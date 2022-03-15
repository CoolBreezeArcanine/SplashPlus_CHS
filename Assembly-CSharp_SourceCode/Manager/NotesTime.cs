using System;
using Note;
using UnityEngine;

namespace Manager
{
	public struct NotesTime : IComparable
	{
		private int _grid;

		private float _msec;

		public int grid => _grid;

		public float msec => _msec;

		public float frame => _msec * 0.06f;

		public NotesTime(int grid)
		{
			_grid = grid;
			_msec = 0f;
		}

		public NotesTime(int bar, int grid, NotesReader sr)
		{
			_grid = 0;
			_msec = 0f;
			init(bar, grid, sr);
		}

		public NotesTime(NotesTime time)
		{
			_grid = time.grid;
			_msec = time.msec;
		}

		public static bool operator <(NotesTime l, NotesTime r)
		{
			return l.grid < r.grid;
		}

		public static bool operator >(NotesTime l, NotesTime r)
		{
			return l.grid > r.grid;
		}

		public static bool operator <=(NotesTime l, NotesTime r)
		{
			return !(l > r);
		}

		public static bool operator >=(NotesTime l, NotesTime r)
		{
			return !(l < r);
		}

		public static bool operator ==(NotesTime l, NotesTime r)
		{
			return l.grid == r.grid;
		}

		public static bool operator !=(NotesTime l, NotesTime r)
		{
			return !(l == r);
		}

		public static NotesTime operator +(NotesTime l, NotesTime r)
		{
			return new NotesTime(l.grid + r.grid);
		}

		public static NotesTime operator -(NotesTime l, NotesTime r)
		{
			return new NotesTime(l.grid - r.grid);
		}

		public void clear()
		{
			_grid = 0;
			_msec = 0f;
		}

		public void init(int bar, int grid, NotesReader sr)
		{
			if (sr.getResolution() != 0)
			{
				_grid = bar * sr.getResolution() + grid;
				_msec = sr.calcMsec(this);
			}
		}

		public void copy(NotesTime src)
		{
			_grid = src.grid;
			_msec = src.msec;
		}

		public void calcMsec(NotesReader sr)
		{
			_msec = sr.calcMsec(this);
		}

		public void calcByMsec(NotesReader sr)
		{
			_grid = Mathf.FloorToInt(sr.calcGridByMsec(_msec) + 0.5f);
		}

		public float getBar(int resolution = 1920)
		{
			if (resolution == 0)
			{
				return 0f;
			}
			return (float)grid / (float)resolution;
		}

		public float getFourBeat(int resolution = 1920)
		{
			return getBar(resolution) * 4f;
		}

		public void setGrid(int grid)
		{
			_grid = grid;
		}

		public void setFourBeat(float beat, int resolution = 1920)
		{
			float num = beat * (float)resolution * 0.25f;
			_grid = Mathf.FloorToInt(num + 0.5f);
			_msec = 0f;
		}

		public void setRatioTime(NotesTime start, NotesTime end, float rate, NotesReader sr = null)
		{
			_msec = NoteUtil.getRate(start.msec, end.msec, rate);
			if (sr == null)
			{
				float rate2 = NoteUtil.getRate(start.grid, end.grid, rate);
				_grid = Mathf.FloorToInt(rate2 + 0.5f);
			}
			else
			{
				calcByMsec(sr);
			}
		}

		public void setMsec(float msec)
		{
			_msec = msec;
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (GetType() != obj.GetType())
			{
				throw new ArgumentException("別の型とは比較できません。", "obj");
			}
			return grid.CompareTo(((NotesTime)obj).grid);
		}

		public override bool Equals(object obj)
		{
			return true;
		}

		public override int GetHashCode()
		{
			return 0;
		}
	}
}
