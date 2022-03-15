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
			new M2sfileRecord_Data(0, "VERSION", "バージョン", 3, 0, 0, 0, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(1, "MUSIC", "曲ID", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(2, "SEQUENCEID", "識別ID", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(3, "DIFFICULT", "難易度", 2, 0, 0, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(4, "LEVEL", "レベル", 2, 0, 1, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(5, "CREATOR", "譜面作者", 2, 0, 0, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(6, "BPM_DEF", "基本BPM", 5, 0, 1, 1, 1, 1, 1, -1, -1),
			new M2sfileRecord_Data(7, "MET_DEF", "基本拍数", 3, 0, 2, 2, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(8, "RESOLUTION", "解像度", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(9, "CLK_DEF", "初期クリック", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(10, "PROGJUDGE_BPM", "中間判定基準BPM", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(11, "TUTORIAL", "チュートリアル", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(12, "FES", "宴譜面", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(13, "BPM", "BPM変更", 4, 1, 2, 2, 1, -1, -1, -1, -1),
			new M2sfileRecord_Data(14, "MET", "拍変更", 5, 1, 2, 2, 2, 2, -1, -1, -1),
			new M2sfileRecord_Data(15, "STP", "停止指示", 4, 1, 2, 2, 2, -1, -1, -1, -1),
			new M2sfileRecord_Data(16, "SFL", "ソフラン指示", 5, 1, 2, 2, 2, 1, -1, -1, -1),
			new M2sfileRecord_Data(17, "DCM", "速度指示", 5, 1, 2, 2, 2, 1, -1, -1, -1),
			new M2sfileRecord_Data(18, "CLK", "クリック音", 3, 1, 2, 2, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(19, "TAP", "タップ", 5, 2, 2, 2, 2, 2, -1, -1, -1),
			new M2sfileRecord_Data(20, "BRK", "ＢＲＥＡＫ", 5, 2, 2, 2, 2, 2, -1, -1, -1),
			new M2sfileRecord_Data(21, "CHL", "旧ホールド", 6, 2, 2, 2, 2, 2, 4, -1, -1),
			new M2sfileRecord_Data(22, "HLD", "ホールド", 6, 2, 2, 2, 2, 2, 4, -1, -1),
			new M2sfileRecord_Data(23, "STR", "☆", 5, 2, 2, 2, 2, 2, -1, -1, -1),
			new M2sfileRecord_Data(24, "BST", "ブレイク☆", 6, 2, 2, 2, 2, 2, 2, -1, -1),
			new M2sfileRecord_Data(25, "FLK", "フリック", 5, 2, 2, 2, 2, 2, -1, -1, -1),
			new M2sfileRecord_Data(26, "TTP", "タッチタップ", 7, 2, 2, 2, 2, 0, 2, 0, -1),
			new M2sfileRecord_Data(27, "TST", "タッチ☆", 5, 2, 2, 2, 2, 2, -1, -1, -1),
			new M2sfileRecord_Data(28, "THO", "タッチホールド", 8, 2, 2, 2, 2, 2, 0, 2, 0),
			new M2sfileRecord_Data(29, "TSL", "タッチスライド", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(30, "TFL", "タッチフリック", 6, 2, 2, 2, 2, 2, 2, -1, -1),
			new M2sfileRecord_Data(31, "XTP", "EXタップ", 5, 2, 2, 2, 2, 2, -1, -1, -1),
			new M2sfileRecord_Data(32, "XHO", "EXホールド", 6, 2, 2, 2, 2, 2, 4, -1, -1),
			new M2sfileRecord_Data(33, "XST", "☆", 5, 2, 2, 2, 2, 2, -1, -1, -1),
			new M2sfileRecord_Data(34, "SI_", "スライド 直線", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(35, "SCL", "スライド 外周Ｌ", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(36, "SCR", "スライド 外周Ｒ", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(37, "SUL", "スライド Ｕ字Ｌ", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(38, "SUR", "スライド Ｕ字Ｒ", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(39, "SSL", "スライド 雷Ｌ", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(40, "SSR", "スライド 雷Ｒ", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(41, "SV_", "スライド Ｖ字", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(42, "SXL", "スライド 〆字Ｌ", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(43, "SXR", "スライド 〆字Ｒ", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(44, "SLL", "スライド Ｌ字Ｌ", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(45, "SLR", "スライド Ｌ字Ｒ", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(46, "SF_", "スライド 扇スライド", 7, 2, 2, 2, 2, 2, 2, 2, -1),
			new M2sfileRecord_Data(47, "T_REC_TAP", "レコード数(TAP)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(48, "T_REC_BRK", "レコード数(BRK)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(49, "T_REC_HLD", "レコード数(HLD)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(50, "T_REC_XHO", "レコード数(XLD)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(51, "T_REC_STR", "レコード数(STR)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(52, "T_REC_BST", "レコード数(BST)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(53, "T_REC_XST", "レコード数(XST)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(54, "T_REC_TTP", "レコード数(TTP)", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(55, "T_REC_THO", "レコード数(THO)", 2, 0, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(56, "T_REC_FLK", "レコード数(FLK)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(57, "T_REC_SLD", "レコード数(SLD)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(58, "T_REC_ALL", "レコード数(ALL)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(59, "T_NUM_TAP", "ノーツ登録数(TAP)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(60, "T_NUM_BRK", "ノーツ登録数(BRK)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(61, "T_NUM_HLD", "ノーツ登録数(HLD)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(62, "T_NUM_FLK", "ノーツ登録数(FLK)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(63, "T_NUM_SLD", "ノーツ登録数(SLD)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(64, "T_NUM_ALL", "ノーツ登録数(ALL)", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(65, "TTM_EACHPAIRS", "EACHペア数", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(66, "TTM_SCR_TAP", "TAPスコア", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(67, "TTM_SCR_BRK", "BREAKスコア", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(68, "TTM_SCR_HLD", "HOLDスコア", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(69, "TTM_SCR_SLD", "SLIDEスコア", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(70, "TTM_SCR_ALL", "合計スコア", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(71, "TTM_SCR_S", "RankSスコア", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(72, "TTM_SCR_SS", "RankSSスコア", 2, 3, 2, -1, -1, -1, -1, -1, -1),
			new M2sfileRecord_Data(73, "TTM_RAT_ACV", "達成率", 2, 3, 2, -1, -1, -1, -1, -1, -1)
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
