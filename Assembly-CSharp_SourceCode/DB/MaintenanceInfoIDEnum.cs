namespace DB
{
	public static class MaintenanceInfoIDEnum
	{
		private static readonly MaintenanceInfoTableRecord[] records;

		public static bool IsActive(this MaintenanceInfoID self)
		{
			if (self >= MaintenanceInfoID.ShowNotice && self < MaintenanceInfoID.End)
			{
				return self != MaintenanceInfoID.ShowNotice;
			}
			return false;
		}

		public static bool IsValid(this MaintenanceInfoID self)
		{
			if (self >= MaintenanceInfoID.ShowNotice)
			{
				return self < MaintenanceInfoID.End;
			}
			return false;
		}

		public static void Clamp(this MaintenanceInfoID self)
		{
			if (self < MaintenanceInfoID.ShowNotice)
			{
				self = MaintenanceInfoID.ShowNotice;
			}
			else if ((int)self >= GetEnd())
			{
				self = (MaintenanceInfoID)GetEnd();
			}
		}

		public static int GetEnd(this MaintenanceInfoID self)
		{
			return GetEnd();
		}

		public static MaintenanceInfoID FindID(string enumName)
		{
			for (MaintenanceInfoID maintenanceInfoID = MaintenanceInfoID.ShowNotice; maintenanceInfoID < MaintenanceInfoID.End; maintenanceInfoID++)
			{
				if (maintenanceInfoID.GetEnumName() == enumName)
				{
					return maintenanceInfoID;
				}
			}
			return MaintenanceInfoID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this MaintenanceInfoID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this MaintenanceInfoID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this MaintenanceInfoID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		static MaintenanceInfoIDEnum()
		{
			records = new MaintenanceInfoTableRecord[7]
			{
				new MaintenanceInfoTableRecord(0, "ShowNotice", "还有{0}分钟游戏报名就要结束了"),
				new MaintenanceInfoTableRecord(1, "ShowEndReception", "今天的报名已结束"),
				new MaintenanceInfoTableRecord(2, "MaintenanceReboot", "因系统维护\r\n正在重启"),
				new MaintenanceInfoTableRecord(3, "DataReboot", "因更新数据\r\n正在重启"),
				new MaintenanceInfoTableRecord(4, "Reporting", "正在进行网络连接\r\n请稍等"),
				new MaintenanceInfoTableRecord(5, "RefreshReboot", "システム安定化のため再起動を行います"),
				new MaintenanceInfoTableRecord(6, "RefreshNotice", "{0}分後に、システム安定化のため再起動を行います")
			};
		}
	}
}
