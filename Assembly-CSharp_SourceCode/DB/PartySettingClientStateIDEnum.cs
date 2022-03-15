namespace DB
{
	public static class PartySettingClientStateIDEnum
	{
		private static readonly PartySettingClientStateTableRecord[] records;

		public static bool IsActive(this PartySettingClientStateID self)
		{
			if (self >= PartySettingClientStateID.First && self < PartySettingClientStateID.End)
			{
				return self != PartySettingClientStateID.First;
			}
			return false;
		}

		public static bool IsValid(this PartySettingClientStateID self)
		{
			if (self >= PartySettingClientStateID.First)
			{
				return self < PartySettingClientStateID.End;
			}
			return false;
		}

		public static void Clamp(this PartySettingClientStateID self)
		{
			if (self < PartySettingClientStateID.First)
			{
				self = PartySettingClientStateID.First;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PartySettingClientStateID)GetEnd();
			}
		}

		public static int GetEnd(this PartySettingClientStateID self)
		{
			return GetEnd();
		}

		public static PartySettingClientStateID FindID(string enumName)
		{
			for (PartySettingClientStateID partySettingClientStateID = PartySettingClientStateID.First; partySettingClientStateID < PartySettingClientStateID.End; partySettingClientStateID++)
			{
				if (partySettingClientStateID.GetEnumName() == enumName)
				{
					return partySettingClientStateID;
				}
			}
			return PartySettingClientStateID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PartySettingClientStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PartySettingClientStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PartySettingClientStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static bool IsNormal(this PartySettingClientStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isNormal;
			}
			return false;
		}

		public static bool IsError(this PartySettingClientStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isError;
			}
			return false;
		}

		public static bool IsBusy(this PartySettingClientStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isBusy;
			}
			return false;
		}

		static PartySettingClientStateIDEnum()
		{
			records = new PartySettingClientStateTableRecord[7]
			{
				new PartySettingClientStateTableRecord(0, "First", "---", 0, 0, 0),
				new PartySettingClientStateTableRecord(1, "Search", "---", 0, 0, 0),
				new PartySettingClientStateTableRecord(2, "Connect", "---", 0, 0, 0),
				new PartySettingClientStateTableRecord(3, "Request", "---", 0, 0, 1),
				new PartySettingClientStateTableRecord(4, "Idle", "良", 1, 0, 0),
				new PartySettingClientStateTableRecord(5, "Error", "坏", 0, 1, 0),
				new PartySettingClientStateTableRecord(6, "Finish", "---", 0, 0, 0)
			};
		}
	}
}
