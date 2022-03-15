namespace DB
{
	public static class SortRootIDEnum
	{
		private static readonly SortRootTableRecord[] records;

		public static bool IsActive(this SortRootID self)
		{
			if (self >= SortRootID.Tab && self < SortRootID.End)
			{
				return self != SortRootID.Tab;
			}
			return false;
		}

		public static bool IsValid(this SortRootID self)
		{
			if (self >= SortRootID.Tab)
			{
				return self < SortRootID.End;
			}
			return false;
		}

		public static void Clamp(this SortRootID self)
		{
			if (self < SortRootID.Tab)
			{
				self = SortRootID.Tab;
			}
			else if ((int)self >= GetEnd())
			{
				self = (SortRootID)GetEnd();
			}
		}

		public static int GetEnd(this SortRootID self)
		{
			return GetEnd();
		}

		public static SortRootID FindID(string enumName)
		{
			for (SortRootID sortRootID = SortRootID.Tab; sortRootID < SortRootID.End; sortRootID++)
			{
				if (sortRootID.GetEnumName() == enumName)
				{
					return sortRootID;
				}
			}
			return SortRootID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this SortRootID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this SortRootID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this SortRootID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this SortRootID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static SortRootIDEnum()
		{
			records = new SortRootTableRecord[2]
			{
				new SortRootTableRecord(0, "Tab", "种类设定", string.Empty),
				new SortRootTableRecord(1, "Music", "排序设定", string.Empty)
			};
		}
	}
}
