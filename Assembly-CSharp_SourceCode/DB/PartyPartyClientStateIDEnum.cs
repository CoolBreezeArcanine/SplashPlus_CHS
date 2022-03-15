namespace DB
{
	public static class PartyPartyClientStateIDEnum
	{
		private static readonly PartyPartyClientStateTableRecord[] records = new PartyPartyClientStateTableRecord[20]
		{
			new PartyPartyClientStateTableRecord(0, "First", "First", 0, 0, 0, 0),
			new PartyPartyClientStateTableRecord(1, "Setup", "Setup", 0, 0, 0, 0),
			new PartyPartyClientStateTableRecord(2, "Wait", "Wait", 0, 1, 0, 0),
			new PartyPartyClientStateTableRecord(3, "Connect", "Connect", 0, 1, 1, 0),
			new PartyPartyClientStateTableRecord(4, "Request", "Request", 1, 1, 1, 0),
			new PartyPartyClientStateTableRecord(5, "Joined", "Joined", 1, 1, 0, 0),
			new PartyPartyClientStateTableRecord(6, "FinishSetting", "FinishSetting", 1, 1, 0, 1),
			new PartyPartyClientStateTableRecord(7, "ToReady", "ToReady", 1, 1, 0, 1),
			new PartyPartyClientStateTableRecord(8, "BeginPlay", "BeginPlay", 1, 1, 0, 1),
			new PartyPartyClientStateTableRecord(9, "AllBeginPlay", "AllBeginPlay", 1, 1, 0, 1),
			new PartyPartyClientStateTableRecord(10, "Ready", "Ready", 1, 1, 0, 1),
			new PartyPartyClientStateTableRecord(11, "Sync", "Sync", 1, 1, 0, 0),
			new PartyPartyClientStateTableRecord(12, "Play", "Play", 1, 1, 0, 0),
			new PartyPartyClientStateTableRecord(13, "FinishPlay", "FinishPlay", 1, 1, 0, 0),
			new PartyPartyClientStateTableRecord(14, "News", "News", 1, 1, 0, 0),
			new PartyPartyClientStateTableRecord(15, "NewsEnd", "NewsEnd", 1, 1, 0, 0),
			new PartyPartyClientStateTableRecord(16, "Result", "Result", 1, 1, 0, 0),
			new PartyPartyClientStateTableRecord(17, "Disconnected", "Disconnected", 0, 1, 0, 0),
			new PartyPartyClientStateTableRecord(18, "Finish", "Finish", 0, 0, 0, 0),
			new PartyPartyClientStateTableRecord(19, "Error", "Error", 0, 0, 0, 0)
		};

		public static bool IsActive(this PartyPartyClientStateID self)
		{
			if (self >= PartyPartyClientStateID.First && self < PartyPartyClientStateID.End)
			{
				return self != PartyPartyClientStateID.First;
			}
			return false;
		}

		public static bool IsValid(this PartyPartyClientStateID self)
		{
			if (self >= PartyPartyClientStateID.First)
			{
				return self < PartyPartyClientStateID.End;
			}
			return false;
		}

		public static void Clamp(this PartyPartyClientStateID self)
		{
			if (self < PartyPartyClientStateID.First)
			{
				self = PartyPartyClientStateID.First;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PartyPartyClientStateID)GetEnd();
			}
		}

		public static int GetEnd(this PartyPartyClientStateID self)
		{
			return GetEnd();
		}

		public static PartyPartyClientStateID FindID(string enumName)
		{
			for (PartyPartyClientStateID partyPartyClientStateID = PartyPartyClientStateID.First; partyPartyClientStateID < PartyPartyClientStateID.End; partyPartyClientStateID++)
			{
				if (partyPartyClientStateID.GetEnumName() == enumName)
				{
					return partyPartyClientStateID;
				}
			}
			return PartyPartyClientStateID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PartyPartyClientStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PartyPartyClientStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PartyPartyClientStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static bool IsConnect(this PartyPartyClientStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isConnect;
			}
			return false;
		}

		public static bool IsNormal(this PartyPartyClientStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isNormal;
			}
			return false;
		}

		public static bool IsRequest(this PartyPartyClientStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isRequest;
			}
			return false;
		}

		public static bool IsWaitPlay(this PartyPartyClientStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isWaitPlay;
			}
			return false;
		}
	}
}
