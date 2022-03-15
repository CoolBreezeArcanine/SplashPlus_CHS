namespace DB
{
	public static class PartyHeartBeatStateIDEnum
	{
		private static readonly PartyHeartBeatStateTableRecord[] records = new PartyHeartBeatStateTableRecord[3]
		{
			new PartyHeartBeatStateTableRecord(0, "Active", "Active"),
			new PartyHeartBeatStateTableRecord(1, "Timeout", "Timeout"),
			new PartyHeartBeatStateTableRecord(2, "Closed", "Closed")
		};

		public static bool IsActive(this PartyHeartBeatStateID self)
		{
			if (self >= PartyHeartBeatStateID.Active && self < PartyHeartBeatStateID.End)
			{
				return self != PartyHeartBeatStateID.Active;
			}
			return false;
		}

		public static bool IsValid(this PartyHeartBeatStateID self)
		{
			if (self >= PartyHeartBeatStateID.Active)
			{
				return self < PartyHeartBeatStateID.End;
			}
			return false;
		}

		public static void Clamp(this PartyHeartBeatStateID self)
		{
			if (self < PartyHeartBeatStateID.Active)
			{
				self = PartyHeartBeatStateID.Active;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PartyHeartBeatStateID)GetEnd();
			}
		}

		public static int GetEnd(this PartyHeartBeatStateID self)
		{
			return GetEnd();
		}

		public static PartyHeartBeatStateID FindID(string enumName)
		{
			for (PartyHeartBeatStateID partyHeartBeatStateID = PartyHeartBeatStateID.Active; partyHeartBeatStateID < PartyHeartBeatStateID.End; partyHeartBeatStateID++)
			{
				if (partyHeartBeatStateID.GetEnumName() == enumName)
				{
					return partyHeartBeatStateID;
				}
			}
			return PartyHeartBeatStateID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PartyHeartBeatStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PartyHeartBeatStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PartyHeartBeatStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
