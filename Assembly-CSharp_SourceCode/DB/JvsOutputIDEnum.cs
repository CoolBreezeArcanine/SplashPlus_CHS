namespace DB
{
	public static class JvsOutputIDEnum
	{
		private static readonly JvsOutputTableRecord[] records = new JvsOutputTableRecord[5]
		{
			new JvsOutputTableRecord(0, "coin_block", "coin_block", "coin_block"),
			new JvsOutputTableRecord(1, "left_reader_led", "left_reader_led", "left_reader_led"),
			new JvsOutputTableRecord(2, "right_reader_led", "right_reader_led", "right_reader_led"),
			new JvsOutputTableRecord(3, "camera_led_circle", "camera_led_circle", "camera_led_circle"),
			new JvsOutputTableRecord(4, "camera_led_red", "camera_led_red", "camera_led_red")
		};

		public static bool IsActive(this JvsOutputID self)
		{
			if (self >= JvsOutputID.coin_block && self < JvsOutputID.End)
			{
				return self != JvsOutputID.coin_block;
			}
			return false;
		}

		public static bool IsValid(this JvsOutputID self)
		{
			if (self >= JvsOutputID.coin_block)
			{
				return self < JvsOutputID.End;
			}
			return false;
		}

		public static void Clamp(this JvsOutputID self)
		{
			if (self < JvsOutputID.coin_block)
			{
				self = JvsOutputID.coin_block;
			}
			else if ((int)self >= GetEnd())
			{
				self = (JvsOutputID)GetEnd();
			}
		}

		public static int GetEnd(this JvsOutputID self)
		{
			return GetEnd();
		}

		public static JvsOutputID FindID(string enumName)
		{
			for (JvsOutputID jvsOutputID = JvsOutputID.coin_block; jvsOutputID < JvsOutputID.End; jvsOutputID++)
			{
				if (jvsOutputID.GetEnumName() == enumName)
				{
					return jvsOutputID;
				}
			}
			return JvsOutputID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this JvsOutputID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this JvsOutputID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this JvsOutputID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetOutputIDName(this JvsOutputID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].OutputIDName;
			}
			return "";
		}
	}
}
