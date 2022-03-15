namespace DB
{
	public static class PartyPartyManagerStateIDEnum
	{
		private static readonly PartyPartyManagerStateTableRecord[] records = new PartyPartyManagerStateTableRecord[8]
		{
			new PartyPartyManagerStateTableRecord(0, "First", "初期値", 0, 0, 0),
			new PartyPartyManagerStateTableRecord(1, "Setup", "初期化中", 0, 0, 0),
			new PartyPartyManagerStateTableRecord(2, "Wait", "待機", 1, 0, 0),
			new PartyPartyManagerStateTableRecord(3, "SelectMusic", "選曲", 1, 0, 0),
			new PartyPartyManagerStateTableRecord(4, "Host", "ホスト", 1, 1, 0),
			new PartyPartyManagerStateTableRecord(5, "Client", "クライアント", 1, 0, 1),
			new PartyPartyManagerStateTableRecord(6, "Finish", "終了", 1, 0, 0),
			new PartyPartyManagerStateTableRecord(7, "Error", "失敗", 0, 0, 0)
		};

		public static bool IsActive(this PartyPartyManagerStateID self)
		{
			if (self >= PartyPartyManagerStateID.First && self < PartyPartyManagerStateID.End)
			{
				return self != PartyPartyManagerStateID.First;
			}
			return false;
		}

		public static bool IsValid(this PartyPartyManagerStateID self)
		{
			if (self >= PartyPartyManagerStateID.First)
			{
				return self < PartyPartyManagerStateID.End;
			}
			return false;
		}

		public static void Clamp(this PartyPartyManagerStateID self)
		{
			if (self < PartyPartyManagerStateID.First)
			{
				self = PartyPartyManagerStateID.First;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PartyPartyManagerStateID)GetEnd();
			}
		}

		public static int GetEnd(this PartyPartyManagerStateID self)
		{
			return GetEnd();
		}

		public static PartyPartyManagerStateID FindID(string enumName)
		{
			for (PartyPartyManagerStateID partyPartyManagerStateID = PartyPartyManagerStateID.First; partyPartyManagerStateID < PartyPartyManagerStateID.End; partyPartyManagerStateID++)
			{
				if (partyPartyManagerStateID.GetEnumName() == enumName)
				{
					return partyPartyManagerStateID;
				}
			}
			return PartyPartyManagerStateID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PartyPartyManagerStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PartyPartyManagerStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PartyPartyManagerStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static bool IsNormal(this PartyPartyManagerStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isNormal;
			}
			return false;
		}

		public static bool IsHost(this PartyPartyManagerStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isHost;
			}
			return false;
		}

		public static bool IsClient(this PartyPartyManagerStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isClient;
			}
			return false;
		}
	}
}
