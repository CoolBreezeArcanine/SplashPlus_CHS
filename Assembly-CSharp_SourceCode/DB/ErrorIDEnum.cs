namespace DB
{
	public static class ErrorIDEnum
	{
		private static readonly ErrorTableRecord[] records = new ErrorTableRecord[10]
		{
			new ErrorTableRecord(0, "LedBoard_Left_InitError", "LedBoard_Left_InitError", 1, 0, 0),
			new ErrorTableRecord(1, "LedBoard_Left_Timeout", "LedBoard_Left_Timeout", 1, 0, 0),
			new ErrorTableRecord(2, "LedBoard_Left_ResponseError", "LedBoard_Left_ResponseError", 1, 0, 0),
			new ErrorTableRecord(3, "LedBoard_Left_FirmError", "LedBoard_Left_FirmError", 0, 1, 0),
			new ErrorTableRecord(4, "LedBoard_Left_FirmVerError", "LedBoard_Left_FirmVerError", 0, 1, 0),
			new ErrorTableRecord(5, "LedBoard_Right_InitError", "LedBoard_Right_InitError", 1, 0, 0),
			new ErrorTableRecord(6, "LedBoard_Right_Timeout", "LedBoard_Right_Timeout", 1, 0, 0),
			new ErrorTableRecord(7, "LedBoard_Right_ResponseError", "LedBoard_Right_ResponseError", 1, 0, 0),
			new ErrorTableRecord(8, "LedBoard_Right_FirmError", "LedBoard_Right_FirmError", 0, 1, 0),
			new ErrorTableRecord(9, "LedBoard_Right_FirmVerError", "LedBoard_Right_FirmVerError", 0, 1, 0)
		};

		public static bool IsActive(this ErrorID self)
		{
			if (self >= ErrorID.LedBoard_Left_InitError && self < ErrorID.End)
			{
				return self != ErrorID.LedBoard_Left_InitError;
			}
			return false;
		}

		public static bool IsValid(this ErrorID self)
		{
			if (self >= ErrorID.LedBoard_Left_InitError)
			{
				return self < ErrorID.End;
			}
			return false;
		}

		public static void Clamp(this ErrorID self)
		{
			if (self < ErrorID.LedBoard_Left_InitError)
			{
				self = ErrorID.LedBoard_Left_InitError;
			}
			else if ((int)self >= GetEnd())
			{
				self = (ErrorID)GetEnd();
			}
		}

		public static int GetEnd(this ErrorID self)
		{
			return GetEnd();
		}

		public static ErrorID FindID(string enumName)
		{
			for (ErrorID errorID = ErrorID.LedBoard_Left_InitError; errorID < ErrorID.End; errorID++)
			{
				if (errorID.GetEnumName() == enumName)
				{
					return errorID;
				}
			}
			return ErrorID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this ErrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this ErrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this ErrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static bool IsWarning(this ErrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isWarning;
			}
			return false;
		}

		public static bool IsFirmUpdate(this ErrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isFirmUpdate;
			}
			return false;
		}

		public static int GetCode(this ErrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Code;
			}
			return 0;
		}
	}
}
