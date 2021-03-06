namespace Manager
{
	public class M2sfileRecordID
	{
		public enum Def
		{
			Begin = 0,
			VERSION = 0,
			MUSIC = 1,
			SEQUENCEID = 2,
			DIFFICULT = 3,
			LEVEL = 4,
			CREATOR = 5,
			BPM_DEF = 6,
			MET_DEF = 7,
			RESOLUTION = 8,
			CLK_DEF = 9,
			PROGJUDGE_BPM = 10,
			TUTORIAL = 11,
			FES = 12,
			BPM = 13,
			MET = 14,
			STP = 15,
			SFL = 16,
			DCM = 17,
			CLK = 18,
			TAP = 19,
			BRK = 20,
			CHL = 21,
			HLD = 22,
			STR = 23,
			BST = 24,
			FLK = 25,
			TTP = 26,
			TST = 27,
			THO = 28,
			TSL = 29,
			TFL = 30,
			XTP = 31,
			XHO = 32,
			XST = 33,
			SI_ = 34,
			SCL = 35,
			SCR = 36,
			SUL = 37,
			SUR = 38,
			SSL = 39,
			SSR = 40,
			SV_ = 41,
			SXL = 42,
			SXR = 43,
			SLL = 44,
			SLR = 45,
			SF_ = 46,
			T_REC_TAP = 47,
			T_REC_BRK = 48,
			T_REC_XTP = 49,
			T_REC_HLD = 50,
			T_REC_STR = 51,
			T_REC_BST = 52,
			T_REC_XST = 53,
			T_REC_TTP = 54,
			T_REC_THO = 55,
			T_REC_FLK = 56,
			T_REC_SLD = 57,
			T_REC_ALL = 58,
			T_NUM_TAP = 59,
			T_NUM_BRK = 60,
			T_NUM_HLD = 61,
			T_NUM_FLK = 62,
			T_NUM_SLD = 63,
			T_NUM_ALL = 64,
			TTM_EACHPAIRS = 65,
			TTM_SCR_TAP = 66,
			TTM_SCR_BRK = 67,
			TTM_SCR_HLD = 68,
			TTM_SCR_SLD = 69,
			TTM_SCR_ALL = 70,
			TTM_SCR_S = 71,
			TTM_SCR_SS = 72,
			TTM_RAT_ACV = 73,
			End = 74,
			Invalid = -1
		}

		private struct M2sfileRecord_Data
		{
			public int enumValue;

			public string enumName;

			public string name;

			public int paramNum;

			public int category;

			public int param2;

			public int param3;

			public int param4;

			public int param5;

			public int param6;

			public int param7;

			public int param8;

			public M2sfileRecord_Data(int t_enumValue, string t_enumName, string t_name, int t_paramNum, int t_category, int t_param2, int t_param3, int t_param4, int t_param5, int t_param6, int t_param7, int t_param8)
			{
				enumValue = t_enumValue;
				enumName = t_enumName;
				name = t_name;
				paramNum = t_paramNum;
				category = t_category;
				param2 = t_param2;
				param3 = t_param3;
				param4 = t_param4;
				param5 = t_param5;
				param6 = t_param6;
				param7 = t_param7;
				param8 = t_param8;
			}
		}

		private Def _value;

		private static M2sfileRecord_Data[] s_M2sfileRecord_Data = new M2sfileRecord_Data[74]
		{
			new M2sfileRecord_Data(0, "VERSION", "???????????????", 3, 0, 0, 0, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(1, "MUSIC", "???ID", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(2, "SEQUENCEID", "??????ID", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(3, "DIFFICULT", "?????????", 2, 0, 0, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(4, "LEVEL", "?????????", 2, 0, 1, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(5, "CREATOR", "????????????", 2, 0, 0, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(6, "BPM_DEF", "??????BPM", 5, 0, 1, 1, 1, 1, 1, -1, -1),
			new M2sfileRecord_Data(7, "MET_DEF", "????????????", 3, 0, 2, 2, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(8, "RESOLUTION", "?????????", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(9, "CLK_DEF", "??????????????????", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(10, "PROGJUDGE_BPM", "??????????????????BPM", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(11, "TUTORIAL", "?????????????????????", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(12, "FES", "?????????", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(13, "BPM", "BPM??????", 4, 1, 2, 2, 1, -1, -1, -1, -1),
			new M2sfileRecord_Data(14, "MET", "?????????", 5, 1, 2, 2, 2, 2, -1, -1, -1),
			new M2sfileRecord_Data(15, "STP", "????????????", 4, 1, 2, 2, 2, -1, -1, -1, -1),
			new M2sfileRecord_Data(16, "SFL", "??????????????????", 5, 1, 2, 2, 2, 1, -1, -1, -1),
			new M2sfileRecord_Data(17, "DCM", "????????????", 5, 1, 2, 2, 2, 1, -1, -1, -1),
			new M2sfileRecord_Data(18, "CLK", "???????????????", 3, 1, 2, 2, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(19, "TAP", "?????????", 5, 2, 2, 2, 2, 2, -1, -1, -1),
			new M2sfileRecord_Data(20, "BRK", "???????????????", 5, 2, 2, 2, 2, 2, -1, -1, -1),
			new M2sfileRecord_Data(21, "CHL", "???????????????", 6, 2, 2, 2, 2, 2, 4, -1, -1),
			new M2sfileRecord_Data(22, "HLD", "????????????", 6, 2, 2, 2, 2, 2, 4, -1, -1),
			new M2sfileRecord_Data(23, "STR", "???", 5, 2, 2, 2, 2, 2, -1, -1, -1),
			new M2sfileRecord_Data(24, "BST", "???????????????", 6, 2, 2, 2, 2, 2, 2, -1, -1),
			new M2sfileRecord_Data(25, "FLK", "????????????", 5, 2, 2, 2, 2, 2, -1, -1, -1),
			new M2sfileRecord_Data(26, "TTP", "??????????????????", 7, 2, 2, 2, 2, 0, 2, 0, -1),
			new M2sfileRecord_Data(27, "TST", "????????????", 5, 2, 2, 2, 2, 2, -1, -1, -1),
			new M2sfileRecord_Data(28, "THO", "?????????????????????", 8, 2, 2, 2, 2, 2, 0, 2, 0),
			new M2sfileRecord_Data(29, "TSL", "?????????????????????", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(30, "TFL", "?????????????????????", 6, 2, 2, 2, 2, 2, 2, -1, -1),
			new M2sfileRecord_Data(31, "XTP", "EX?????????", 5, 2, 2, 2, 2, 2, -1, -1, -1),
			new M2sfileRecord_Data(32, "XHO", "EX????????????", 6, 2, 2, 2, 2, 2, 4, -1, -1),
			new M2sfileRecord_Data(33, "XST", "???", 5, 2, 2, 2, 2, 2, -1, -1, -1),
			new M2sfileRecord_Data(34, "SI_", "???????????? ??????", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(35, "SCL", "???????????? ?????????", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(36, "SCR", "???????????? ?????????", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(37, "SUL", "???????????? ?????????", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(38, "SUR", "???????????? ?????????", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(39, "SSL", "???????????? ??????", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(40, "SSR", "???????????? ??????", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(41, "SV_", "???????????? ??????", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(42, "SXL", "???????????? ?????????", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(43, "SXR", "???????????? ?????????", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(44, "SLL", "???????????? ?????????", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(45, "SLR", "???????????? ?????????", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(46, "SF_", "???????????? ???????????????", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(47, "T_REC_TAP", "???????????????(TAP)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(48, "T_REC_BRK", "???????????????(BRK)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(49, "T_REC_HLD", "???????????????(HLD)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(50, "T_REC_XHO", "???????????????(XLD)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(51, "T_REC_STR", "???????????????(STR)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(52, "T_REC_BST", "???????????????(BST)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(53, "T_REC_XST", "???????????????(XST)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(54, "T_REC_TTP", "???????????????(TTP)", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(55, "T_REC_THO", "???????????????(THO)", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(56, "T_REC_FLK", "???????????????(FLK)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(57, "T_REC_SLD", "???????????????(SLD)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(58, "T_REC_ALL", "???????????????(ALL)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(59, "T_NUM_TAP", "??????????????????(TAP)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(60, "T_NUM_BRK", "??????????????????(BRK)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(61, "T_NUM_HLD", "??????????????????(HLD)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(62, "T_NUM_FLK", "??????????????????(FLK)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(63, "T_NUM_SLD", "??????????????????(SLD)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(64, "T_NUM_ALL", "??????????????????(ALL)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(65, "TTM_EACHPAIRS", "EACH?????????", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(66, "TTM_SCR_TAP", "TAP?????????", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(67, "TTM_SCR_BRK", "BREAK?????????", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(68, "TTM_SCR_HLD", "HOLD?????????", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(69, "TTM_SCR_SLD", "SLIDE?????????", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(70, "TTM_SCR_ALL", "???????????????", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(71, "TTM_SCR_S", "RankS?????????", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(72, "TTM_SCR_SS", "RankSS?????????", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(73, "TTM_RAT_ACV", "?????????", 2, 3, 2, -1, -1, -1, -1, -1, -1)
		};

		public static M2sfileRecordID findID(string enumName)
		{
			for (Def def = Def.Begin; def < Def.End; def++)
			{
				if (s_M2sfileRecord_Data[(int)def].enumName == enumName)
				{
					return new M2sfileRecordID(def);
				}
			}
			return new M2sfileRecordID();
		}

		public M2sfileRecordID(Def val = Def.Invalid)
		{
			_value = val;
		}

		public M2sfileRecordID(M2sfileRecordID val)
		{
			_value = val._value;
		}

		public static implicit operator M2sfileRecordID(Def val)
		{
			return new M2sfileRecordID(val);
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

		public static M2sfileRecordID operator ++(M2sfileRecordID op)
		{
			return new M2sfileRecordID(op._value + 1);
		}

		public static M2sfileRecordID operator --(M2sfileRecordID op)
		{
			return new M2sfileRecordID(op._value - 1);
		}

		public static bool operator ==(M2sfileRecordID l, M2sfileRecordID r)
		{
			return l._value == r._value;
		}

		public static bool operator !=(M2sfileRecordID l, M2sfileRecordID r)
		{
			return l._value != r._value;
		}

		public static bool operator <(M2sfileRecordID l, M2sfileRecordID r)
		{
			return l._value < r._value;
		}

		public static bool operator <=(M2sfileRecordID l, M2sfileRecordID r)
		{
			return l._value <= r._value;
		}

		public static bool operator >(M2sfileRecordID l, M2sfileRecordID r)
		{
			return l._value > r._value;
		}

		public static bool operator >=(M2sfileRecordID l, M2sfileRecordID r)
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
				_value = Def.TTM_RAT_ACV;
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
				def = Def.TTM_RAT_ACV;
			}
			return (int)def;
		}

		public int GetEnumValue()
		{
			if (isValid())
			{
				return s_M2sfileRecord_Data[(int)_value].enumValue;
			}
			return 0;
		}

		public string getEnumName()
		{
			if (isValid())
			{
				return s_M2sfileRecord_Data[(int)_value].enumName;
			}
			return "";
		}

		public string getName()
		{
			if (isValid())
			{
				return s_M2sfileRecord_Data[(int)_value].name;
			}
			return "";
		}

		public int getParamNum()
		{
			if (isValid())
			{
				return s_M2sfileRecord_Data[(int)_value].paramNum;
			}
			return 0;
		}

		public M2sfileCategoryID getCategory()
		{
			if (isValid())
			{
				return new M2sfileCategoryID((M2sfileCategoryID.Def)s_M2sfileRecord_Data[(int)_value].category);
			}
			return new M2sfileCategoryID();
		}

		public M2sfileParamID getParam2()
		{
			if (isValid())
			{
				return new M2sfileParamID((M2sfileParamID.Def)s_M2sfileRecord_Data[(int)_value].param2);
			}
			return new M2sfileParamID(M2sfileParamID.Def.Invalid);
		}

		public M2sfileParamID getParam3()
		{
			if (isValid())
			{
				return new M2sfileParamID((M2sfileParamID.Def)s_M2sfileRecord_Data[(int)_value].param3);
			}
			return new M2sfileParamID(M2sfileParamID.Def.Invalid);
		}

		public M2sfileParamID getParam4()
		{
			if (isValid())
			{
				return new M2sfileParamID((M2sfileParamID.Def)s_M2sfileRecord_Data[(int)_value].param4);
			}
			return new M2sfileParamID(M2sfileParamID.Def.Invalid);
		}

		public M2sfileParamID getParam5()
		{
			if (isValid())
			{
				return new M2sfileParamID((M2sfileParamID.Def)s_M2sfileRecord_Data[(int)_value].param5);
			}
			return new M2sfileParamID(M2sfileParamID.Def.Invalid);
		}

		public M2sfileParamID getParam6()
		{
			if (isValid())
			{
				return new M2sfileParamID((M2sfileParamID.Def)s_M2sfileRecord_Data[(int)_value].param6);
			}
			return new M2sfileParamID(M2sfileParamID.Def.Invalid);
		}

		public M2sfileParamID getParam7()
		{
			if (isValid())
			{
				return new M2sfileParamID((M2sfileParamID.Def)s_M2sfileRecord_Data[(int)_value].param7);
			}
			return new M2sfileParamID(M2sfileParamID.Def.Invalid);
		}

		public M2sfileParamID getParam8()
		{
			if (isValid())
			{
				return new M2sfileParamID((M2sfileParamID.Def)s_M2sfileRecord_Data[(int)_value].param8);
			}
			return new M2sfileParamID(M2sfileParamID.Def.Invalid);
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
