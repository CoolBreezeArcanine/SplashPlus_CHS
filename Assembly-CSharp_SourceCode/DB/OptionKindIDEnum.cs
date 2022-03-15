namespace DB
{
	public static class OptionKindIDEnum
	{
		private static readonly OptionKindTableRecord[] records;

		public static bool IsActive(this OptionKindID self)
		{
			if (self >= OptionKindID.Basic && self < OptionKindID.End)
			{
				return self != OptionKindID.Basic;
			}
			return false;
		}

		public static bool IsValid(this OptionKindID self)
		{
			if (self >= OptionKindID.Basic)
			{
				return self < OptionKindID.End;
			}
			return false;
		}

		public static void Clamp(this OptionKindID self)
		{
			if (self < OptionKindID.Basic)
			{
				self = OptionKindID.Basic;
			}
			else if ((int)self >= GetEnd())
			{
				self = (OptionKindID)GetEnd();
			}
		}

		public static int GetEnd(this OptionKindID self)
		{
			return GetEnd();
		}

		public static OptionKindID FindID(string enumName)
		{
			for (OptionKindID optionKindID = OptionKindID.Basic; optionKindID < OptionKindID.End; optionKindID++)
			{
				if (optionKindID.GetEnumName() == enumName)
				{
					return optionKindID;
				}
			}
			return OptionKindID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this OptionKindID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this OptionKindID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this OptionKindID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this OptionKindID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static string GetDetail(this OptionKindID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Detail;
			}
			return "";
		}

		public static string GetDetailEx(this OptionKindID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].DetailEx;
			}
			return "";
		}

		static OptionKindIDEnum()
		{
			records = new OptionKindTableRecord[4]
			{
				new OptionKindTableRecord(0, "Basic", "适合初级玩家的设定", string.Empty, string.Empty, string.Empty),
				new OptionKindTableRecord(1, "Advance", "适合中级玩家的设定", string.Empty, string.Empty, string.Empty),
				new OptionKindTableRecord(2, "Expert", "适合高级玩家的设定", string.Empty, string.Empty, string.Empty),
				new OptionKindTableRecord(3, "Custom", "详细设定", string.Empty, string.Empty, string.Empty)
			};
		}
	}
}
