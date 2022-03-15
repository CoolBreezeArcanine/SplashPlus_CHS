namespace Manager
{
	public class M2sfileNotetypeID
	{
		public enum Def
		{
			Begin = 0,
			NON = 0,
			TAP = 1,
			BRK = 2,
			HLD = 3,
			STR = 4,
			BST = 5,
			TTP = 6,
			THO = 7,
			XTP = 8,
			XHO = 9,
			XST = 10,
			SI_ = 11,
			SCL = 12,
			SCR = 13,
			SUL = 14,
			SUR = 15,
			SSL = 16,
			SSR = 17,
			SV_ = 18,
			SXL = 19,
			SXR = 20,
			SLL = 21,
			SLR = 22,
			SF_ = 23,
			End = 24,
			Invalid = -1
		}

		private struct M2sfileNotetype_Data
		{
			public int enumValue;

			public string enumName;

			public string name;

			public int notetype;

			public int slidetype;

			public M2sfileNotetype_Data(int t_enumValue, string t_enumName, string t_name, int t_notetype, int t_slidetype)
			{
				enumValue = t_enumValue;
				enumName = t_enumName;
				name = t_name;
				notetype = t_notetype;
				slidetype = t_slidetype;
			}
		}

		private Def _value;

		private static M2sfileNotetype_Data[] s_M2sfileNotetype_Data = new M2sfileNotetype_Data[24]
		{
			new M2sfileNotetype_Data(0, "NON", "タグなし", -1, -1),
			new M2sfileNotetype_Data(1, "TAP", "タップタグ", 0, 0),
			new M2sfileNotetype_Data(2, "BRK", "ブレイク", 3, 0),
			new M2sfileNotetype_Data(3, "HLD", "ホールドタグ", 1, 0),
			new M2sfileNotetype_Data(4, "STR", "☆", 2, 0),
			new M2sfileNotetype_Data(5, "BST", "ブレイク☆", 5, 0),
			new M2sfileNotetype_Data(6, "TTP", "タッチタップ", 6, 0),
			new M2sfileNotetype_Data(7, "THO", "タッチホールド", 7, 0),
			new M2sfileNotetype_Data(8, "XTP", "EXタップ", 8, 0),
			new M2sfileNotetype_Data(9, "XHO", "EXホールド", 9, 0),
			new M2sfileNotetype_Data(10, "XST", "EX☆", 10, 0),
			new M2sfileNotetype_Data(11, "SI_", "スライド 直線", 4, 1),
			new M2sfileNotetype_Data(12, "SCL", "スライド 外周Ｌ", 4, 2),
			new M2sfileNotetype_Data(13, "SCR", "スライド 外周Ｒ", 4, 3),
			new M2sfileNotetype_Data(14, "SUL", "スライド Ｕ字Ｌ", 4, 4),
			new M2sfileNotetype_Data(15, "SUR", "スライド Ｕ字Ｒ", 4, 5),
			new M2sfileNotetype_Data(16, "SSL", "スライド 雷Ｌ", 4, 6),
			new M2sfileNotetype_Data(17, "SSR", "スライド 雷Ｒ", 4, 7),
			new M2sfileNotetype_Data(18, "SV_", "スライド Ｖ字", 4, 8),
			new M2sfileNotetype_Data(19, "SXL", "スライド 〆字Ｌ", 4, 9),
			new M2sfileNotetype_Data(20, "SXR", "スライド 〆字Ｒ", 4, 10),
			new M2sfileNotetype_Data(21, "SLL", "タグスライド Ｌ字Ｌ", 4, 11),
			new M2sfileNotetype_Data(22, "SLR", "スライド Ｌ字Ｒ", 4, 12),
			new M2sfileNotetype_Data(23, "SF_", "スライド 扇スライド", 4, 13)
		};

		public static M2sfileNotetypeID findID(string enumName)
		{
			for (Def def = Def.Begin; def < Def.End; def++)
			{
				if (s_M2sfileNotetype_Data[(int)def].enumName == enumName)
				{
					return new M2sfileNotetypeID(def);
				}
			}
			return new M2sfileNotetypeID();
		}

		public M2sfileNotetypeID(Def val = Def.Invalid)
		{
			_value = val;
		}

		public M2sfileNotetypeID(M2sfileNotetypeID val)
		{
			_value = val._value;
		}

		public static implicit operator M2sfileNotetypeID(Def val)
		{
			return new M2sfileNotetypeID(val);
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

		public static M2sfileNotetypeID operator ++(M2sfileNotetypeID op)
		{
			return new M2sfileNotetypeID(op._value + 1);
		}

		public static M2sfileNotetypeID operator --(M2sfileNotetypeID op)
		{
			return new M2sfileNotetypeID(op._value - 1);
		}

		public static bool operator ==(M2sfileNotetypeID l, M2sfileNotetypeID r)
		{
			return l._value == r._value;
		}

		public static bool operator !=(M2sfileNotetypeID l, M2sfileNotetypeID r)
		{
			return l._value != r._value;
		}

		public static bool operator <(M2sfileNotetypeID l, M2sfileNotetypeID r)
		{
			return l._value < r._value;
		}

		public static bool operator <=(M2sfileNotetypeID l, M2sfileNotetypeID r)
		{
			return l._value <= r._value;
		}

		public static bool operator >(M2sfileNotetypeID l, M2sfileNotetypeID r)
		{
			return l._value > r._value;
		}

		public static bool operator >=(M2sfileNotetypeID l, M2sfileNotetypeID r)
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
				_value = Def.SF_;
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
				def = Def.SF_;
			}
			return (int)def;
		}

		public int GetEnumValue()
		{
			if (isValid())
			{
				return s_M2sfileNotetype_Data[(int)_value].enumValue;
			}
			return 0;
		}

		public string getEnumName()
		{
			if (isValid())
			{
				return s_M2sfileNotetype_Data[(int)_value].enumName;
			}
			return "";
		}

		public string getName()
		{
			if (isValid())
			{
				return s_M2sfileNotetype_Data[(int)_value].name;
			}
			return "";
		}

		public NoteTypeID getNotetype()
		{
			if (isValid())
			{
				return new NoteTypeID((NoteTypeID.Def)s_M2sfileNotetype_Data[(int)_value].notetype);
			}
			return new NoteTypeID(NoteTypeID.Def.Invalid);
		}

		public SlideType getSlidetype()
		{
			if (isValid())
			{
				return (SlideType)s_M2sfileNotetype_Data[(int)_value].slidetype;
			}
			return SlideType.Slide_INVALID;
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
