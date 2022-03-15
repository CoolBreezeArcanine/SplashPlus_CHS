namespace DB
{
	public static class PartyPartyJoinResultIDEnum
	{
		private static readonly PartyPartyJoinResultTableRecord[] records = new PartyPartyJoinResultTableRecord[9]
		{
			new PartyPartyJoinResultTableRecord(0, "None", "None"),
			new PartyPartyJoinResultTableRecord(1, "Success", "Success"),
			new PartyPartyJoinResultTableRecord(2, "Full", "Full"),
			new PartyPartyJoinResultTableRecord(3, "NoRecruit", "NoRecruit"),
			new PartyPartyJoinResultTableRecord(4, "Disconnect", "Disconnect"),
			new PartyPartyJoinResultTableRecord(5, "AlreadyJoined", "AlreadyJoined"),
			new PartyPartyJoinResultTableRecord(6, "DifferentGroup", "DifferentGroup"),
			new PartyPartyJoinResultTableRecord(7, "DifferentMusic", "DifferentMusic"),
			new PartyPartyJoinResultTableRecord(8, "DifferentEventMode", "DifferentEventMode")
		};

		public static bool IsActive(this PartyPartyJoinResultID self)
		{
			if (self >= PartyPartyJoinResultID.None && self < PartyPartyJoinResultID.End)
			{
				return self != PartyPartyJoinResultID.None;
			}
			return false;
		}

		public static bool IsValid(this PartyPartyJoinResultID self)
		{
			if (self >= PartyPartyJoinResultID.None)
			{
				return self < PartyPartyJoinResultID.End;
			}
			return false;
		}

		public static void Clamp(this PartyPartyJoinResultID self)
		{
			if (self < PartyPartyJoinResultID.None)
			{
				self = PartyPartyJoinResultID.None;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PartyPartyJoinResultID)GetEnd();
			}
		}

		public static int GetEnd(this PartyPartyJoinResultID self)
		{
			return GetEnd();
		}

		public static PartyPartyJoinResultID FindID(string enumName)
		{
			for (PartyPartyJoinResultID partyPartyJoinResultID = PartyPartyJoinResultID.None; partyPartyJoinResultID < PartyPartyJoinResultID.End; partyPartyJoinResultID++)
			{
				if (partyPartyJoinResultID.GetEnumName() == enumName)
				{
					return partyPartyJoinResultID;
				}
			}
			return PartyPartyJoinResultID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PartyPartyJoinResultID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PartyPartyJoinResultID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PartyPartyJoinResultID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
