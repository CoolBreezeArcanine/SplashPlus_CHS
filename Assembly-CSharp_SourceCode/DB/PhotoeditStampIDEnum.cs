namespace DB
{
	public static class PhotoeditStampIDEnum
	{
		private static readonly PhotoeditStampTableRecord[] records;

		public static bool IsActive(this PhotoeditStampID self)
		{
			if (self >= PhotoeditStampID.Stamp00 && self < PhotoeditStampID.End)
			{
				return self != PhotoeditStampID.Stamp00;
			}
			return false;
		}

		public static bool IsValid(this PhotoeditStampID self)
		{
			if (self >= PhotoeditStampID.Stamp00)
			{
				return self < PhotoeditStampID.End;
			}
			return false;
		}

		public static void Clamp(this PhotoeditStampID self)
		{
			if (self < PhotoeditStampID.Stamp00)
			{
				self = PhotoeditStampID.Stamp00;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PhotoeditStampID)GetEnd();
			}
		}

		public static int GetEnd(this PhotoeditStampID self)
		{
			return GetEnd();
		}

		public static PhotoeditStampID FindID(string enumName)
		{
			for (PhotoeditStampID photoeditStampID = PhotoeditStampID.Stamp00; photoeditStampID < PhotoeditStampID.End; photoeditStampID++)
			{
				if (photoeditStampID.GetEnumName() == enumName)
				{
					return photoeditStampID;
				}
			}
			return PhotoeditStampID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PhotoeditStampID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PhotoeditStampID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PhotoeditStampID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this PhotoeditStampID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static PhotoeditStampIDEnum()
		{
			records = new PhotoeditStampTableRecord[13]
			{
				new PhotoeditStampTableRecord(0, "Stamp00", "OFF", string.Empty),
				new PhotoeditStampTableRecord(1, "Stamp01", "?????????????????????", string.Empty),
				new PhotoeditStampTableRecord(2, "Stamp02", "?????????????????????", string.Empty),
				new PhotoeditStampTableRecord(3, "Stamp03", "??????????????????", string.Empty),
				new PhotoeditStampTableRecord(4, "Stamp04", "???????????????", string.Empty),
				new PhotoeditStampTableRecord(5, "Stamp05", "??????????????????", string.Empty),
				new PhotoeditStampTableRecord(6, "Stamp06", "???????????????", string.Empty),
				new PhotoeditStampTableRecord(7, "Stamp07", "?????????", string.Empty),
				new PhotoeditStampTableRecord(8, "Stamp08", "????????????", string.Empty),
				new PhotoeditStampTableRecord(9, "Stamp09", "???????????????????????????", string.Empty),
				new PhotoeditStampTableRecord(10, "Stamp10", "???????????????????????????", string.Empty),
				new PhotoeditStampTableRecord(11, "Stamp11", "????????????????????????", string.Empty),
				new PhotoeditStampTableRecord(12, "Stamp12", "SYNC??????????????????", string.Empty)
			};
		}
	}
}
