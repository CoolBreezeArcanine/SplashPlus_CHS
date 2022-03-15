namespace DB
{
	public static class JvsButtonIDEnum
	{
		private static readonly JvsButtonTableRecord[] records = new JvsButtonTableRecord[20]
		{
			new JvsButtonTableRecord(0, "Test", "Test", -1, "test", 115, 0, 1),
			new JvsButtonTableRecord(1, "Service", "Service", 0, "service", 5, 0, 1),
			new JvsButtonTableRecord(2, "Button1_1P", "Button1_1P", 0, "button_01", 67, 1, 0),
			new JvsButtonTableRecord(3, "Button2_1P", "Button2_1P", 0, "button_02", 49, 1, 0),
			new JvsButtonTableRecord(4, "Button3_1P", "Button3_1P", 0, "button_03", 48, 1, 0),
			new JvsButtonTableRecord(5, "Button4_1P", "Button4_1P", 0, "button_04", 47, 1, 0),
			new JvsButtonTableRecord(6, "Button5_1P", "Button5_1P", 0, "button_05", 68, 1, 0),
			new JvsButtonTableRecord(7, "Button6_1P", "Button6_1P", 0, "button_06", 70, 1, 0),
			new JvsButtonTableRecord(8, "Button7_1P", "Button7_1P", 0, "button_07", 45, 1, 0),
			new JvsButtonTableRecord(9, "Button8_1P", "Button8_1P", 0, "button_08", 61, 1, 0),
			new JvsButtonTableRecord(10, "Select_1P", "Select_1P", 0, "select", 25, 0, 0),
			new JvsButtonTableRecord(11, "Button1_2P", "Button1_2P", 1, "button_01", 80, 1, 0),
			new JvsButtonTableRecord(12, "Button2_2P", "Button2_2P", 1, "button_02", 81, 1, 0),
			new JvsButtonTableRecord(13, "Button3_2P", "Button3_2P", 1, "button_03", 78, 1, 0),
			new JvsButtonTableRecord(14, "Button4_2P", "Button4_2P", 1, "button_04", 75, 1, 0),
			new JvsButtonTableRecord(15, "Button5_2P", "Button5_2P", 1, "button_05", 74, 1, 0),
			new JvsButtonTableRecord(16, "Button6_2P", "Button6_2P", 1, "button_06", 73, 1, 0),
			new JvsButtonTableRecord(17, "Button7_2P", "Button7_2P", 1, "button_07", 76, 1, 0),
			new JvsButtonTableRecord(18, "Button8_2P", "Button8_2P", 1, "button_08", 79, 1, 0),
			new JvsButtonTableRecord(19, "Select_2P", "Select_2P", 1, "select", 84, 0, 0)
		};

		public static bool IsActive(this JvsButtonID self)
		{
			if (self >= JvsButtonID.Test && self < JvsButtonID.End)
			{
				return self != JvsButtonID.Test;
			}
			return false;
		}

		public static bool IsValid(this JvsButtonID self)
		{
			if (self >= JvsButtonID.Test)
			{
				return self < JvsButtonID.End;
			}
			return false;
		}

		public static void Clamp(this JvsButtonID self)
		{
			if (self < JvsButtonID.Test)
			{
				self = JvsButtonID.Test;
			}
			else if ((int)self >= GetEnd())
			{
				self = (JvsButtonID)GetEnd();
			}
		}

		public static int GetEnd(this JvsButtonID self)
		{
			return GetEnd();
		}

		public static JvsButtonID FindID(string enumName)
		{
			for (JvsButtonID jvsButtonID = JvsButtonID.Test; jvsButtonID < JvsButtonID.End; jvsButtonID++)
			{
				if (jvsButtonID.GetEnumName() == enumName)
				{
					return jvsButtonID;
				}
			}
			return JvsButtonID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this JvsButtonID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this JvsButtonID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this JvsButtonID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static int GetJvsPlayer(this JvsButtonID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].JvsPlayer;
			}
			return 0;
		}

		public static string GetInputIDName(this JvsButtonID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].InputIDName;
			}
			return "";
		}

		public static KeyCodeID GetSubstituteKey(this JvsButtonID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].SubstituteKey;
			}
			return KeyCodeID.Invalid;
		}

		public static int GetInvert(this JvsButtonID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Invert;
			}
			return 0;
		}

		public static int GetSystem(this JvsButtonID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].System;
			}
			return 0;
		}
	}
}
