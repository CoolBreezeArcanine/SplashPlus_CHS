namespace DB
{
	public static class PhotoeditLayoutIDEnum
	{
		private static readonly PhotoeditLayoutTableRecord[] records;

		public static bool IsActive(this PhotoeditLayoutID self)
		{
			if (self >= PhotoeditLayoutID.Normal && self < PhotoeditLayoutID.End)
			{
				return self != PhotoeditLayoutID.Normal;
			}
			return false;
		}

		public static bool IsValid(this PhotoeditLayoutID self)
		{
			if (self >= PhotoeditLayoutID.Normal)
			{
				return self < PhotoeditLayoutID.End;
			}
			return false;
		}

		public static void Clamp(this PhotoeditLayoutID self)
		{
			if (self < PhotoeditLayoutID.Normal)
			{
				self = PhotoeditLayoutID.Normal;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PhotoeditLayoutID)GetEnd();
			}
		}

		public static int GetEnd(this PhotoeditLayoutID self)
		{
			return GetEnd();
		}

		public static PhotoeditLayoutID FindID(string enumName)
		{
			for (PhotoeditLayoutID photoeditLayoutID = PhotoeditLayoutID.Normal; photoeditLayoutID < PhotoeditLayoutID.End; photoeditLayoutID++)
			{
				if (photoeditLayoutID.GetEnumName() == enumName)
				{
					return photoeditLayoutID;
				}
			}
			return PhotoeditLayoutID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PhotoeditLayoutID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PhotoeditLayoutID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PhotoeditLayoutID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this PhotoeditLayoutID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static PhotoeditLayoutIDEnum()
		{
			records = new PhotoeditLayoutTableRecord[3]
			{
				new PhotoeditLayoutTableRecord(0, "Normal", "标准", string.Empty),
				new PhotoeditLayoutTableRecord(1, "Character", "角色", string.Empty),
				new PhotoeditLayoutTableRecord(2, "AllPhoto", "照片整体", string.Empty)
			};
		}
	}
}
