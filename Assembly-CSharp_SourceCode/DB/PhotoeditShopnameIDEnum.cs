namespace DB
{
	public static class PhotoeditShopnameIDEnum
	{
		private static readonly PhotoeditShopnameTableRecord[] records = new PhotoeditShopnameTableRecord[2]
		{
			new PhotoeditShopnameTableRecord(0, "On", "ON", ""),
			new PhotoeditShopnameTableRecord(1, "Off", "OFF", "")
		};

		public static bool IsActive(this PhotoeditShopnameID self)
		{
			if (self >= PhotoeditShopnameID.On && self < PhotoeditShopnameID.End)
			{
				return self != PhotoeditShopnameID.On;
			}
			return false;
		}

		public static bool IsValid(this PhotoeditShopnameID self)
		{
			if (self >= PhotoeditShopnameID.On)
			{
				return self < PhotoeditShopnameID.End;
			}
			return false;
		}

		public static void Clamp(this PhotoeditShopnameID self)
		{
			if (self < PhotoeditShopnameID.On)
			{
				self = PhotoeditShopnameID.On;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PhotoeditShopnameID)GetEnd();
			}
		}

		public static int GetEnd(this PhotoeditShopnameID self)
		{
			return GetEnd();
		}

		public static PhotoeditShopnameID FindID(string enumName)
		{
			for (PhotoeditShopnameID photoeditShopnameID = PhotoeditShopnameID.On; photoeditShopnameID < PhotoeditShopnameID.End; photoeditShopnameID++)
			{
				if (photoeditShopnameID.GetEnumName() == enumName)
				{
					return photoeditShopnameID;
				}
			}
			return PhotoeditShopnameID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PhotoeditShopnameID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PhotoeditShopnameID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PhotoeditShopnameID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this PhotoeditShopnameID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}
	}
}
