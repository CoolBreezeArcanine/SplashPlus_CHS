namespace DB
{
	public static class PartySettingHostStateIDEnum
	{
		private static readonly PartySettingHostStateTableRecord[] records;

		public static bool IsActive(this PartySettingHostStateID self)
		{
			if (self >= PartySettingHostStateID.First && self < PartySettingHostStateID.End)
			{
				return self != PartySettingHostStateID.First;
			}
			return false;
		}

		public static bool IsValid(this PartySettingHostStateID self)
		{
			if (self >= PartySettingHostStateID.First)
			{
				return self < PartySettingHostStateID.End;
			}
			return false;
		}

		public static void Clamp(this PartySettingHostStateID self)
		{
			if (self < PartySettingHostStateID.First)
			{
				self = PartySettingHostStateID.First;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PartySettingHostStateID)GetEnd();
			}
		}

		public static int GetEnd(this PartySettingHostStateID self)
		{
			return GetEnd();
		}

		public static PartySettingHostStateID FindID(string enumName)
		{
			for (PartySettingHostStateID partySettingHostStateID = PartySettingHostStateID.First; partySettingHostStateID < PartySettingHostStateID.End; partySettingHostStateID++)
			{
				if (partySettingHostStateID.GetEnumName() == enumName)
				{
					return partySettingHostStateID;
				}
			}
			return PartySettingHostStateID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PartySettingHostStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PartySettingHostStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PartySettingHostStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static bool IsNormal(this PartySettingHostStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isNormal;
			}
			return false;
		}

		public static bool IsError(this PartySettingHostStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isError;
			}
			return false;
		}

		static PartySettingHostStateIDEnum()
		{
			records = new PartySettingHostStateTableRecord[5]
			{
				new PartySettingHostStateTableRecord(0, "First", "---", 0, 0),
				new PartySettingHostStateTableRecord(1, "Setup", "---", 0, 0),
				new PartySettingHostStateTableRecord(2, "Active", "良", 1, 0),
				new PartySettingHostStateTableRecord(3, "Error", "坏", 0, 1),
				new PartySettingHostStateTableRecord(4, "Finish", "---", 0, 0)
			};
		}
	}
}
