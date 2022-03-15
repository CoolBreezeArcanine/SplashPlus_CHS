namespace DB
{
	public static class PhotoeditSettingIDEnum
	{
		private static readonly PhotoeditSettingTableRecord[] records;

		public static bool IsActive(this PhotoeditSettingID self)
		{
			if (self >= PhotoeditSettingID.Layout && self < PhotoeditSettingID.End)
			{
				return self != PhotoeditSettingID.Layout;
			}
			return false;
		}

		public static bool IsValid(this PhotoeditSettingID self)
		{
			if (self >= PhotoeditSettingID.Layout)
			{
				return self < PhotoeditSettingID.End;
			}
			return false;
		}

		public static void Clamp(this PhotoeditSettingID self)
		{
			if (self < PhotoeditSettingID.Layout)
			{
				self = PhotoeditSettingID.Layout;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PhotoeditSettingID)GetEnd();
			}
		}

		public static int GetEnd(this PhotoeditSettingID self)
		{
			return GetEnd();
		}

		public static PhotoeditSettingID FindID(string enumName)
		{
			for (PhotoeditSettingID photoeditSettingID = PhotoeditSettingID.Layout; photoeditSettingID < PhotoeditSettingID.End; photoeditSettingID++)
			{
				if (photoeditSettingID.GetEnumName() == enumName)
				{
					return photoeditSettingID;
				}
			}
			return PhotoeditSettingID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PhotoeditSettingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PhotoeditSettingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PhotoeditSettingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this PhotoeditSettingID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static PhotoeditSettingIDEnum()
		{
			records = new PhotoeditSettingTableRecord[5]
			{
				new PhotoeditSettingTableRecord(0, "Layout", "排版", string.Empty),
				new PhotoeditSettingTableRecord(1, "Stamp", "图章", string.Empty),
				new PhotoeditSettingTableRecord(2, "PlayerInfo", "玩家信息", string.Empty),
				new PhotoeditSettingTableRecord(3, "Date", "游戏日期", string.Empty),
				new PhotoeditSettingTableRecord(4, "ShopName", "店铺名", "")
			};
		}
	}
}
