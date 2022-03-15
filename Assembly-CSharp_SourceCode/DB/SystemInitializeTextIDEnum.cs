namespace DB
{
	public static class SystemInitializeTextIDEnum
	{
		private static readonly SystemInitializeTextTableRecord[] records;

		public static bool IsActive(this SystemInitializeTextID self)
		{
			if (self >= SystemInitializeTextID.SystemCheck && self < SystemInitializeTextID.End)
			{
				return self != SystemInitializeTextID.SystemCheck;
			}
			return false;
		}

		public static bool IsValid(this SystemInitializeTextID self)
		{
			if (self >= SystemInitializeTextID.SystemCheck)
			{
				return self < SystemInitializeTextID.End;
			}
			return false;
		}

		public static void Clamp(this SystemInitializeTextID self)
		{
			if (self < SystemInitializeTextID.SystemCheck)
			{
				self = SystemInitializeTextID.SystemCheck;
			}
			else if ((int)self >= GetEnd())
			{
				self = (SystemInitializeTextID)GetEnd();
			}
		}

		public static int GetEnd(this SystemInitializeTextID self)
		{
			return GetEnd();
		}

		public static SystemInitializeTextID FindID(string enumName)
		{
			for (SystemInitializeTextID systemInitializeTextID = SystemInitializeTextID.SystemCheck; systemInitializeTextID < SystemInitializeTextID.End; systemInitializeTextID++)
			{
				if (systemInitializeTextID.GetEnumName() == enumName)
				{
					return systemInitializeTextID;
				}
			}
			return SystemInitializeTextID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this SystemInitializeTextID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this SystemInitializeTextID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this SystemInitializeTextID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		static SystemInitializeTextIDEnum()
		{
			records = new SystemInitializeTextTableRecord[5]
			{
				new SystemInitializeTextTableRecord(0, "SystemCheck", "系统启动中"),
				new SystemInitializeTextTableRecord(1, "SystemWait", "启动中"),
				new SystemInitializeTextTableRecord(2, "DataCheck", "游戏数据"),
				new SystemInitializeTextTableRecord(3, "DataWait", "初始化中"),
				new SystemInitializeTextTableRecord(4, "AimeReader", "Aime读卡器")
			};
		}
	}
}
