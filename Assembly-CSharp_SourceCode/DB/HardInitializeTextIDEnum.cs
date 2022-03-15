namespace DB
{
	public static class HardInitializeTextIDEnum
	{
		private static readonly HardInitializeTextTableRecord[] records;

		public static bool IsActive(this HardInitializeTextID self)
		{
			if (self >= HardInitializeTextID.TouchPanel1P && self < HardInitializeTextID.End)
			{
				return self != HardInitializeTextID.TouchPanel1P;
			}
			return false;
		}

		public static bool IsValid(this HardInitializeTextID self)
		{
			if (self >= HardInitializeTextID.TouchPanel1P)
			{
				return self < HardInitializeTextID.End;
			}
			return false;
		}

		public static void Clamp(this HardInitializeTextID self)
		{
			if (self < HardInitializeTextID.TouchPanel1P)
			{
				self = HardInitializeTextID.TouchPanel1P;
			}
			else if ((int)self >= GetEnd())
			{
				self = (HardInitializeTextID)GetEnd();
			}
		}

		public static int GetEnd(this HardInitializeTextID self)
		{
			return GetEnd();
		}

		public static HardInitializeTextID FindID(string enumName)
		{
			for (HardInitializeTextID hardInitializeTextID = HardInitializeTextID.TouchPanel1P; hardInitializeTextID < HardInitializeTextID.End; hardInitializeTextID++)
			{
				if (hardInitializeTextID.GetEnumName() == enumName)
				{
					return hardInitializeTextID;
				}
			}
			return HardInitializeTextID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this HardInitializeTextID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this HardInitializeTextID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this HardInitializeTextID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		static HardInitializeTextIDEnum()
		{
			records = new HardInitializeTextTableRecord[13]
			{
				new HardInitializeTextTableRecord(0, "TouchPanel1P", "触摸感应器(1P)"),
				new HardInitializeTextTableRecord(1, "TouchPanel2P", "触摸感应器(2P)"),
				new HardInitializeTextTableRecord(2, "Led1P", "LED(1P)"),
				new HardInitializeTextTableRecord(3, "Led2P", "LED(2P)"),
				new HardInitializeTextTableRecord(4, "Camera", "摄像机"),
				new HardInitializeTextTableRecord(5, "CodeReader1P", "二维码扫描(1P)"),
				new HardInitializeTextTableRecord(6, "CodeReader2P", "二维码扫描(2P)"),
				new HardInitializeTextTableRecord(7, "DataLoad", "检查数据"),
				new HardInitializeTextTableRecord(8, "Link", "机台间的通讯"),
				new HardInitializeTextTableRecord(9, "DeliveryServer", "正在重复确认服务器"),
				new HardInitializeTextTableRecord(10, "DeliveryClient", "正在搜寻服务器"),
				new HardInitializeTextTableRecord(11, "LinkServer", "重新检查标准设定机"),
				new HardInitializeTextTableRecord(12, "LinkClient", "取得标准设定机内的设定值")
			};
		}
	}
}
