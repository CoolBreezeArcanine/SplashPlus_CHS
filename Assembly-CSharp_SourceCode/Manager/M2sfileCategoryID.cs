namespace Manager
{
	public class M2sfileCategoryID
	{
		public enum Def
		{
			Begin = 0,
			Header = 0,
			Composition = 1,
			Notes = 2,
			Totals = 3,
			End = 4,
			Invalid = -1
		}

		private struct M2sfileCategory_Data
		{
			public int enumValue;

			public string enumName;

			public string name;

			public M2sfileCategory_Data(int t_enumValue, string t_enumName, string t_name)
			{
				enumValue = t_enumValue;
				enumName = t_enumName;
				name = t_name;
			}
		}

		private Def _value;

		private static M2sfileCategory_Data[] s_M2sfileCategory_Data = new M2sfileCategory_Data[4]
		{
			new M2sfileCategory_Data(0, "Header", "ヘッダ情報"),
			new M2sfileCategory_Data(1, "Composition", "構成情報"),
			new M2sfileCategory_Data(2, "Notes", "ノート情報"),
			new M2sfileCategory_Data(3, "Totals", "統計情報")
		};

		public static M2sfileCategoryID findID(string enumName)
		{
			for (Def def = Def.Begin; def < Def.End; def++)
			{
				if (s_M2sfileCategory_Data[(int)def].enumName == enumName)
				{
					return new M2sfileCategoryID(def);
				}
			}
			return new M2sfileCategoryID();
		}

		public M2sfileCategoryID(Def val = Def.Invalid)
		{
			_value = val;
		}

		public M2sfileCategoryID(M2sfileCategoryID val)
		{
			_value = val._value;
		}

		public static implicit operator M2sfileCategoryID(Def val)
		{
			return new M2sfileCategoryID(val);
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

		public static M2sfileCategoryID operator ++(M2sfileCategoryID op)
		{
			return new M2sfileCategoryID(op._value + 1);
		}

		public static M2sfileCategoryID operator --(M2sfileCategoryID op)
		{
			return new M2sfileCategoryID(op._value - 1);
		}

		public static bool operator ==(M2sfileCategoryID l, M2sfileCategoryID r)
		{
			return l._value == r._value;
		}

		public static bool operator !=(M2sfileCategoryID l, M2sfileCategoryID r)
		{
			return l._value != r._value;
		}

		public static bool operator <(M2sfileCategoryID l, M2sfileCategoryID r)
		{
			return l._value < r._value;
		}

		public static bool operator <=(M2sfileCategoryID l, M2sfileCategoryID r)
		{
			return l._value <= r._value;
		}

		public static bool operator >(M2sfileCategoryID l, M2sfileCategoryID r)
		{
			return l._value > r._value;
		}

		public static bool operator >=(M2sfileCategoryID l, M2sfileCategoryID r)
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
				_value = Def.Totals;
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
				def = Def.Totals;
			}
			return (int)def;
		}

		public int GetEnumValue()
		{
			if (isValid())
			{
				return s_M2sfileCategory_Data[(int)_value].enumValue;
			}
			return 0;
		}

		public string getEnumName()
		{
			if (isValid())
			{
				return s_M2sfileCategory_Data[(int)_value].enumName;
			}
			return "";
		}

		public string getName()
		{
			if (isValid())
			{
				return s_M2sfileCategory_Data[(int)_value].name;
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
