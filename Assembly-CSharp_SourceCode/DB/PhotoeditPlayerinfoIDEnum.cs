namespace DB
{
	public static class PhotoeditPlayerinfoIDEnum
	{
		private static readonly PhotoeditPlayerinfoTableRecord[] records = new PhotoeditPlayerinfoTableRecord[2]
		{
			new PhotoeditPlayerinfoTableRecord(0, "On", "ON", ""),
			new PhotoeditPlayerinfoTableRecord(1, "Off", "OFF", "")
		};

		public static bool IsActive(this PhotoeditPlayerinfoID self)
		{
			if (self >= PhotoeditPlayerinfoID.On && self < PhotoeditPlayerinfoID.End)
			{
				return self != PhotoeditPlayerinfoID.On;
			}
			return false;
		}

		public static bool IsValid(this PhotoeditPlayerinfoID self)
		{
			if (self >= PhotoeditPlayerinfoID.On)
			{
				return self < PhotoeditPlayerinfoID.End;
			}
			return false;
		}

		public static void Clamp(this PhotoeditPlayerinfoID self)
		{
			if (self < PhotoeditPlayerinfoID.On)
			{
				self = PhotoeditPlayerinfoID.On;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PhotoeditPlayerinfoID)GetEnd();
			}
		}

		public static int GetEnd(this PhotoeditPlayerinfoID self)
		{
			return GetEnd();
		}

		public static PhotoeditPlayerinfoID FindID(string enumName)
		{
			for (PhotoeditPlayerinfoID photoeditPlayerinfoID = PhotoeditPlayerinfoID.On; photoeditPlayerinfoID < PhotoeditPlayerinfoID.End; photoeditPlayerinfoID++)
			{
				if (photoeditPlayerinfoID.GetEnumName() == enumName)
				{
					return photoeditPlayerinfoID;
				}
			}
			return PhotoeditPlayerinfoID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PhotoeditPlayerinfoID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PhotoeditPlayerinfoID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PhotoeditPlayerinfoID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this PhotoeditPlayerinfoID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}
	}
}
