namespace DB
{
	public static class PhotoeditDateIDEnum
	{
		private static readonly PhotoeditDateTableRecord[] records = new PhotoeditDateTableRecord[2]
		{
			new PhotoeditDateTableRecord(0, "On", "ON", ""),
			new PhotoeditDateTableRecord(1, "Off", "OFF", "")
		};

		public static bool IsActive(this PhotoeditDateID self)
		{
			if (self >= PhotoeditDateID.On && self < PhotoeditDateID.End)
			{
				return self != PhotoeditDateID.On;
			}
			return false;
		}

		public static bool IsValid(this PhotoeditDateID self)
		{
			if (self >= PhotoeditDateID.On)
			{
				return self < PhotoeditDateID.End;
			}
			return false;
		}

		public static void Clamp(this PhotoeditDateID self)
		{
			if (self < PhotoeditDateID.On)
			{
				self = PhotoeditDateID.On;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PhotoeditDateID)GetEnd();
			}
		}

		public static int GetEnd(this PhotoeditDateID self)
		{
			return GetEnd();
		}

		public static PhotoeditDateID FindID(string enumName)
		{
			for (PhotoeditDateID photoeditDateID = PhotoeditDateID.On; photoeditDateID < PhotoeditDateID.End; photoeditDateID++)
			{
				if (photoeditDateID.GetEnumName() == enumName)
				{
					return photoeditDateID;
				}
			}
			return PhotoeditDateID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PhotoeditDateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PhotoeditDateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PhotoeditDateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this PhotoeditDateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}
	}
}
