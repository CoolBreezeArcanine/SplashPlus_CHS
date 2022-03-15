namespace DB
{
	public static class PartyPartyHostStateIDEnum
	{
		private static readonly PartyPartyHostStateTableRecord[] records = new PartyPartyHostStateTableRecord[14]
		{
			new PartyPartyHostStateTableRecord(0, "First", "First", 0, 0, 0, 0),
			new PartyPartyHostStateTableRecord(1, "Setup", "Setup", 0, 0, 0, 0),
			new PartyPartyHostStateTableRecord(2, "Wait", "Wait", 1, 0, 0, 0),
			new PartyPartyHostStateTableRecord(3, "Recruit", "Recruit", 1, 1, 1, 0),
			new PartyPartyHostStateTableRecord(4, "FinishRecruit", "FinishRecruit", 1, 1, 0, 0),
			new PartyPartyHostStateTableRecord(5, "BeginPlay", "BeginPlay", 1, 1, 0, 0),
			new PartyPartyHostStateTableRecord(6, "AllBeginPlay", "AllBeginPlay", 1, 1, 0, 0),
			new PartyPartyHostStateTableRecord(7, "Ready", "Ready", 1, 1, 0, 0),
			new PartyPartyHostStateTableRecord(8, "Sync", "Sync", 1, 1, 0, 0),
			new PartyPartyHostStateTableRecord(9, "Play", "Play", 1, 1, 0, 1),
			new PartyPartyHostStateTableRecord(10, "News", "News", 1, 1, 0, 0),
			new PartyPartyHostStateTableRecord(11, "Result", "Result", 1, 1, 0, 0),
			new PartyPartyHostStateTableRecord(12, "Finish", "Finish", 0, 0, 0, 0),
			new PartyPartyHostStateTableRecord(13, "Error", "Error", 0, 0, 0, 0)
		};

		public static bool IsActive(this PartyPartyHostStateID self)
		{
			if (self >= PartyPartyHostStateID.First && self < PartyPartyHostStateID.End)
			{
				return self != PartyPartyHostStateID.First;
			}
			return false;
		}

		public static bool IsValid(this PartyPartyHostStateID self)
		{
			if (self >= PartyPartyHostStateID.First)
			{
				return self < PartyPartyHostStateID.End;
			}
			return false;
		}

		public static void Clamp(this PartyPartyHostStateID self)
		{
			if (self < PartyPartyHostStateID.First)
			{
				self = PartyPartyHostStateID.First;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PartyPartyHostStateID)GetEnd();
			}
		}

		public static int GetEnd(this PartyPartyHostStateID self)
		{
			return GetEnd();
		}

		public static PartyPartyHostStateID FindID(string enumName)
		{
			for (PartyPartyHostStateID partyPartyHostStateID = PartyPartyHostStateID.First; partyPartyHostStateID < PartyPartyHostStateID.End; partyPartyHostStateID++)
			{
				if (partyPartyHostStateID.GetEnumName() == enumName)
				{
					return partyPartyHostStateID;
				}
			}
			return PartyPartyHostStateID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PartyPartyHostStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PartyPartyHostStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PartyPartyHostStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static bool IsNormal(this PartyPartyHostStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isNormal;
			}
			return false;
		}

		public static bool IsWorking(this PartyPartyHostStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isWorking;
			}
			return false;
		}

		public static bool IsRecruit(this PartyPartyHostStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isRecruit;
			}
			return false;
		}

		public static bool IsPlay(this PartyPartyHostStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isPlay;
			}
			return false;
		}
	}
}
