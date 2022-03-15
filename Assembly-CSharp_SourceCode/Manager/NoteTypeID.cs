namespace Manager
{
	public struct NoteTypeID
	{
		public enum Def
		{
			Begin = 0,
			Tap = 0,
			Hold = 1,
			Star = 2,
			Break = 3,
			Slide = 4,
			BreakStar = 5,
			TouchTap = 6,
			TouchHold = 7,
			ExTap = 8,
			ExHold = 9,
			ExStar = 10,
			End = 11,
			Invalid = -1
		}

		public enum TouchArea
		{
			None,
			B,
			C,
			E,
			Max
		}

		public enum TouchSize
		{
			M1,
			L1,
			Max
		}

		private struct NotesType_Data
		{
			public int enumValue;

			public string enumName;

			public string name;

			public int notesScore;

			public int notesGauge;

			public NotesType_Data(int t_enumValue, string t_enumName, string t_name, int t_notesScore, int t_notesGauge)
			{
				enumValue = t_enumValue;
				enumName = t_enumName;
				name = t_name;
				notesScore = t_notesScore;
				notesGauge = t_notesGauge;
			}
		}

		private Def _value;

		private static readonly NotesType_Data[] s_NoteType_Data = new NotesType_Data[11]
		{
			new NotesType_Data(0, "Tap", "タップ", 1, 1),
			new NotesType_Data(1, "Hold", "ホールド", 1, 1),
			new NotesType_Data(2, "Star", "☆", 1, 1),
			new NotesType_Data(3, "Break", "ブレイク", 1, 1),
			new NotesType_Data(4, "Slide", "スライド", 1, 1),
			new NotesType_Data(5, "BreakStar", "ブレイク☆", 1, 1),
			new NotesType_Data(6, "TouchTap", "タッチタップ", 1, 1),
			new NotesType_Data(7, "TouchHold", "タッチホールド", 1, 1),
			new NotesType_Data(8, "ExTap", "Exタップ", 1, 1),
			new NotesType_Data(9, "ExHold", "Exホールド", 1, 1),
			new NotesType_Data(10, "ExStar", "Ex☆", 1, 1)
		};

		public static NoteTypeID findID(string enumName)
		{
			for (Def def = Def.Begin; def < Def.End; def++)
			{
				if (s_NoteType_Data[(int)def].enumName == enumName)
				{
					return new NoteTypeID(def);
				}
			}
			return new NoteTypeID(Def.Invalid);
		}

		public NoteTypeID(Def val = Def.Invalid)
		{
			_value = val;
		}

		public NoteTypeID(NoteTypeID val)
		{
			_value = val._value;
		}

		public static implicit operator NoteTypeID(Def val)
		{
			return new NoteTypeID(val);
		}

		public void setValue(int value)
		{
			_value = (Def)value;
		}

		public Def getValue()
		{
			return _value;
		}

		public Def getEnum()
		{
			return _value;
		}

		public static NoteTypeID operator ++(NoteTypeID op)
		{
			return new NoteTypeID(op._value + 1);
		}

		public static NoteTypeID operator --(NoteTypeID op)
		{
			return new NoteTypeID(op._value - 1);
		}

		public static bool operator ==(NoteTypeID l, NoteTypeID r)
		{
			return l._value == r._value;
		}

		public static bool operator !=(NoteTypeID l, NoteTypeID r)
		{
			return l._value != r._value;
		}

		public static bool operator <(NoteTypeID l, NoteTypeID r)
		{
			return l._value < r._value;
		}

		public static bool operator <=(NoteTypeID l, NoteTypeID r)
		{
			return l._value <= r._value;
		}

		public static bool operator >(NoteTypeID l, NoteTypeID r)
		{
			return l._value > r._value;
		}

		public static bool operator >=(NoteTypeID l, NoteTypeID r)
		{
			return l._value >= r._value;
		}

		public void clamp()
		{
			if (_value < Def.Begin)
			{
				_value = Def.Begin;
			}
			if (_value >= Def.End)
			{
				_value = Def.ExStar;
			}
		}

		public void setClampValue(int value)
		{
			setValue(value);
			clamp();
		}

		public int getClampValue()
		{
			Def def = _value;
			if (def < Def.Begin)
			{
				def = Def.Begin;
			}
			if (def >= Def.End)
			{
				def = Def.ExStar;
			}
			return (int)def;
		}

		public int GetEnumValue()
		{
			if (isValid())
			{
				return s_NoteType_Data[(int)_value].enumValue;
			}
			return 0;
		}

		public string getEnumName()
		{
			if (isValid())
			{
				return s_NoteType_Data[(int)_value].enumName;
			}
			return "";
		}

		public string getName()
		{
			if (isValid())
			{
				return s_NoteType_Data[(int)_value].name;
			}
			return "";
		}

		public bool isValid()
		{
			if (Def.Begin <= _value)
			{
				return _value < Def.End;
			}
			return false;
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
