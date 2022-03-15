namespace DB
{
	public static class PartyConnectStateIDEnum
	{
		private static readonly PartyConnectStateTableRecord[] records = new PartyConnectStateTableRecord[4]
		{
			new PartyConnectStateTableRecord(0, "Close", "閉じている"),
			new PartyConnectStateTableRecord(1, "Connect", "接続中"),
			new PartyConnectStateTableRecord(2, "Active", "接続完了"),
			new PartyConnectStateTableRecord(3, "Shutdown", "切断中")
		};

		public static bool IsActive(this PartyConnectStateID self)
		{
			if (self >= PartyConnectStateID.Close && self < PartyConnectStateID.End)
			{
				return self != PartyConnectStateID.Close;
			}
			return false;
		}

		public static bool IsValid(this PartyConnectStateID self)
		{
			if (self >= PartyConnectStateID.Close)
			{
				return self < PartyConnectStateID.End;
			}
			return false;
		}

		public static void Clamp(this PartyConnectStateID self)
		{
			if (self < PartyConnectStateID.Close)
			{
				self = PartyConnectStateID.Close;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PartyConnectStateID)GetEnd();
			}
		}

		public static int GetEnd(this PartyConnectStateID self)
		{
			return GetEnd();
		}

		public static PartyConnectStateID FindID(string enumName)
		{
			for (PartyConnectStateID partyConnectStateID = PartyConnectStateID.Close; partyConnectStateID < PartyConnectStateID.End; partyConnectStateID++)
			{
				if (partyConnectStateID.GetEnumName() == enumName)
				{
					return partyConnectStateID;
				}
			}
			return PartyConnectStateID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PartyConnectStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PartyConnectStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PartyConnectStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
