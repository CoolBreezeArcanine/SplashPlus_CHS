namespace DB
{
	public static class PartyDeliveryCheckerStateIDEnum
	{
		private static readonly PartyDeliveryCheckerStateTableRecord[] records = new PartyDeliveryCheckerStateTableRecord[7]
		{
			new PartyDeliveryCheckerStateTableRecord(0, "First", "First"),
			new PartyDeliveryCheckerStateTableRecord(1, "ServerCheck", "ServerCheck"),
			new PartyDeliveryCheckerStateTableRecord(2, "ServerActive", "ServerActive"),
			new PartyDeliveryCheckerStateTableRecord(3, "ClientCheck", "ClientCheck"),
			new PartyDeliveryCheckerStateTableRecord(4, "ClientActive", "ClientActive"),
			new PartyDeliveryCheckerStateTableRecord(5, "Error", "Error"),
			new PartyDeliveryCheckerStateTableRecord(6, "Finish", "Finish")
		};

		public static bool IsActive(this PartyDeliveryCheckerStateID self)
		{
			if (self >= PartyDeliveryCheckerStateID.First && self < PartyDeliveryCheckerStateID.End)
			{
				return self != PartyDeliveryCheckerStateID.First;
			}
			return false;
		}

		public static bool IsValid(this PartyDeliveryCheckerStateID self)
		{
			if (self >= PartyDeliveryCheckerStateID.First)
			{
				return self < PartyDeliveryCheckerStateID.End;
			}
			return false;
		}

		public static void Clamp(this PartyDeliveryCheckerStateID self)
		{
			if (self < PartyDeliveryCheckerStateID.First)
			{
				self = PartyDeliveryCheckerStateID.First;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PartyDeliveryCheckerStateID)GetEnd();
			}
		}

		public static int GetEnd(this PartyDeliveryCheckerStateID self)
		{
			return GetEnd();
		}

		public static PartyDeliveryCheckerStateID FindID(string enumName)
		{
			for (PartyDeliveryCheckerStateID partyDeliveryCheckerStateID = PartyDeliveryCheckerStateID.First; partyDeliveryCheckerStateID < PartyDeliveryCheckerStateID.End; partyDeliveryCheckerStateID++)
			{
				if (partyDeliveryCheckerStateID.GetEnumName() == enumName)
				{
					return partyDeliveryCheckerStateID;
				}
			}
			return PartyDeliveryCheckerStateID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PartyDeliveryCheckerStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PartyDeliveryCheckerStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PartyDeliveryCheckerStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
