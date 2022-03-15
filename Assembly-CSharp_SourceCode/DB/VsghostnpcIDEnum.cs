namespace DB
{
	public static class VsghostnpcIDEnum
	{
		private static readonly VsghostnpcTableRecord[] records = new VsghostnpcTableRecord[10]
		{
			new VsghostnpcTableRecord(0, "vsNpc01", "ｍａｉｍａｉ"),
			new VsghostnpcTableRecord(1, "vsNpc02", "ｄｅｌｕｘｅ"),
			new VsghostnpcTableRecord(2, "vsNpc03", "Ｏｓａｋｉ"),
			new VsghostnpcTableRecord(3, "vsNpc04", "Ｏｔｏｒｉｉ"),
			new VsghostnpcTableRecord(4, "vsNpc05", "Ｒａｎｄ"),
			new VsghostnpcTableRecord(5, "vsNpc06", "Ｄｏｌｌｙ"),
			new VsghostnpcTableRecord(6, "vsNpc07", "Ｌｉｍｅ"),
			new VsghostnpcTableRecord(7, "vsNpc08", "Ｌｅｍｏｎ"),
			new VsghostnpcTableRecord(8, "vsNpc09", "Ｏｔｏｈｉｍｅ"),
			new VsghostnpcTableRecord(9, "vsNpc10", "Ｓｐｌａｓｈ")
		};

		public static bool IsActive(this VsghostnpcID self)
		{
			if (self >= VsghostnpcID.vsNpc01 && self < VsghostnpcID.End)
			{
				return self != VsghostnpcID.vsNpc01;
			}
			return false;
		}

		public static bool IsValid(this VsghostnpcID self)
		{
			if (self >= VsghostnpcID.vsNpc01)
			{
				return self < VsghostnpcID.End;
			}
			return false;
		}

		public static void Clamp(this VsghostnpcID self)
		{
			if (self < VsghostnpcID.vsNpc01)
			{
				self = VsghostnpcID.vsNpc01;
			}
			else if ((int)self >= GetEnd())
			{
				self = (VsghostnpcID)GetEnd();
			}
		}

		public static int GetEnd(this VsghostnpcID self)
		{
			return GetEnd();
		}

		public static VsghostnpcID FindID(string enumName)
		{
			for (VsghostnpcID vsghostnpcID = VsghostnpcID.vsNpc01; vsghostnpcID < VsghostnpcID.End; vsghostnpcID++)
			{
				if (vsghostnpcID.GetEnumName() == enumName)
				{
					return vsghostnpcID;
				}
			}
			return VsghostnpcID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this VsghostnpcID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this VsghostnpcID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this VsghostnpcID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
