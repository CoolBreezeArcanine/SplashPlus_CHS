namespace DB
{
	public static class PartyPartyStanceIDEnum
	{
		private static readonly PartyPartyStanceTableRecord[] records = new PartyPartyStanceTableRecord[2]
		{
			new PartyPartyStanceTableRecord(0, "Anyone", "誰でもOK!"),
			new PartyPartyStanceTableRecord(1, "Friends", "フレンドと遊ぶ")
		};

		public static bool IsActive(this PartyPartyStanceID self)
		{
			if (self >= PartyPartyStanceID.Anyone && self < PartyPartyStanceID.End)
			{
				return self != PartyPartyStanceID.Anyone;
			}
			return false;
		}

		public static bool IsValid(this PartyPartyStanceID self)
		{
			if (self >= PartyPartyStanceID.Anyone)
			{
				return self < PartyPartyStanceID.End;
			}
			return false;
		}

		public static void Clamp(this PartyPartyStanceID self)
		{
			if (self < PartyPartyStanceID.Anyone)
			{
				self = PartyPartyStanceID.Anyone;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PartyPartyStanceID)GetEnd();
			}
		}

		public static int GetEnd(this PartyPartyStanceID self)
		{
			return GetEnd();
		}

		public static PartyPartyStanceID FindID(string enumName)
		{
			for (PartyPartyStanceID partyPartyStanceID = PartyPartyStanceID.Anyone; partyPartyStanceID < PartyPartyStanceID.End; partyPartyStanceID++)
			{
				if (partyPartyStanceID.GetEnumName() == enumName)
				{
					return partyPartyStanceID;
				}
			}
			return PartyPartyStanceID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PartyPartyStanceID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PartyPartyStanceID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PartyPartyStanceID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
