namespace Manager
{
	public struct M2sfileParamID
	{
		public enum Def
		{
			Begin = 0,
			STRING = 0,
			FLOAT = 1,
			INT = 2,
			NOTETYPE = 3,
			DIRTYPE = 4,
			End = 5,
			Invalid = -1
		}

		private struct M2sfileParam_Data
		{
			public int enumValue;

			public string enumName;

			public string name;

			public int availableStrFlag;

			public int availableIntFlag;

			public int availableFloatFlag;

			public M2sfileParam_Data(int t_enumValue, string t_enumName, string t_name, int t_availableStrFlag, int t_availableIntFlag, int t_availableFloatFlag)
			{
				enumValue = t_enumValue;
				enumName = t_enumName;
				name = t_name;
				availableStrFlag = t_availableStrFlag;
				availableIntFlag = t_availableIntFlag;
				availableFloatFlag = t_availableFloatFlag;
			}
		}

		private static M2sfileParam_Data[] s_M2sfileParam_Data = new M2sfileParam_Data[5]
		{
			new M2sfileParam_Data(0, "STRING", "文字列", 1, 0, 0),
			new M2sfileParam_Data(1, "FLOAT", "小数点", 0, 0, 1),
			new M2sfileParam_Data(2, "INT", "整数", 0, 1, 0),
			new M2sfileParam_Data(3, "NOTETYPE", "ノート", 1, 0, 0),
			new M2sfileParam_Data(4, "DIRTYPE", "方向", 1, 0, 0)
		};

		private Def _value;

		public static M2sfileParamID findID(string enumName)
		{
			for (Def def = Def.Begin; def < Def.End; def++)
			{
				if (s_M2sfileParam_Data[(int)def].enumName == enumName)
				{
					return new M2sfileParamID(def);
				}
			}
			return new M2sfileParamID(Def.Invalid);
		}

		public M2sfileParamID(Def val = Def.Invalid)
		{
			_value = val;
		}

		public M2sfileParamID(M2sfileParamID val)
		{
			_value = val._value;
		}

		public static implicit operator M2sfileParamID(Def val)
		{
			return new M2sfileParamID(val);
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

		public static M2sfileParamID operator ++(M2sfileParamID op)
		{
			return new M2sfileParamID(op._value + 1);
		}

		public static M2sfileParamID operator --(M2sfileParamID op)
		{
			return new M2sfileParamID(op._value - 1);
		}

		public static bool operator ==(M2sfileParamID l, M2sfileParamID r)
		{
			return l._value == r._value;
		}

		public static bool operator !=(M2sfileParamID l, M2sfileParamID r)
		{
			return l._value != r._value;
		}

		public static bool operator <(M2sfileParamID l, M2sfileParamID r)
		{
			return l._value < r._value;
		}

		public static bool operator <=(M2sfileParamID l, M2sfileParamID r)
		{
			return l._value <= r._value;
		}

		public static bool operator >(M2sfileParamID l, M2sfileParamID r)
		{
			return l._value > r._value;
		}

		public static bool operator >=(M2sfileParamID l, M2sfileParamID r)
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
				_value = Def.DIRTYPE;
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
				def = Def.DIRTYPE;
			}
			return (int)def;
		}

		public int GetEnumValue()
		{
			if (isValid())
			{
				return s_M2sfileParam_Data[(int)_value].enumValue;
			}
			return 0;
		}

		public string getEnumName()
		{
			if (isValid())
			{
				return s_M2sfileParam_Data[(int)_value].enumName;
			}
			return "";
		}

		public string getName()
		{
			if (isValid())
			{
				return s_M2sfileParam_Data[(int)_value].name;
			}
			return "";
		}

		public bool isAvailableStr()
		{
			if (isValid())
			{
				return s_M2sfileParam_Data[(int)_value].availableStrFlag == 1;
			}
			return false;
		}

		public bool isAvailableInt()
		{
			if (isValid())
			{
				return s_M2sfileParam_Data[(int)_value].availableIntFlag == 1;
			}
			return false;
		}

		public bool isAvailableFloat()
		{
			if (isValid())
			{
				return s_M2sfileParam_Data[(int)_value].availableFloatFlag == 1;
			}
			return false;
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
