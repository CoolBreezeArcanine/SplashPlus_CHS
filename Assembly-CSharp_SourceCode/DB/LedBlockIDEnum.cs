namespace DB
{
	public static class LedBlockIDEnum
	{
		private static readonly LedBlockTableRecord[] records = new LedBlockTableRecord[28]
		{
			new LedBlockTableRecord(0, "Button1_1P", "1Pボタン1LED", 0, 0, 0, 0),
			new LedBlockTableRecord(1, "Button2_1P", "1Pボタン2LED", 1, 0, 0, 0),
			new LedBlockTableRecord(2, "Button3_1P", "1Pボタン3LED", 2, 0, 0, 0),
			new LedBlockTableRecord(3, "Button4_1P", "1Pボタン4LED", 3, 0, 0, 0),
			new LedBlockTableRecord(4, "Button5_1P", "1Pボタン5LED", 4, 0, 0, 0),
			new LedBlockTableRecord(5, "Button6_1P", "1Pボタン6LED", 5, 0, 0, 0),
			new LedBlockTableRecord(6, "Button7_1P", "1Pボタン7LED", 6, 0, 0, 0),
			new LedBlockTableRecord(7, "Button8_1P", "1Pボタン8LED", 7, 0, 0, 0),
			new LedBlockTableRecord(8, "Body_1P", "1PボディLED", 8, 0, 0, 1),
			new LedBlockTableRecord(9, "Circle_1P", "1P画面外周LED", 9, 0, 0, 1),
			new LedBlockTableRecord(10, "Side_1P", "1P横LED", 10, 0, 0, 1),
			new LedBlockTableRecord(11, "Billboard_1P", "1P上部LED", -1, 0, 1, 0),
			new LedBlockTableRecord(12, "QrLed_1P", "1PQRLED", 1, -1, 1, 0),
			new LedBlockTableRecord(13, "Button1_2P", "2Pボタン1LED", 0, 1, 0, 0),
			new LedBlockTableRecord(14, "Button2_2P", "2Pボタン2LED", 1, 1, 0, 0),
			new LedBlockTableRecord(15, "Button3_2P", "2Pボタン3LED", 2, 1, 0, 0),
			new LedBlockTableRecord(16, "Button4_2P", "2Pボタン4LED", 3, 1, 0, 0),
			new LedBlockTableRecord(17, "Button5_2P", "2Pボタン5LED", 4, 1, 0, 0),
			new LedBlockTableRecord(18, "Button6_2P", "2Pボタン6LED", 5, 1, 0, 0),
			new LedBlockTableRecord(19, "Button7_2P", "2Pボタン7LED", 6, 1, 0, 0),
			new LedBlockTableRecord(20, "Button8_2P", "2Pボタン8LED", 7, 1, 0, 0),
			new LedBlockTableRecord(21, "Body_2P", "2PボディLED", 8, 1, 0, 1),
			new LedBlockTableRecord(22, "Circle_2P", "2P画面外周LED", 9, 1, 0, 1),
			new LedBlockTableRecord(23, "Side_2P", "2P横LED", 10, 1, 0, 1),
			new LedBlockTableRecord(24, "Billboard_2P", "2P上部LED", -1, 1, 1, 0),
			new LedBlockTableRecord(25, "QrLed_2P", "2PQRLED", 2, -1, 1, 0),
			new LedBlockTableRecord(26, "Cam", "カメラ（左のみ）", 3, -1, 1, 0),
			new LedBlockTableRecord(27, "CamRec", "カメラRec", 4, -1, 1, 0)
		};

		public static bool IsActive(this LedBlockID self)
		{
			if (self >= LedBlockID.Button1_1P && self < LedBlockID.End)
			{
				return self != LedBlockID.Button1_1P;
			}
			return false;
		}

		public static bool IsValid(this LedBlockID self)
		{
			if (self >= LedBlockID.Button1_1P)
			{
				return self < LedBlockID.End;
			}
			return false;
		}

		public static void Clamp(this LedBlockID self)
		{
			if (self < LedBlockID.Button1_1P)
			{
				self = LedBlockID.Button1_1P;
			}
			else if ((int)self >= GetEnd())
			{
				self = (LedBlockID)GetEnd();
			}
		}

		public static int GetEnd(this LedBlockID self)
		{
			return GetEnd();
		}

		public static LedBlockID FindID(string enumName)
		{
			for (LedBlockID ledBlockID = LedBlockID.Button1_1P; ledBlockID < LedBlockID.End; ledBlockID++)
			{
				if (ledBlockID.GetEnumName() == enumName)
				{
					return ledBlockID;
				}
			}
			return LedBlockID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this LedBlockID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this LedBlockID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this LedBlockID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static int GetLedbdID(this LedBlockID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].LedbdID;
			}
			return 0;
		}

		public static int GetPlayerindex(this LedBlockID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Playerindex;
			}
			return 0;
		}

		public static bool IsJvs(this LedBlockID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isJvs;
			}
			return false;
		}

		public static bool IsFet(this LedBlockID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isFet;
			}
			return false;
		}
	}
}
